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

    [Checkbox(label: "Collect runtime statistics", description: "Collects cache hit rates and conveyor call counts (small overhead). Turn off to eliminate the collection overhead.")]
    public bool CollectStatistics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("World load & networking")]
    [Checkbox(label: "Fix grid merge", description: "Disable conveyor updates during grid merge (MyCubeGrid.MergeGridInternal)")]
    public bool FixGridMerge
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix grid paste", description: "Disable updates during grid paste (MyCubeGrid.PasteBlocksServer)")]
    public bool FixGridPaste
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix memory statistics", description: "Reads the process memory size for the statistics once per second instead of on every frame (MyWindowsSystem.ProcessPrivateMemory)")]
    public bool FixMemoryStats
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix P2P update stats", description: "Eliminate 98% of EOS P2P network statistics updates (VRage.EOS.MyP2PQoSAdapter.UpdateStats)")]
    public bool FixP2PUpdateStats
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix garbage collection", description: "Eliminate long pauses on starting and stopping large worlds by disabling selected GC.Collect calls")]
    public bool FixGarbageCollection
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix grid groups", description: "Disable resource updates while grids are being moved between groups")]
    public bool FixGridGroups
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Cache compiled mods", description: "Caches compiled mods for faster world load")]
    public bool CacheMods
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Cache compiled scripts", description: "Caches compiled in-game scripts (PB programs) to reduce lag")]
    public bool CacheScripts
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Disable Mod API statistics", description: "Disable the collection of Mod API call statistics to eliminate the overhead")]
    public bool DisableModApiStatistics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Faster image loading", description: "Decodes planet maps and PNG textures with a newer ImageSharp than the game ships")]
    public bool UpgradeImageSharp
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Skip asteroid voxel preloading", description: "Skips loading the vanilla asteroid voxel files on game start, they are loaded on first use instead (needs restart)")]
    public bool SkipVoxelPreload
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Faster XML loading", description: "Loads worlds, blueprints and definitions from XML faster by setting up the XML reader's name lookup once per file instead of once per block")]
    public bool FixXmlDeserialization
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("Simulation")]
    [Checkbox(label: "Lower safe zone CPU load", description: "Caches frequent recalculations in safe zones")]
    public bool FixSafeZone
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix wind turbine performance", description: "Caches the result of MyWindTurbine.IsInAtmosphere")]
    public bool FixWindTurbine
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix voxel performance", description: "Reduces memory allocations in IMyStorageExtensions.GetMaterialAt")]
    public bool FixVoxel
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix frequent memory allocations", description: "Optimizes frequent memory allocations in various parts of the game")]
    public bool FixMemory
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Less frequent sync of block counts", description: "Suppresses frequent calls to MyPlayerCollection.SendDirtyBlockLimits")]
    public bool FixBlockLimit
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Cache actions allowed by the safe zone", description: "Caches the result of MySafeZone.IsActionAllowed and MySessionComponentSafeZones.IsActionAllowedForSafezone for 2 seconds")]
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

    [Checkbox(label: "Fix turret target groups", description: "Refreshes the turret target groups in linear instead of quadratic time (MyGridTargeting.RefreshGridConnections)")]
    public bool FixTargetGroups
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("Requires server restart")]
    [Checkbox(label: "Fix turret targeting (needs restart)", description: "Reduces memory allocations in the turret targeting system (needs restart)")]
    public bool FixTargeting
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix physics performance (needs restart)", description: "Optimizes the MyPhysicsBody.RigidBody getter (needs restart)")]
    public bool FixPhysics
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Dropdown(label: "Physics thread count", description: "Game: leaves the sizing of the Havok worker thread pool to the game.\r\nAuto: one worker per physical CPU core, minus one to leave the main thread a core of its own. Hyper-threading siblings are not counted, they only add contention.\r\nManual: exactly the number below. Needs a restart.")]
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
    [Slider(HavokThreads.Min, HavokThreads.Max, 1f, SliderAttribute.SliderType.Integer, label: "Physics threads", description: "Number of Havok worker threads used in the Manual mode. In the Auto mode it shows the number this machine gets and cannot be changed, in the Game mode it is unused. Havok caps the pool on its own (11 workers on a 16 core host), so a larger number stops making a difference. Needs a restart.", enabledBy: nameof(HavokThreadCountEditable))]
    public int HavokThreadCount
    {
        get;
        set => SetField(ref field, HavokThreadCountMode == HavokThreadCountMode.Auto ? HavokThreads.Auto : value);
    } = HavokThreads.Auto;

    public bool HavokThreadCountEditable => HavokThreadCountMode != HavokThreadCountMode.Auto;

    [Checkbox(label: "Fix character performance (needs restart)", description: "Disables character footprint logic on server side (needs restart)")]
    public bool FixCharacter
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Separator("Optional")]
    [Checkbox(label: "Less frequent update of block access rights", description: "Caches the result of MyCubeBlock.GetUserRelationToOwner and MyTerminalBlock.HasPlayerAccessReason")]
    public bool FixAccess
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Skip redundant updates of PB access to blocks", description: "Skips MyGridTerminalSystem.UpdateGridBlocksOwnership before a programmable block run when the owner, the blocks and their ownership have not changed since the last run (faction and admin changes are picked up within 2 seconds)")]
    public bool FixTerminal
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

    [Checkbox(label: "Conveyor network performance fixes", description: "Caches conveyor network lookups")]
    public bool FixConveyor
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Fix toolbar group updates", description: "Caches the terminal actions of block groups on the toolbar for a second (MyToolbarItemTerminalGroup.GetActionsWithGenericDuplicates)")]
    public bool FixToolbar
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Checkbox(label: "Disable functional blocks in projected grids", description: "Disable functional blocks in projected grids without affecting the blocks built from the projection")]
    public bool FixProjection
    {
        get;
        set => SetField(ref field, value);
    } = true;

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
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}
