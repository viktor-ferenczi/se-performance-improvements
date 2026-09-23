using System;
using System.ComponentModel;

namespace Shared.Config;

// How the number of Havok physics worker threads is decided. See
// Shared.Patches.MyWindowsSystemPatch.
public enum HavokThreadCountMode
{
    // One worker per logical processor, capped (HavokThreads.Auto)
    Auto,

    // Exactly the configured number of workers
    Manual,
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

    // One worker per logical processor, capped. The cap is what the plugin has always used
    // here; beyond it the pool costs more in scheduling than the physics step wins back.
    public static int Auto => Math.Max(Min, Math.Min(16, Environment.ProcessorCount));
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

    // Decodes planet maps and PNG textures with a newer ImageSharp than the game ships
    bool UpgradeImageSharp { get; set; }

    // Skips the eager preloading of the vanilla asteroid voxel storages on game start
    bool SkipVoxelPreload { get; set; }

    // Caches frequent recalculations in safe zones
    bool FixSafeZone { get; set; }

    // Reduces memory allocations in the turret targeting system (needs restart)
    bool FixTargeting { get; set; }

    // Caches the result of MyWindTurbine.IsInAtmosphere
    bool FixWindTurbine { get; set; }

    // Reduces memory allocations in IMyStorageExtensions.GetMaterialAt
    bool FixVoxel { get; set; }

    // Optimizes the MyPhysicsBody.RigidBody getter and sets the Havok physics thread count (needs restart)
    bool FixPhysics { get; set; }

    // Whether the Havok physics thread count is decided automatically or taken from HavokThreadCount
    HavokThreadCountMode HavokThreadCountMode { get; set; }

    // Number of Havok physics worker threads. In the Auto mode the plugin keeps this updated
    // with the number it decided, so the configuration always shows the effective count.
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

    // Suppresses frequent calls to MyGridTerminalSystem.UpdateGridBlocksOwnership updating IsAccessibleForProgrammableBlock unnecessarily often
    bool FixTerminal { get; set; }

    // Disables UpdateVisibility of LCD surfaces on multiplayer servers
    bool FixTextPanel { get; set; }

    // Caches conveyor network lookups
    bool FixConveyor { get; set; }

    // Caches the terminal actions of block groups on the toolbar (client only)
    bool FixToolbar { get; set; }

    // Rate limits excessive logging from MyDefinitionManager.GetBlueprintDefinition
    bool FixLogFlooding { get; set; }

    // Disables the tracking of wheel trails on the server, where they are not needed (trails are only visual)
    bool FixWheelTrail { get; set; }

    // Disables functional blocks in projected grids without affecting the blocks built from the projection
    bool FixProjection { get; set; }

    // Collects runtime statistics (cache hit rates, conveyor call counts) and, on the
    // dedicated server, publishes them through the PluginSdk statistics API. Turn off
    // to eliminate the small per-operation collection overhead.
    bool CollectStatistics { get; set; }
}
