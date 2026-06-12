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
    #region Options

    // Core
    private bool enabled = true;

    // World load & networking
    private bool fixGridMerge = true;
    private bool fixGridPaste = true;
    private bool fixP2PUpdateStats = true;
    private bool fixGarbageCollection = true;
    private bool fixGridGroups = true;
    private bool cacheMods = true;
    private bool cacheScripts = true;
    private bool disableModApiStatistics = true;

    // Simulation
    private bool fixSafeZone = true;
    private bool fixWindTurbine = true;
    private bool fixVoxel = true;
    private bool fixMemory = true;
    private bool fixBlockLimit = true;
    private bool fixSafeAction = true;
    private bool fixWheelTrail = true;

    // Requires server restart
    private bool fixTargeting = true;
    private bool fixPhysics = true;
    private bool fixCharacter = true;

    // Optional
    private bool fixAccess = true;
    private bool fixTerminal = true;
    private bool fixTextPanel = true;
    private bool fixConveyor = true;
    private bool fixLogFlooding = true;
    private bool fixProjection = true;

    #endregion

    #region User interface

    public readonly string Title = "Performance";

    [Separator("Core")]
    [Checkbox(description: "Enable the plugin (all fixes)")]
    public bool Enabled
    {
        get => enabled;
        set => SetField(ref enabled, value);
    }

    [Separator("World load & networking")]
    [Checkbox(label: "Fix grid merge", description: "Disable conveyor updates during grid merge (MyCubeGrid.MergeGridInternal)")]
    public bool FixGridMerge
    {
        get => fixGridMerge;
        set => SetField(ref fixGridMerge, value);
    }

    [Checkbox(label: "Fix grid paste", description: "Disable updates during grid paste (MyCubeGrid.PasteBlocksServer)")]
    public bool FixGridPaste
    {
        get => fixGridPaste;
        set => SetField(ref fixGridPaste, value);
    }

    [Checkbox(label: "Fix P2P update stats", description: "Eliminate 98% of EOS P2P network statistics updates (VRage.EOS.MyP2PQoSAdapter.UpdateStats)")]
    public bool FixP2PUpdateStats
    {
        get => fixP2PUpdateStats;
        set => SetField(ref fixP2PUpdateStats, value);
    }

    [Checkbox(label: "Fix garbage collection", description: "Eliminate long pauses on starting and stopping large worlds by disabling selected GC.Collect calls")]
    public bool FixGarbageCollection
    {
        get => fixGarbageCollection;
        set => SetField(ref fixGarbageCollection, value);
    }

    [Checkbox(label: "Fix grid groups", description: "Disable resource updates while grids are being moved between groups")]
    public bool FixGridGroups
    {
        get => fixGridGroups;
        set => SetField(ref fixGridGroups, value);
    }

    [Checkbox(label: "Cache compiled mods", description: "Caches compiled mods for faster world load")]
    public bool CacheMods
    {
        get => cacheMods;
        set => SetField(ref cacheMods, value);
    }

    [Checkbox(label: "Cache compiled scripts", description: "Caches compiled in-game scripts (PB programs) to reduce lag")]
    public bool CacheScripts
    {
        get => cacheScripts;
        set => SetField(ref cacheScripts, value);
    }

    [Checkbox(label: "Disable Mod API statistics", description: "Disable the collection of Mod API call statistics to eliminate the overhead")]
    public bool DisableModApiStatistics
    {
        get => disableModApiStatistics;
        set => SetField(ref disableModApiStatistics, value);
    }

    [Separator("Simulation")]
    [Checkbox(label: "Lower safe zone CPU load", description: "Caches frequent recalculations in safe zones")]
    public bool FixSafeZone
    {
        get => fixSafeZone;
        set => SetField(ref fixSafeZone, value);
    }

    [Checkbox(label: "Fix wind turbine performance", description: "Caches the result of MyWindTurbine.IsInAtmosphere")]
    public bool FixWindTurbine
    {
        get => fixWindTurbine;
        set => SetField(ref fixWindTurbine, value);
    }

    [Checkbox(label: "Fix voxel performance", description: "Reduces memory allocations in IMyStorageExtensions.GetMaterialAt")]
    public bool FixVoxel
    {
        get => fixVoxel;
        set => SetField(ref fixVoxel, value);
    }

    [Checkbox(label: "Fix frequent memory allocations", description: "Optimizes frequent memory allocations in various parts of the game")]
    public bool FixMemory
    {
        get => fixMemory;
        set => SetField(ref fixMemory, value);
    }

    [Checkbox(label: "Less frequent sync of block counts", description: "Suppresses frequent calls to MyPlayerCollection.SendDirtyBlockLimits")]
    public bool FixBlockLimit
    {
        get => fixBlockLimit;
        set => SetField(ref fixBlockLimit, value);
    }

    [Checkbox(label: "Cache actions allowed by the safe zone", description: "Caches the result of MySafeZone.IsActionAllowed and MySessionComponentSafeZones.IsActionAllowedForSafezone for 2 seconds")]
    public bool FixSafeAction
    {
        get => fixSafeAction;
        set => SetField(ref fixSafeAction, value);
    }

    [Checkbox(label: "Disable tracking of wheel trails on server", description: "Disable the tracking of wheel trails on server, where they are not needed at all (trails are only visual elements)")]
    public bool FixWheelTrail
    {
        get => fixWheelTrail;
        set => SetField(ref fixWheelTrail, value);
    }

    [Separator("Requires server restart")]
    [Checkbox(label: "Fix targeting allocations (needs restart)", description: "Reduces memory allocations in the turret targeting system (needs restart)")]
    public bool FixTargeting
    {
        get => fixTargeting;
        set => SetField(ref fixTargeting, value);
    }

    [Checkbox(label: "Fix physics performance (needs restart)", description: "Optimizes the MyPhysicsBody.RigidBody getter (needs restart)")]
    public bool FixPhysics
    {
        get => fixPhysics;
        set => SetField(ref fixPhysics, value);
    }

    [Checkbox(label: "Fix character performance (needs restart)", description: "Disables character footprint logic on server side (needs restart)")]
    public bool FixCharacter
    {
        get => fixCharacter;
        set => SetField(ref fixCharacter, value);
    }

    [Separator("Optional")]
    [Checkbox(label: "Less frequent update of block access rights", description: "Caches the result of MyCubeBlock.GetUserRelationToOwner and MyTerminalBlock.HasPlayerAccessReason")]
    public bool FixAccess
    {
        get => fixAccess;
        set => SetField(ref fixAccess, value);
    }

    [Checkbox(label: "Less frequent update of PB access to blocks", description: "Suppresses frequent calls to MyGridTerminalSystem.UpdateGridBlocksOwnership updating IsAccessibleForProgrammableBlock unnecessarily often")]
    public bool FixTerminal
    {
        get => fixTerminal;
        set => SetField(ref fixTerminal, value);
    }

    [Checkbox(label: "Text panel performance fixes", description: "Disables UpdateVisibility of LCD surfaces on multiplayer servers (disable this if LCDs flicker on clients)")]
    public bool FixTextPanel
    {
        get => fixTextPanel;
        set => SetField(ref fixTextPanel, value);
    }

    [Checkbox(label: "Conveyor network performance fixes", description: "Caches conveyor network lookups")]
    public bool FixConveyor
    {
        get => fixConveyor;
        set => SetField(ref fixConveyor, value);
    }

    [Checkbox(label: "Rate limit logs with flooding potential", description: "Rate limited excessive logging from MyDefinitionManager.GetBlueprintDefinition")]
    public bool FixLogFlooding
    {
        get => fixLogFlooding;
        set => SetField(ref fixLogFlooding, value);
    }

    [Checkbox(label: "Disable functional blocks in projected grids", description: "Disable functional blocks in projected grids without affecting the blocks built from the projection")]
    public bool FixProjection
    {
        get => fixProjection;
        set => SetField(ref fixProjection, value);
    }

    #endregion

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
