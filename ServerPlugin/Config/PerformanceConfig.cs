using PluginSdk.Config;
using Shared.Config;

namespace ServerPlugin.Config;

// Server-side configuration, declared through Magnetar's PluginSdk. Admins edit
// it remotely via Quasar, which renders the UI from the layout declared below.
// It also implements the shared IPluginConfig so the Harmony patches in the
// Shared project gate on it via Plugin.Common.Config. INotifyPropertyChanged is
// provided by PluginSdk.Config.PluginConfig.
//
// The server defaults come from weighing each option's risk on a server against its
// gain (SE-635). Left OFF: the conveyor lookup cache, which has hung servers, and the
// two which change what players see or what mods find (LCD surface visibility,
// functional blocks in projected grids). The PB access fix is ON:
// after a faction or admin change it can serve the old access flags for up to two
// seconds. Everything else defaults ON, including the fixes which only trade eager
// work for lazy work or replace an algorithm with a faster one of the same result.
// The client defaults everything ON.
[Tab("general", caption: "General")]
[Tab("worldload", caption: "World load & networking")]
[Tab("simulation", caption: "Simulation")]
[Tab("restart", caption: "Requires server restart")]
[Tab("optional", caption: "Optional (off by default)")]
[Tab("scripts", caption: "Programmable blocks")]
[Tab("pbworker", caption: "PB on worker thread")]
[Section("core", parent: "general", caption: "Core")]
public class PerformanceConfig : PluginConfig, IPluginConfig
{
    // ---- Core -----------------------------------------------------------

    [BoolOption("Enable the plugin (all fixes)", Parent = "core")]
    public bool Enabled
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Collect runtime statistics and publish them to the control plane (small overhead)",
        Parent = "core"
    )]
    public bool CollectStatistics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    // ---- World load & networking ---------------------------------------

    [BoolOption(
        "Disable conveyor updates during grid merge (MyCubeGrid.MergeGridInternal)",
        Parent = "worldload"
    )]
    public bool FixGridMerge
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Disable updates during grid paste (MyCubeGrid.PasteBlocksServer)",
        Parent = "worldload"
    )]
    public bool FixGridPaste
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Read the process memory size for the statistics once per second instead of on every frame",
        Parent = "worldload"
    )]
    public bool FixMemoryStats
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Eliminate long pauses on starting and stopping large worlds by disabling selected GC.Collect calls",
        Parent = "worldload"
    )]
    public bool FixGarbageCollection
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Disable resource updates while grids are being moved between groups",
        Parent = "worldload"
    )]
    public bool FixGridGroups
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption("Cache compiled mods for faster world load", Parent = "worldload")]
    public bool CacheMods
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Disable the collection of Mod API call statistics to eliminate the overhead",
        Parent = "worldload"
    )]
    public bool DisableModApiStatistics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Decode planet maps and PNG textures with a newer ImageSharp than the game ships",
        Parent = "worldload"
    )]
    public bool UpgradeImageSharp
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Skip loading the vanilla asteroid voxel files on server start, they are loaded on first use instead (needs restart)",
        Parent = "worldload"
    )]
    public bool SkipVoxelPreload
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Load worlds, blueprints and definitions from XML faster by setting up the XML reader's name lookup once per file instead of once per block",
        Parent = "worldload"
    )]
    public bool FixXmlDeserialization
    {
        get;
        set => SetField(ref field, value);
    } = true;

    // ---- Simulation -----------------------------------------------------

    [BoolOption("Cache frequent recalculations in safe zones", Parent = "simulation")]
    public bool FixSafeZone
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption("Cache the result of MyWindTurbine.IsInAtmosphere", Parent = "simulation")]
    public bool FixWindTurbine
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Reduce memory allocations in IMyStorageExtensions.GetMaterialAt",
        Parent = "simulation"
    )]
    public bool FixVoxel
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Optimize frequent memory allocations in various parts of the game",
        Parent = "simulation"
    )]
    public bool FixMemory
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Suppress frequent calls to MyPlayerCollection.SendDirtyBlockLimits",
        Parent = "simulation"
    )]
    public bool FixBlockLimit
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Cache the result of MySafeZone.IsActionAllowed and MySessionComponentSafeZones.IsActionAllowedForSafezone for 2 seconds",
        Parent = "simulation"
    )]
    public bool FixSafeAction
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Disable tracking of wheel trails on the server, where they are not needed (trails are only visual)",
        Parent = "simulation"
    )]
    public bool FixWheelTrail
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Push apart two grids stuck inside each other after a collision, which costs most of the simulation speed while it lasts, like a ship rammed into a station in a safe zone without damage. Static grids never move",
        Parent = "simulation"
    )]
    public bool SeparateStuckGrids
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Push a grid stuck inside a planet or an asteroid out of it, which costs most of the simulation speed while it lasts, like a ship teleported into the ground or rammed into an asteroid in a safe zone without damage. Static grids never move",
        Parent = "simulation"
    )]
    public bool PushGridsOutOfVoxels
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Refresh the turret target groups in linear instead of quadratic time",
        Parent = "simulation"
    )]
    public bool FixTargetGroups
    {
        get;
        set => SetField(ref field, value);
    } = true;

    // ---- Requires server restart ---------------------------------------

    [BoolOption(
        "Reduce memory allocations in the turret targeting system (needs restart)",
        Parent = "restart"
    )]
    public bool FixTargeting
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption("Optimize the MyPhysicsBody.RigidBody getter (needs restart)", Parent = "restart")]
    public bool FixPhysics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [EnumOption(
        "Physics thread count: Game leaves the sizing of the Havok worker thread pool to the game; Auto is one worker per physical CPU core, minus one to leave the main thread a core of its own, at most 6 so several servers can share a host, not counting hyper-threading siblings which only add contention; a physics heavy world may want more in Manual; Manual is exactly the number below (needs restart)",
        Parent = "restart"
    )]
    public HavokThreadCountMode HavokThreadCountMode
    {
        get;
        set => SetField(ref field, value);
    } = HavokThreadCountMode.Auto;

    [IntOption(
        HavokThreads.Min,
        HavokThreads.Max,
        "Number of Havok worker threads used in the Manual mode; in the Auto mode it shows the number this machine gets. Havok caps the pool on its own (11 workers on a 16 core host), so a larger number stops making a difference (needs restart)",
        Parent = "restart"
    )]
    public int HavokThreadCount
    {
        get;
        set => SetField(ref field, value);
    } = HavokThreads.Auto;

    [BoolOption(
        "Disable character footprint logic on the server side (needs restart)",
        Parent = "restart"
    )]
    public bool FixCharacter
    {
        get;
        set => SetField(ref field, value);
    } = true;

    // ---- Optional (off by default) -------------------------------------

    [BoolOption(
        "Cache the result of MyCubeBlock.GetUserRelationToOwner and MyTerminalBlock.HasPlayerAccessReason",
        Parent = "optional"
    )]
    public bool FixAccess
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [BoolOption(
        "Disable UpdateVisibility of LCD surfaces on multiplayer servers (disable if LCDs flicker on clients)",
        Parent = "optional"
    )]
    public bool FixTextPanel
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [BoolOption("Cache conveyor network lookups", Parent = "optional")]
    public bool FixConveyor
    {
        get;
        set => SetField(ref field, value);
    } = false;

    // Client only: the dedicated server has no toolbar updates
    public bool FixToolbar
    {
        get => false;
        set { }
    }

    [BoolOption(
        "Disable functional blocks in projected grids without affecting the blocks built from the projection",
        Parent = "optional"
    )]
    public bool FixProjection
    {
        get;
        set => SetField(ref field, value);
    } = false;

    // ---- Programmable blocks -------------------------------------------

    [BoolOption("Cache compiled in-game scripts (PB programs) to reduce lag", Parent = "scripts")]
    public bool CacheScripts
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Skip MyGridTerminalSystem.UpdateGridBlocksOwnership before a programmable block run when the owner, the blocks and their ownership have not changed since the last run (faction and admin changes are picked up within 2 seconds)",
        Parent = "scripts"
    )]
    public bool FixTerminal
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Give every thread its own programmable block instruction counter, so scripts can run off the main thread (needs restart)",
        Parent = "scripts"
    )]
    public bool FixScriptCounter
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [FloatOption(
        0.1f,
        1000f,
        "Longest time a single programmable block run may take, in milliseconds; the game's own limit is 3000. A script over the limit is stopped like one over the instruction limit. Needs the per-thread instruction counter",
        Parent = "scripts"
    )]
    public float ScriptRunLimitMs
    {
        get;
        set => SetField(ref field, value);
    } = 100f;

    [FloatOption(
        0.1f,
        100f,
        "A programmable block script whose own code runs longer than this, in milliseconds, in 3 runs in a row is stopped; one slow run does not stop it. Needs the per-thread instruction counter",
        Parent = "scripts"
    )]
    public float ScriptSoftLimitMs
    {
        get;
        set => SetField(ref field, value);
    } = 5f;

    [BoolOption(
        "Hand the programmable block scripts that could run off the main thread proxy objects instead of the game objects they use, so their game calls can be checked and their writes delayed. Applies to scripts compiled or loaded afterwards",
        Parent = "scripts"
    )]
    public bool FixScriptProxies
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [BoolOption(
        "Hold the programmable block runs of a frame to the time budget below and delay the rest to later frames, the longest waiting first. Runs from timers, buttons and other scripts are not delayed, but count against the budget. Needs the per-thread instruction counter",
        Parent = "scripts"
    )]
    public bool FixScriptScheduler
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [FloatOption(
        0.1f,
        100f,
        "Programmable block time per frame, in milliseconds. A run starts only if the time used so far plus the longest of its last 10 run times fits",
        Parent = "scripts"
    )]
    public float ScriptFrameBudgetMs
    {
        get;
        set => SetField(ref field, value);
    } = 5f;

    [EnumOption(
        "Programmable block thread count: Auto is the same number as the automatic physics thread count; Manual is exactly the number below. Used by the PB frame budget for the scripts with proxies, which then run on that many threads, the main thread included",
        Parent = "scripts"
    )]
    public ScriptThreadCountMode ScriptThreadCountMode
    {
        get;
        set => SetField(ref field, value);
    } = ScriptThreadCountMode.Auto;

    [IntOption(
        ScriptThreads.Min,
        ScriptThreads.Max,
        "Number of threads running programmable block scripts in the Manual mode, the main thread included",
        Parent = "scripts"
    )]
    public int ScriptThreadCount
    {
        get;
        set => SetField(ref field, value);
    } = ScriptThreads.Auto;

    [BoolOption(
        "Scripts drawing sprites on LCDs (DrawFrame) may run on a worker thread. The sprites are collected on the worker and sent to the LCD on the main thread after the scripts ran. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainSprites
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Scripts checking access to blocks and grids (CanAccess) may run on a worker thread. The checks read ownership the main thread prepares before the scripts run. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainAccess
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Scripts reading terminal properties and actions (GetProperty, GetActionWithName, GetValue, SetValue, ApplyAction) may run on a worker thread. Lookups use per-thread lists, and values set and actions applied are made on the main thread after the scripts ran. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainTerminal
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Scripts reading DetailedInfo of blocks may run on a worker thread. The game's shared number formatting buffer becomes per thread. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainDetailedInfo
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Scripts using IGC (broadcast and unicast listeners and messages) may run on a worker thread. Listener registration and sends are serialized, messages are delivered as before. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainIgc
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Scripts reading the artificial or total gravity of ship controllers and parachutes may run on a worker thread. The gravity generators are brought up to date on the main thread before the scripts run. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainGravity
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Scripts reading turret, turret controller and sensor targets, camera raycasts, the nearest player of a remote control or the hacking state of blocks may run on a worker thread. Raycasts query the physics world from worker threads. Higher risk: off by default because it puts physics queries on worker threads. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainSensing
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [BoolOption(
        "Scripts moving items between inventories or checking conveyor connections (TransferItemTo, CanTransferItemTo, IsConnectedTo) may run on a worker thread. The moves are made on the main thread after the scripts ran. Higher risk: off by default because a script sees the old inventory contents until its next run, so a loop moving items until an inventory is empty would not end. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainTransfers
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [BoolOption(
        "Scripts running other programmable blocks (TryRun) may run on a worker thread. The other PB runs on the same thread if its script may run on a worker thread too and it is in the same grid group; otherwise TryRun returns false, as for a busy PB. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainTryRun
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Scripts using the inventory helper methods of terminal blocks (HasInventory, GetInventory, GetInventoryCount, Get/SetUseConveyorSystem) may run on a worker thread. Needs the PB frame budget and proxies with more than one PB thread",
        Parent = "pbworker"
    )]
    public bool ScriptOffMainInventory
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [BoolOption(
        "Log for each programmable block whether its script runs on the main thread or on a worker thread, and why: the game members that keep it on the main thread, or the PB on worker thread options it needs. Logged when a PB is first seen and when the decision changes. Off by default because it is only for diagnosing why a script stays on the main thread; it logs only on changes, so it can stay on",
        Parent = "pbworker"
    )]
    public bool ScriptLogThreadDecisions
    {
        get;
        set => SetField(ref field, value);
    } = false;
}
