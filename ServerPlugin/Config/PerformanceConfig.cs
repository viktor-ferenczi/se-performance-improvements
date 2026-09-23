using PluginSdk.Config;
using Shared.Config;

namespace ServerPlugin.Config;

// Server-side configuration, declared through Magnetar's PluginSdk. Admins edit
// it remotely via Quasar, which renders the UI from the layout declared below.
// It also implements the shared IPluginConfig so the Harmony patches in the
// Shared project gate on it via Plugin.Common.Config. INotifyPropertyChanged is
// provided by PluginSdk.Config.PluginConfig.
//
// The defaults are the conservative server defaults: a few gameplay/visual
// affecting fixes (block access rights, PB access, LCD visibility, conveyor
// caching, log rate limiting, projected blocks) are left OFF so an admin opts
// into them deliberately. The client defaults everything ON.
[Tab("general", caption: "General")]
[Tab("worldload", caption: "World load & networking")]
[Tab("simulation", caption: "Simulation")]
[Tab("restart", caption: "Requires server restart")]
[Tab("optional", caption: "Optional (off by default)")]
[Section("core", parent: "general", caption: "Core")]
public class PerformanceConfig : PluginConfig, IPluginConfig
{
    // ---- Core -----------------------------------------------------------

    [BoolOption("Enable the plugin (all fixes)", Parent = "core")]
    public bool Enabled { get; set => SetField(ref field, value); } = true;

    [BoolOption("Collect runtime statistics and publish them to the control plane (small overhead)", Parent = "core")]
    public bool CollectStatistics { get; set => SetField(ref field, value); } = true;

    // ---- World load & networking ---------------------------------------

    [BoolOption("Disable conveyor updates during grid merge (MyCubeGrid.MergeGridInternal)", Parent = "worldload")]
    public bool FixGridMerge { get; set => SetField(ref field, value); } = true;

    [BoolOption("Disable updates during grid paste (MyCubeGrid.PasteBlocksServer)", Parent = "worldload")]
    public bool FixGridPaste { get; set => SetField(ref field, value); } = true;

    [BoolOption("Eliminate 98% of EOS P2P network statistics updates (VRage.EOS.MyP2PQoSAdapter.UpdateStats)", Parent = "worldload")]
    public bool FixP2PUpdateStats { get; set => SetField(ref field, value); } = true;

    [BoolOption("Eliminate long pauses on starting and stopping large worlds by disabling selected GC.Collect calls", Parent = "worldload")]
    public bool FixGarbageCollection { get; set => SetField(ref field, value); } = true;

    [BoolOption("Disable resource updates while grids are being moved between groups", Parent = "worldload")]
    public bool FixGridGroups { get; set => SetField(ref field, value); } = true;

    [BoolOption("Cache compiled mods for faster world load", Parent = "worldload")]
    public bool CacheMods { get; set => SetField(ref field, value); } = true;

    [BoolOption("Cache compiled in-game scripts (PB programs) to reduce lag", Parent = "worldload")]
    public bool CacheScripts { get; set => SetField(ref field, value); } = true;

    [BoolOption("Disable the collection of Mod API call statistics to eliminate the overhead", Parent = "worldload")]
    public bool DisableModApiStatistics { get; set => SetField(ref field, value); } = true;

    [BoolOption("Decode planet maps with a newer ImageSharp than the game ships", Parent = "worldload")]
    public bool UpgradeImageSharp { get; set => SetField(ref field, value); } = true;

    // ---- Simulation -----------------------------------------------------

    [BoolOption("Cache frequent recalculations in safe zones", Parent = "simulation")]
    public bool FixSafeZone { get; set => SetField(ref field, value); } = true;

    [BoolOption("Cache the result of MyWindTurbine.IsInAtmosphere", Parent = "simulation")]
    public bool FixWindTurbine { get; set => SetField(ref field, value); } = true;

    [BoolOption("Reduce memory allocations in IMyStorageExtensions.GetMaterialAt", Parent = "simulation")]
    public bool FixVoxel { get; set => SetField(ref field, value); } = true;

    [BoolOption("Optimize frequent memory allocations in various parts of the game", Parent = "simulation")]
    public bool FixMemory { get; set => SetField(ref field, value); } = true;

    [BoolOption("Suppress frequent calls to MyPlayerCollection.SendDirtyBlockLimits", Parent = "simulation")]
    public bool FixBlockLimit { get; set => SetField(ref field, value); } = true;

    [BoolOption("Cache the result of MySafeZone.IsActionAllowed and MySessionComponentSafeZones.IsActionAllowedForSafezone for 2 seconds", Parent = "simulation")]
    public bool FixSafeAction { get; set => SetField(ref field, value); } = true;

    [BoolOption("Disable tracking of wheel trails on the server, where they are not needed (trails are only visual)", Parent = "simulation")]
    public bool FixWheelTrail { get; set => SetField(ref field, value); } = true;

    // ---- Requires server restart ---------------------------------------

    [BoolOption("Reduce memory allocations in the turret targeting system (needs restart)", Parent = "restart")]
    public bool FixTargeting { get; set => SetField(ref field, value); } = true;

    [BoolOption("Optimize the MyPhysicsBody.RigidBody getter and set the Havok physics thread count (needs restart)", Parent = "restart")]
    public bool FixPhysics { get; set => SetField(ref field, value); } = true;

    [EnumOption("Physics thread count: Auto is one Havok worker thread per logical processor capped at 16, Manual is exactly the number below (needs restart)", Parent = "restart")]
    public HavokThreadCountMode HavokThreadCountMode { get; set => SetField(ref field, value); } = HavokThreadCountMode.Auto;

    [IntOption(HavokThreads.Min, HavokThreads.Max, "Number of Havok worker threads used in the Manual mode; in the Auto mode it shows the number this machine gets. Havok caps the pool on its own (11 workers on a 16 core host), so a larger number stops making a difference (needs restart)", Parent = "restart")]
    public int HavokThreadCount { get; set => SetField(ref field, value); } = HavokThreads.Auto;

    [BoolOption("Disable character footprint logic on the server side (needs restart)", Parent = "restart")]
    public bool FixCharacter { get; set => SetField(ref field, value); } = true;

    // ---- Optional (off by default) -------------------------------------

    [BoolOption("Cache the result of MyCubeBlock.GetUserRelationToOwner and MyTerminalBlock.HasPlayerAccessReason", Parent = "optional")]
    public bool FixAccess { get; set => SetField(ref field, value); } = false;

    [BoolOption("Suppress frequent MyGridTerminalSystem.UpdateGridBlocksOwnership calls updating IsAccessibleForProgrammableBlock", Parent = "optional")]
    public bool FixTerminal { get; set => SetField(ref field, value); } = false;

    [BoolOption("Disable UpdateVisibility of LCD surfaces on multiplayer servers (disable if LCDs flicker on clients)", Parent = "optional")]
    public bool FixTextPanel { get; set => SetField(ref field, value); } = false;

    [BoolOption("Cache conveyor network lookups", Parent = "optional")]
    public bool FixConveyor { get; set => SetField(ref field, value); } = false;

    // Client only: the dedicated server has no toolbar updates
    public bool FixToolbar
    {
        get => false;
        set { }
    }

    [BoolOption("Rate limit excessive logging from MyDefinitionManager.GetBlueprintDefinition", Parent = "optional")]
    public bool FixLogFlooding { get; set => SetField(ref field, value); } = false;

    [BoolOption("Disable functional blocks in projected grids without affecting the blocks built from the projection", Parent = "optional")]
    public bool FixProjection { get; set => SetField(ref field, value); } = false;

    [BoolOption("Skip loading the vanilla asteroid voxel files on server start, they are loaded on first use instead (needs restart)", Parent = "optional")]
    public bool SkipVoxelPreload { get; set => SetField(ref field, value); } = false;
}
