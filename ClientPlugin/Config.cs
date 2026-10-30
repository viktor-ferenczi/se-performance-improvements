using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClientPlugin.Settings;
using ClientPlugin.Settings.Elements;
using Shared.Config;

namespace ClientPlugin;

// Client-side configuration, edited through the in-game settings dialog
// (see ClientPlugin/Settings). It implements the shared IPluginConfig so the
// Harmony patches in the Shared project can gate on it via Plugin.Common.Config.
//
// The options are grouped into the same sections as the server config
// (ServerPlugin.Config.PerformanceConfig), declared via [Separator] since the
// in-game dialog has no tabs. Unlike the server, the client defaults all fixes
// ON: a single player / offline player wants maximum performance, so there is
// no "off by default" qualifier on the Optional section here.
public class Config : IPluginConfig
{
    public readonly string Title = "Performance";

    [Separator("Core")]
    [Checkbox(description: "Enable the plugin (all fixes)")]
    public bool Enabled
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Collect runtime statistics",
        description: "Collects cache hit rates and conveyor call counts (small overhead). Turn off to eliminate the collection overhead."
    )]
    public bool CollectStatistics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("World load & networking")]
    [Checkbox(
        label: "Fix grid merge",
        description: "Disable conveyor updates during grid merge (MyCubeGrid.MergeGridInternal)"
    )]
    public bool FixGridMerge
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix grid paste",
        description: "Disable updates during grid paste (MyCubeGrid.PasteBlocksServer)"
    )]
    public bool FixGridPaste
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix memory statistics",
        description: "Reads the process memory size for the statistics once per second instead of on every frame (MyWindowsSystem.ProcessPrivateMemory)"
    )]
    public bool FixMemoryStats
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix garbage collection",
        description: "Eliminate long pauses on starting and stopping large worlds by disabling selected GC.Collect calls"
    )]
    public bool FixGarbageCollection
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix grid groups",
        description: "Disable resource updates while grids are being moved between groups"
    )]
    public bool FixGridGroups
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Cache compiled mods",
        description: "Caches compiled mods for faster world load"
    )]
    public bool CacheMods
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Disable Mod API statistics",
        description: "Disable the collection of Mod API call statistics to eliminate the overhead"
    )]
    public bool DisableModApiStatistics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Faster image loading",
        description: "Decodes planet maps and PNG textures with a newer ImageSharp than the game ships"
    )]
    public bool UpgradeImageSharp
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Skip asteroid voxel preloading",
        description: "Skips loading the vanilla asteroid voxel files on game start, they are loaded on first use instead (needs restart)"
    )]
    public bool SkipVoxelPreload
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Faster XML loading",
        description: "Loads worlds, blueprints and definitions from XML faster by setting up the XML reader's name lookup once per file instead of once per block"
    )]
    public bool FixXmlDeserialization
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("Simulation")]
    [Checkbox(
        label: "Lower safe zone CPU load",
        description: "Caches frequent recalculations in safe zones"
    )]
    public bool FixSafeZone
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix wind turbine performance",
        description: "Caches the result of MyWindTurbine.IsInAtmosphere"
    )]
    public bool FixWindTurbine
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix voxel performance",
        description: "Reduces memory allocations in IMyStorageExtensions.GetMaterialAt"
    )]
    public bool FixVoxel
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix frequent memory allocations",
        description: "Optimizes frequent memory allocations in various parts of the game"
    )]
    public bool FixMemory
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Less frequent sync of block counts",
        description: "Suppresses frequent calls to MyPlayerCollection.SendDirtyBlockLimits"
    )]
    public bool FixBlockLimit
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Cache actions allowed by the safe zone",
        description: "Caches the result of MySafeZone.IsActionAllowed and MySessionComponentSafeZones.IsActionAllowedForSafezone for 2 seconds"
    )]
    public bool FixSafeAction
    {
        get;
        set => SetField(ref field, value);
    } = true;

    // Server only
    public bool FixWheelTrail
    {
        get => false;
        set { }
    }

    [Checkbox(
        label: "Separate stuck grids",
        description: "Pushes apart grids stuck inside each other after a collision, like a ship rammed into a station in a no-damage safe zone. Only in offline games and multiplayer games hosted by this client."
    )]
    public bool SeparateStuckGrids
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Push grids out of voxels",
        description: "Pushes a grid stuck inside a planet or an asteroid out of it, like a ship rammed into an asteroid in a no-damage safe zone. Only in offline games and multiplayer games hosted by this client."
    )]
    public bool PushGridsOutOfVoxels
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix turret target groups",
        description: "Refreshes the turret target groups in linear instead of quadratic time (MyGridTargeting.RefreshGridConnections)"
    )]
    public bool FixTargetGroups
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("Requires server restart")]
    [Checkbox(
        label: "Fix turret targeting (needs restart)",
        description: "Reduces memory allocations in the turret targeting system (needs restart)"
    )]
    public bool FixTargeting
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix physics performance (needs restart)",
        description: "Optimizes the MyPhysicsBody.RigidBody getter (needs restart)"
    )]
    public bool FixPhysics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Dropdown(
        label: "Physics thread count",
        description: "Game: leaves the sizing of the Havok worker thread pool to the game.\r\nAuto: one worker per physical CPU core, minus one to leave the main thread a core of its own, at most 6. Hyper-threading siblings are not counted, they only add contention.\r\nManual: exactly the number below. Needs a restart."
    )]
    public HavokThreadCountMode HavokThreadCountMode
    {
        get;
        set
        {
            if (SetField(ref field, value) && value == HavokThreadCountMode.Auto)
                HavokThreadCount = HavokThreads.Auto;
        }
    } = HavokThreadCountMode.Auto;

    // In the Auto mode the count is always this machine's number, whatever was stored or set
    [Slider(
        HavokThreads.Min,
        HavokThreads.Max,
        1f,
        SliderAttribute.SliderType.Integer,
        label: "Physics threads",
        description: "Number of Havok worker threads used in the Manual mode. It can only be changed in the Manual mode. In the Auto mode it shows the number this machine gets, in the Game mode it is unused. Havok caps the pool on its own (11 workers on a 16 core host), so a larger number stops making a difference. Needs a restart.",
        enabledBy: nameof(HavokThreadCountEditable)
    )]
    public int HavokThreadCount
    {
        get;
        set =>
            SetField(
                ref field,
                HavokThreadCountMode == HavokThreadCountMode.Auto ? HavokThreads.Auto : value
            );
    } = HavokThreads.Auto;

    public bool HavokThreadCountEditable => HavokThreadCountMode == HavokThreadCountMode.Manual;

    [Checkbox(
        label: "Fix character performance (needs restart)",
        description: "Disables character footprint logic on server side (needs restart)"
    )]
    public bool FixCharacter
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("Optional")]
    [Checkbox(
        label: "Less frequent update of block access rights",
        description: "Caches the result of MyCubeBlock.GetUserRelationToOwner and MyTerminalBlock.HasPlayerAccessReason"
    )]
    public bool FixAccess
    {
        get;
        set => SetField(ref field, value);
    } = true;

    // Server only
    public bool FixTextPanel
    {
        get => false;
        set { }
    }

    [Checkbox(
        label: "Conveyor network performance fixes",
        description: "Caches conveyor network lookups"
    )]
    public bool FixConveyor
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Fix toolbar group updates",
        description: "Caches the terminal actions of block groups on the toolbar for a second (MyToolbarItemTerminalGroup.GetActionsWithGenericDuplicates)"
    )]
    public bool FixToolbar
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Disable functional blocks in projected grids",
        description: "Disable functional blocks in projected grids without affecting the blocks built from the projection"
    )]
    public bool FixProjection
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("Programmable blocks")]
    [Checkbox(
        label: "Cache compiled scripts",
        description: "Caches compiled in-game scripts (PB programs) to reduce lag"
    )]
    public bool CacheScripts
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Skip redundant updates of PB access to blocks",
        description: "Skips MyGridTerminalSystem.UpdateGridBlocksOwnership before a programmable block run when the owner, the blocks and their ownership have not changed since the last run (faction and admin changes are picked up within 2 seconds)"
    )]
    public bool FixTerminal
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Per-thread PB instruction counter (needs restart)",
        description: "Gives every thread its own programmable block instruction counter, so scripts can run off the main thread. Needs a restart."
    )]
    public bool FixScriptCounter
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [Slider(
        0.1f,
        1000f,
        0.1f,
        SliderAttribute.SliderType.Float,
        label: "PB run time limit",
        description: "Longest time a single programmable block run may take. The game's own limit is 3000 ms. A script over the limit is stopped in the middle of the run, like one over the instruction limit. Needs the per-thread PB instruction counter.",
        enabledBy: nameof(FixScriptCounter),
        unit: "ms"
    )]
    public float ScriptRunLimitMs
    {
        get;
        set => SetField(ref field, value);
    } = 100f;

    [Slider(
        0.1f,
        100f,
        0.1f,
        SliderAttribute.SliderType.Float,
        label: "PB soft run time limit",
        description: "A script whose own code runs longer than this in 3 runs in a row is stopped. One slow run does not stop it. Needs the per-thread PB instruction counter.",
        enabledBy: nameof(FixScriptCounter),
        unit: "ms"
    )]
    public float ScriptSoftLimitMs
    {
        get;
        set => SetField(ref field, value);
    } = 5f;

    [Checkbox(
        label: "PB proxies",
        description: "Hands the scripts that could run off the main thread proxy objects instead of the game objects they use, so their game calls can be checked and their writes delayed. Applies to scripts compiled or loaded afterwards.",
        enabledBy: nameof(FixScriptCounter)
    )]
    public bool FixScriptProxies
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [Checkbox(
        label: "PB frame budget",
        description: "Holds the programmable block runs of a frame to the time budget below and delays the rest to later frames, the longest waiting first. Runs from timers, buttons and other scripts are not delayed, but count against the budget. Needs the per-thread PB instruction counter.",
        enabledBy: nameof(FixScriptCounter)
    )]
    public bool FixScriptScheduler
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [Slider(
        0.1f,
        100f,
        0.1f,
        SliderAttribute.SliderType.Float,
        label: "PB time per frame",
        description: "Programmable block time per frame. A run starts only if the time used so far plus the longest of its last 10 run times fits.",
        enabledBy: nameof(ScriptFrameBudgetEditable),
        unit: "ms"
    )]
    public float ScriptFrameBudgetMs
    {
        get;
        set => SetField(ref field, value);
    } = 5f;

    [Dropdown(
        label: "PB thread count",
        description: "Auto: the same number as the automatic physics thread count. Manual: exactly the number below. Used by the PB frame budget for the scripts with proxies, which then run on that many threads, the main thread included.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public ScriptThreadCountMode ScriptThreadCountMode
    {
        get;
        set => SetField(ref field, value);
    } = ScriptThreadCountMode.Auto;

    [Slider(
        ScriptThreads.Min,
        ScriptThreads.Max,
        1f,
        SliderAttribute.SliderType.Integer,
        label: "PB threads",
        description: "Number of threads running programmable block scripts in the Manual mode, the main thread included.",
        enabledBy: nameof(ScriptThreadCountEditable)
    )]
    public int ScriptThreadCount
    {
        get;
        set => SetField(ref field, value);
    } = ScriptThreads.Auto;

    [Separator("PB on worker thread")]
    [Checkbox(
        label: "LCD sprites",
        description: "Scripts drawing sprites on LCDs (DrawFrame) may run on a worker thread. The sprites are collected on the worker and sent to the LCD on the main thread after the scripts ran. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainSprites
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Block access checks",
        description: "Scripts checking access to blocks and grids (CanAccess) may run on a worker thread. The checks read ownership the main thread prepares before the scripts run. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainAccess
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Terminal properties and actions",
        description: "Scripts reading terminal properties and actions (GetProperty, GetActionWithName, GetValue, SetValue, ApplyAction) may run on a worker thread. Lookups use per-thread lists, and values set and actions applied are made on the main thread after the scripts ran. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainTerminal
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Block detailed info",
        description: "Scripts reading DetailedInfo of blocks may run on a worker thread. The game's shared number formatting buffer becomes per thread. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainDetailedInfo
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Intergrid communication",
        description: "Scripts using IGC (broadcast and unicast listeners and messages) may run on a worker thread. Listener registration and sends are serialized, messages are delivered as before. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainIgc
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Gravity of ship controllers",
        description: "Scripts reading the artificial or total gravity of ship controllers and parachutes may run on a worker thread. The gravity generators are brought up to date on the main thread before the scripts run. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainGravity
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Sensors, turrets and cameras",
        description: "Scripts reading turret, turret controller and sensor targets, camera raycasts, the nearest player of a remote control or the hacking state of blocks may run on a worker thread. Raycasts query the physics world from worker threads. Higher risk: off by default because it puts physics queries on worker threads. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainSensing
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [Checkbox(
        label: "Inventory transfers",
        description: "Scripts moving items between inventories or checking conveyor connections (TransferItemTo, CanTransferItemTo, IsConnectedTo) may run on a worker thread. The moves are made on the main thread after the scripts ran. Higher risk: off by default because a script sees the old inventory contents until its next run, so a loop moving items until an inventory is empty would not end. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainTransfers
    {
        get;
        set => SetField(ref field, value);
    } = false;

    [Checkbox(
        label: "Running other PBs",
        description: "Scripts running other programmable blocks (TryRun) may run on a worker thread. The other PB runs on the same thread if its script may run on a worker thread too and it is in the same grid group; otherwise TryRun returns false, as for a busy PB. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainTryRun
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Inventory helpers",
        description: "Scripts using the inventory helper methods of terminal blocks (HasInventory, GetInventory, GetInventoryCount, Get/SetUseConveyorSystem) may run on a worker thread. Needs the PB frame budget and proxies with more than one PB thread.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptOffMainInventory
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(
        label: "Log thread decisions",
        description: "Logs for each programmable block whether its script runs on the main thread or on a worker thread, and why: the game members that keep it on the main thread, or the PB on worker thread options it needs. Logged when a PB is first seen and when the decision changes. Off by default because it is only for diagnosing why a script stays on the main thread; it logs only on changes, so it can stay on.",
        enabledBy: nameof(ScriptThreadsUsed)
    )]
    public bool ScriptLogThreadDecisions
    {
        get;
        set => SetField(ref field, value);
    } = false;

    public bool ScriptFrameBudgetEditable => FixScriptCounter && FixScriptScheduler;

    // The thread count matters only when the frame budget runs scripts with proxies
    public bool ScriptThreadsUsed => FixScriptCounter && FixScriptScheduler && FixScriptProxies;

    public bool ScriptThreadCountEditable =>
        ScriptThreadsUsed && ScriptThreadCountMode == ScriptThreadCountMode.Manual;

    #region Property change notification boilerplate

    public static readonly Config Default = new Config();
    public static readonly Config Current = ConfigStorage.Load();

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}
