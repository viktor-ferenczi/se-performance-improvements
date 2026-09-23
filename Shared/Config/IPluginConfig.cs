using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Shared.Config;

// How the number of Havok physics worker threads is decided. See
// Shared.Patches.MyWindowsSystemPatch.
public enum HavokThreadCountMode
{
    // The platform's number: one worker per logical processor capped at 16 on Windows,
    // one per physical core minus one on Linux (HavokThreads.Auto)
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

    // The cap on the automatic count. The shipped Havok build caps the pool below this on
    // its own (11 workers on a 16 core host), so this is the plugin's own upper bound.
    private const int AutoMax = 16;

    // The fallback cap on Linux when the physical core count cannot be determined, so a
    // hyper-threaded host does not end up with a worker per sibling thread.
    private const int LinuxFallbackMax = 7;

    // On Windows one worker per logical processor, capped. The cap is what the plugin has
    // always used here; beyond it the pool costs more in scheduling than the physics step
    // wins back.
    //
    // On Linux the game runs the Windows build of Havok through the native wrappers, where
    // the pool's workers wait on emulated Win32 events and semaphores and spin between jobs,
    // so an oversized pool costs more than it wins: a large jointed scene (a lattice of 600
    // small grids resting on each other) stepped 30% faster with two workers than with
    // eleven, while 300 independent falling grids were no worse. See the Havok section of
    // Docs/PerformanceFixes.md for the measurements.
    //
    // The count here is therefore one worker per *physical* core, minus one to leave the
    // main thread a core of its own: hyper-threading siblings share the execution units a
    // spinning physics worker is using, so they add contention rather than throughput. When
    // the topology cannot be read the logical count minus one is used, capped, which is the
    // same answer on a host without hyper-threading and a conservative one on a host with it.
    public static int Auto => Math.Max(Min, Math.Min(AutoMax, Linux ? LinuxAuto : Environment.ProcessorCount));

    private static int LinuxAuto
    {
        get
        {
            var physical = PhysicalCoreCount;
            return physical > 0
                ? physical - 1
                : Math.Min(LinuxFallbackMax, Environment.ProcessorCount - 1);
        }
    }

    private static readonly bool Linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    // Number of physical cores on Linux, or 0 when it cannot be determined. Read once: the
    // topology does not change while the process runs.
    private static readonly int PhysicalCoreCount = Linux ? CountPhysicalCores() : 0;

    // Counts the distinct hyper-threading sibling groups the kernel reports. Each group is
    // one physical core, and the list is identical for every thread of the core, so the
    // number of distinct lists is the number of cores.
    //
    // /sys is the primary source because it is architecture independent. /proc/cpuinfo is
    // the fallback for kernels or containers which do not expose the topology there; its
    // "physical id" plus "core id" pair identifies a core, and it is x86 specific, which is
    // why it is not the first choice. Anything unreadable answers 0 and the caller falls
    // back to the logical count.
    private static int CountPhysicalCores()
    {
        try
        {
            var cores = new HashSet<string>();

            foreach (var cpu in Directory.EnumerateDirectories("/sys/devices/system/cpu", "cpu[0-9]*"))
            {
                var siblings = Path.Combine(cpu, "topology", "thread_siblings_list");
                if (File.Exists(siblings))
                    cores.Add(File.ReadAllText(siblings).Trim());
            }

            if (cores.Count > 0)
                return cores.Count;

            return CountPhysicalCoresFromCpuInfo();
        }
        catch (Exception)
        {
            // Nothing here is worth failing a config object's construction over, and the
            // logger is not available yet at this point anyway.
            return 0;
        }
    }

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

    // Eliminates 98% of EOS P2P network statistics updates (VRage.EOS.MyP2PQoSAdapter.UpdateStats)
    bool FixP2PUpdateStats { get; set; }

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

    // Disables UpdateVisibility of LCD surfaces on multiplayer servers
    bool FixTextPanel { get; set; }

    // Caches conveyor network lookups
    bool FixConveyor { get; set; }

    // Caches the terminal actions of block groups on the toolbar (client only)
    bool FixToolbar { get; set; }

    // Disables the tracking of wheel trails on the server, where they are not needed (trails are only visual)
    bool FixWheelTrail { get; set; }

    // Disables functional blocks in projected grids without affecting the blocks built from the projection
    bool FixProjection { get; set; }

    // Collects runtime statistics (cache hit rates, conveyor call counts) and, on the
    // dedicated server, publishes them through the PluginSdk statistics API. Turn off
    // to eliminate the small per-operation collection overhead.
    bool CollectStatistics { get; set; }
}
