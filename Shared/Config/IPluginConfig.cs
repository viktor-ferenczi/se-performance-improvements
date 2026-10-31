using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Shared.Config;

// How the number of Havok physics worker threads is decided. See
// Shared.Patches.MyWindowsSystemPatch.
public enum HavokThreadCountMode
{
    // One worker per physical CPU core, minus one, at most 6 (HavokThreads.Auto)
    Auto,

    // Exactly the configured number of workers
    Manual,

    // Leaves the game's own sizing alone: MyWindowsSystem keeps answering null and Havok
    // sizes the pool on its own terms, exactly as without the plugin
    Game,
}

// The range and the automatic value of the Havok physics thread count. It lives next to the
// configuration rather than in the patch which applies it, so the client and server config
// classes can use it in their attributes and defaults without pulling the patch (and with it
// Common, which is not set up yet while a config object is being constructed) into their
// static initialization.
public static class HavokThreads
{
    // Two, not one: see the comment in Shared.Patches.MyWindowsSystemPatch for why one
    // worker thread is not single threaded physics. The maximum is a sanity limit.
    public const int Min = 2;
    public const int Max = 64;

    // The cap on the automatic count. Several servers often share a host (cluster nodes,
    // or any other automation running more than one Magnetar instance), and every pool's
    // workers spin between jobs, so a pool per server sized to the whole machine makes them
    // fight over the same cores. Six keeps that bearable by default; a physics heavy world
    // can still ask for more with Manual.
    private const int AutoMax = 6;

    // One Havok worker per physical core, minus one so the main thread keeps a core of its
    // own.
    //
    // Physical rather than logical, on both platforms. The pool's workers spin between jobs,
    // and a spinning worker shares the execution units of its hyper-threading sibling, so
    // counting siblings as cores inflates the pool without adding throughput. On Linux, where
    // the game runs the Windows build of Havok through the native wrappers and the workers
    // wait on emulated Win32 events and semaphores, an oversized pool is measurably worse: a
    // lattice of 600 small grids resting on each other stepped 30% faster with two workers
    // than with eleven, while 300 independent falling grids were no worse. See the Havok
    // section of Docs/PerformanceFixes.md for the measurements.
    //
    // When the topology cannot be read the logical count stands in for it; the cap keeps that
    // from going wrong on a host with hyper-threading.
    public static int Auto
    {
        get
        {
            var cores = PhysicalCoreCount > 0 ? PhysicalCoreCount : Environment.ProcessorCount;
            return AutoForCoreCount(cores);
        }
    }

    internal static int AutoForCoreCount(int cores) => Math.Max(Min, Math.Min(AutoMax, cores - 1));

    private static readonly bool Linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    private static readonly bool Windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    // Number of physical cores, or 0 when it cannot be determined. Read once: the topology
    // does not change while the process runs.
    private static readonly int PhysicalCoreCount = CountPhysicalCores();

    private static int CountPhysicalCores()
    {
        try
        {
            if (Linux)
                return CountPhysicalCoresFromSysfs();

            if (Windows)
                return CountPhysicalCoresFromWin32();

            return 0;
        }
        catch (Exception)
        {
            // Nothing here is worth failing a config object's construction over, and the
            // logger is not available yet at this point anyway.
            return 0;
        }
    }

    // Counts the distinct hyper-threading sibling groups the kernel reports. Each group is
    // one physical core, and the list is identical for every thread of the core, so the
    // number of distinct lists is the number of cores.
    //
    // /sys is the primary source because it is architecture independent. /proc/cpuinfo is
    // the fallback for kernels or containers which do not expose the topology there; its
    // "physical id" plus "core id" pair identifies a core, and it is x86 specific, which is
    // why it is not the first choice.
    private static int CountPhysicalCoresFromSysfs()
    {
        var cores = new HashSet<string>();

        foreach (var cpu in Directory.EnumerateDirectories("/sys/devices/system/cpu", "cpu[0-9]*"))
        {
            var siblings = Path.Combine(cpu, "topology", "thread_siblings_list");
            if (File.Exists(siblings))
                cores.Add(File.ReadAllText(siblings).Trim());
        }

        return cores.Count > 0 ? cores.Count : CountPhysicalCoresFromCpuInfo();
    }

    // Asks Windows for the processor relationships and counts the RelationProcessorCore
    // records, one per physical core. GetLogicalProcessorInformationEx rather than the
    // original GetLogicalProcessorInformation because the latter only describes the first
    // processor group, so it stops at 64 logical processors.
    //
    // The records are variable length and only their count is needed here, so the buffer is
    // walked by the Size field of each record without decoding the union which follows it.
    private static int CountPhysicalCoresFromWin32()
    {
        var length = 0;
        if (GetLogicalProcessorInformationEx(RelationProcessorCore, IntPtr.Zero, ref length))
            return 0;

        if (Marshal.GetLastWin32Error() != ErrorInsufficientBuffer || length <= 0)
            return 0;

        var buffer = Marshal.AllocHGlobal(length);
        try
        {
            if (!GetLogicalProcessorInformationEx(RelationProcessorCore, buffer, ref length))
                return 0;

            var cores = 0;
            for (var offset = 0; offset + RecordHeaderSize <= length; )
            {
                // struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX { DWORD Relationship; DWORD Size; ... }
                var size = Marshal.ReadInt32(buffer, offset + sizeof(int));
                if (size <= 0)
                    break;

                cores++;
                offset += size;
            }

            return cores;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private const int RelationProcessorCore = 0;
    private const int ErrorInsufficientBuffer = 122;
    private const int RecordHeaderSize = 2 * sizeof(int);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetLogicalProcessorInformationEx(
        int relationshipType,
        IntPtr buffer,
        ref int returnedLength
    );

    private static int CountPhysicalCoresFromCpuInfo()
    {
        if (!File.Exists("/proc/cpuinfo"))
            return 0;

        var cores = new HashSet<string>();
        string package = null;
        string core = null;

        foreach (var line in File.ReadLines("/proc/cpuinfo"))
        {
            if (line.Length == 0)
            {
                package = core = null;
                continue;
            }

            var colon = line.IndexOf(':');
            if (colon < 0)
                continue;

            var key = line.Substring(0, colon).Trim();
            var value = line.Substring(colon + 1).Trim();

            if (key == "physical id")
                package = value;
            else if (key == "core id")
                core = value;

            if (package != null && core != null)
            {
                cores.Add($"{package}/{core}");
                package = core = null;
            }
        }

        return cores.Count;
    }
}

// How the number of programmable block worker threads is decided (ScriptScheduler)
public enum ScriptThreadCountMode
{
    // The same number as the automatic Havok physics thread count (HavokThreads.Auto)
    Auto,

    // Exactly the configured number of threads
    Manual,
}

// The range and the automatic value of the programmable block worker thread count
public static class ScriptThreads
{
    public const int Min = 1;
    public const int Max = 64;

    public static int Auto => HavokThreads.Auto;
}

// Configuration properties shared by the patches in the Shared project.
//
// The client implements this interface with its in-game settings dialog
// (ClientPlugin.Config), the server implements it with a Magnetar PluginSdk
// configuration class (ServerPlugin.Config.PerformanceConfig). The patches
// only ever see this interface via Plugin.Common.Config.
public interface IPluginConfig : INotifyPropertyChanged
{
    // Enables the plugin
    bool Enabled { get; set; }

    // Disables conveyor updates during grid merge (MyCubeGrid.MergeGridInternal)
    bool FixGridMerge { get; set; }

    // Disables updates during grid paste (MyCubeGrid.PasteBlocksServer)
    bool FixGridPaste { get; set; }

    // Eliminates long pauses on starting and stopping large worlds by disabling selected GC.Collect calls
    bool FixGarbageCollection { get; set; }

    // Disables resource updates while grids are being moved between groups
    bool FixGridGroups { get; set; }

    // Caches compiled mods for faster world load
    bool CacheMods { get; set; }

    // Caches compiled in-game scripts (PB programs) to reduce lag
    bool CacheScripts { get; set; }

    // Disables Mod API call statistics collection to eliminate the overhead
    bool DisableModApiStatistics { get; set; }

    // Refreshes the process memory size read for the statistics once per second instead of every frame
    bool FixMemoryStats { get; set; }

    // Decodes planet maps and PNG textures with a newer ImageSharp than the game ships
    bool UpgradeImageSharp { get; set; }

    // Skips the eager preloading of the vanilla asteroid voxel storages on game start
    bool SkipVoxelPreload { get; set; }

    // Sets up the XML readers' name IDs once per file instead of once per block
    bool FixXmlDeserialization { get; set; }

    // Caches frequent recalculations in safe zones
    bool FixSafeZone { get; set; }

    // Reduces memory allocations in the turret targeting system (needs restart)
    bool FixTargeting { get; set; }

    // Refreshes the turret target groups in linear instead of quadratic time
    bool FixTargetGroups { get; set; }

    // Caches the result of MyWindTurbine.IsInAtmosphere
    bool FixWindTurbine { get; set; }

    // Reduces memory allocations in IMyStorageExtensions.GetMaterialAt
    bool FixVoxel { get; set; }

    // Optimizes the MyPhysicsBody.RigidBody getter (needs restart)
    bool FixPhysics { get; set; }

    // Whether the Havok physics thread count is left to the game, decided automatically or
    // taken from HavokThreadCount
    HavokThreadCountMode HavokThreadCountMode { get; set; }

    // Number of Havok physics worker threads. In the Auto mode the plugin keeps this updated
    // with the number it decided, so the configuration always shows the effective count. It is
    // not used in the Game mode.
    int HavokThreadCount { get; set; }

    // Disables character footprint logic on server side (needs restart)
    bool FixCharacter { get; set; }

    // Optimizes frequent memory allocations
    bool FixMemory { get; set; }

    // Caches the result of MyCubeBlock.GetUserRelationToOwner and MyTerminalBlock.HasPlayerAccessReason
    bool FixAccess { get; set; }

    // Suppresses frequent calls to MyPlayerCollection.SendDirtyBlockLimits
    bool FixBlockLimit { get; set; }

    // Caches the result of MySafeZone.IsActionAllowed and MySessionComponentSafeZones.IsActionAllowedForSafezone for 2 seconds
    bool FixSafeAction { get; set; }

    // Skips MyGridTerminalSystem.UpdateGridBlocksOwnership before a programmable block run when nothing it depends on changed
    bool FixTerminal { get; set; }

    // Gives every thread its own programmable block instruction counter, so scripts can run off
    // the main thread (needs restart: the Preloader applies it before the game loads)
    bool FixScriptCounter { get; set; }

    // Longest time a single programmable block run may take, in milliseconds (the game's own limit
    // is 3000). Takes effect only with FixScriptCounter.
    float ScriptRunLimitMs { get; set; }

    // A programmable block script whose own code runs longer than this, in milliseconds, in
    // ScriptRunner.SoftLimitRuns runs in a row is stopped. Takes effect only with FixScriptCounter.
    float ScriptSoftLimitMs { get; set; }

    // Hands programmable block scripts that can run on a worker thread proxies instead of the
    // game objects behind their roots; applies to scripts created after it changes
    bool FixScriptProxies { get; set; }

    // Holds the programmable block runs of a frame to ScriptFrameBudgetMs, delaying the rest to
    // later frames. Takes effect only with FixScriptCounter.
    bool FixScriptScheduler { get; set; }

    // Programmable block time per thread and frame, in milliseconds, for FixScriptScheduler
    float ScriptFrameBudgetMs { get; set; }

    // Whether the programmable block worker thread count is decided automatically or taken from
    // ScriptThreadCount. FixScriptScheduler runs the scripts with proxies (FixScriptProxies) on that
    // many threads, the main thread included.
    ScriptThreadCountMode ScriptThreadCountMode { get; set; }

    // Number of programmable block worker threads in the Manual mode
    int ScriptThreadCount { get; set; }

    // Scripts drawing sprites on LCDs (DrawFrame) may run on a worker thread. The sprites are
    // collected on the worker and sent to the LCD on the main thread after the scripts ran. See
    // StandIn.
    bool ScriptOffMainSprites { get; set; }

    // Scripts checking access to blocks and grids (CanAccess) may run on a worker thread. The checks
    // read ownership the main thread prepares before the scripts run. See StandIn.
    bool ScriptOffMainAccess { get; set; }

    // Scripts reading terminal properties and actions (GetProperty, GetActionWithName, GetValue,
    // SetValue, ApplyAction) may run on a worker thread. Lookups use per-thread lists, and values set
    // and actions applied are made on the main thread after the scripts ran. See StandIn.
    bool ScriptOffMainTerminal { get; set; }

    // Scripts reading DetailedInfo of blocks may run on a worker thread. The game's shared number
    // formatting buffer becomes per thread. See StandIn.
    bool ScriptOffMainDetailedInfo { get; set; }

    // Scripts using IGC (broadcast and unicast listeners and messages) may run on a worker thread.
    // Listener registration and sends are serialized, messages are delivered as before. See StandIn.
    bool ScriptOffMainIgc { get; set; }

    // Scripts reading the artificial or total gravity of ship controllers and parachutes may run off
    // the main thread. The gravity generators are brought up to date on the main thread before the
    // scripts run. See StandIn.
    bool ScriptOffMainGravity { get; set; }

    // Scripts reading turret, turret controller and sensor targets, camera raycasts, the nearest
    // player of a remote control or the hacking state of blocks may run on a worker thread. Raycasts
    // query the physics world from worker threads. Off by default. See StandIn.
    bool ScriptOffMainSensing { get; set; }

    // Scripts moving items between inventories or checking conveyor connections (TransferItemTo,
    // CanTransferItemTo, IsConnectedTo) may run on a worker thread. Off by default. See StandIn.
    bool ScriptOffMainTransfers { get; set; }

    // Scripts running other programmable blocks (TryRun) may run on a worker thread. The other PB
    // runs on the same thread if its script may run on a worker thread too and it is in the same grid
    // group; otherwise TryRun returns false, as for a busy PB. See StandIn.
    bool ScriptOffMainTryRun { get; set; }

    // Scripts using the inventory helper methods of terminal blocks (HasInventory, GetInventory,
    // GetInventoryCount, Get/SetUseConveyorSystem) may run on a worker thread. See StandIn.
    bool ScriptOffMainInventory { get; set; }

    // Logs where each programmable block script runs, on the main thread or off it, and why
    bool ScriptLogThreadDecisions { get; set; }

    // Disables UpdateVisibility of LCD surfaces on multiplayer servers
    bool FixTextPanel { get; set; }

    // Caches conveyor network lookups
    bool FixConveyor { get; set; }

    // Caches the terminal actions of block groups on the toolbar (client only)
    bool FixToolbar { get; set; }

    // Disables the tracking of wheel trails on the server, where they are not needed (trails are only visual)
    bool FixWheelTrail { get; set; }

    // Pushes apart grids stuck inside each other after a collision (only where the game is the server)
    bool SeparateStuckGrids { get; set; }

    // Pushes non-static grids out of planets and asteroids they are stuck in (only where the game is the server)
    bool PushGridsOutOfVoxels { get; set; }

    // Removes dynamic grids lying entirely inside a planet (only where the game is the server)
    bool RemoveBuriedGrids { get; set; }

    // Minutes between two sweeps for buried grids
    int BuriedGridSweepMinutes { get; set; }

    // Of ships joined by locked connectors, only the strongest not being flown keeps its dampeners and gyros slowing the group down (only where the game is the server)
    bool CalmConnectedShips { get; set; }

    // Disables functional blocks in projected grids without affecting the blocks built from the projection
    bool FixProjection { get; set; }

    // Collects runtime statistics (cache hit rates, conveyor call counts) and, on the
    // dedicated server, publishes them through the PluginSdk statistics API. Turn off
    // to eliminate the small per-operation collection overhead.
    bool CollectStatistics { get; set; }
}
