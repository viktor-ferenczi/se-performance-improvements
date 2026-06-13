# Index

Every documented source file, grouped by module. See the [Handbook (TOC)](TOC.md) for the guided, top-down view.

**86 files across 16 modules.**

## [Client Plugin Entry Point](modules/client-plugin.md)

| File | Path | Summary |
| --- | --- | --- |
| [`Config.cs`](files/ClientPlugin/Config.cs.md) | `ClientPlugin/Config.cs` | Declares all client-side performance-fix toggles as `bool` properties decorated with settings-framework attributes, forming the single source of truth for both the in-game dialog and the shared Harmony patch guards. |
| [`Plugin.cs`](files/ClientPlugin/Plugin.cs.md) | `ClientPlugin/Plugin.cs` | Pulsar `IPlugin` entry point that initialises Harmony patches, registers the shared plugin common state, and opens the in-game settings dialog on demand. |

## [Client Settings UI Framework](modules/client-settings.md)

| File | Path | Summary |
| --- | --- | --- |
| [`ConfigStorage.cs`](files/ClientPlugin/Settings/ConfigStorage.cs.md) | `ClientPlugin/Settings/ConfigStorage.cs` | Serialises and deserialises `Config.Current` as an XML file in the user's Space Engineers storage folder. |
| [`Button.cs`](files/ClientPlugin/Settings/Elements/Button.cs.md) | `ClientPlugin/Settings/Elements/Button.cs` | `[Button]` attribute and `IElement` implementation that exposes a `void`-returning `Action` method on `Config` as a clickable button in the settings dialog. |
| [`Checkbox.cs`](files/ClientPlugin/Settings/Elements/Checkbox.cs.md) | `ClientPlugin/Settings/Elements/Checkbox.cs` | `[Checkbox]` attribute and `IElement` implementation that renders a `bool` config property as a label + checkbox row with immediate write-through to `Config.Current`. |
| [`Color.cs`](files/ClientPlugin/Settings/Elements/Color.cs.md) | `ClientPlugin/Settings/Elements/Color.cs` | `[Color]` attribute and `IElement` implementation that renders a `VRageMath.Color` property as a label, colour-preview button, and hex textbox. |
| [`Control.cs`](files/ClientPlugin/Settings/Elements/Control.cs.md) | `ClientPlugin/Settings/Elements/Control.cs` | Immutable wrapper around a `MyGuiControlBase` that carries layout hints (`FixedWidth`, `FillFactor`, `MinWidth`, `RightMargin`, `Offset`, `OriginAlign`) consumed by the active `Layout.cs`. |
| [`Dropdown.cs`](files/ClientPlugin/Settings/Elements/Dropdown.cs.md) | `ClientPlugin/Settings/Elements/Dropdown.cs` | `[Dropdown]` attribute and `IElement` implementation that renders an `enum`-typed config property as a label + combo-box, automatically populating items from enum member names. |
| [`Element.cs`](files/ClientPlugin/Settings/Elements/Element.cs.md) | `ClientPlugin/Settings/Elements/Element.cs` | `IElement` interface — the contract every settings-dialog attribute must implement to supply typed GUI controls to `SettingsGenerator`. |
| [`Keybind.cs`](files/ClientPlugin/Settings/Elements/Keybind.cs.md) | `ClientPlugin/Settings/Elements/Keybind.cs` | `[Keybind]` attribute and `IElement` implementation that renders a `Binding.cs`-typed config property as a label, key-assignment button, and Ctrl/Alt/Shift checkboxes. |
| [`Separator.cs`](files/ClientPlugin/Settings/Elements/Separator.cs.md) | `ClientPlugin/Settings/Elements/Separator.cs` | `[Separator]` attribute and `IElement` implementation that inserts an orange section-caption label followed by a full-width horizontal rule between groups of settings. |
| [`Slider.cs`](files/ClientPlugin/Settings/Elements/Slider.cs.md) | `ClientPlugin/Settings/Elements/Slider.cs` | `[Slider]` attribute and `IElement` implementation that renders a `float` or `int` config property as a label, range slider, and live value label, with optional manual numeric entry. |
| [`Textbox.cs`](files/ClientPlugin/Settings/Elements/Textbox.cs.md) | `ClientPlugin/Settings/Elements/Textbox.cs` | `[Textbox]` attribute and `IElement` implementation that renders a `string` config property as a label + free-text input box. |
| [`Layout.cs`](files/ClientPlugin/Settings/Layouts/Layout.cs.md) | `ClientPlugin/Settings/Layouts/Layout.cs` | Abstract base class that every layout strategy must implement: it declares the screen size, the control-creation step, and the positioning step. |
| [`None.cs`](files/ClientPlugin/Settings/Layouts/None.cs.md) | `ClientPlugin/Settings/Layouts/None.cs` | A no-op `Layout.cs` that places all controls at the origin with no containers — used as the initial placeholder layout before the dialog is opened. |
| [`Simple.cs`](files/ClientPlugin/Settings/Layouts/Simple.cs.md) | `ClientPlugin/Settings/Layouts/Simple.cs` | `Layout.cs` that arranges controls in a vertically scrollable single-column list, distributing horizontal space according to each `Control`'s fill-factor. |
| [`SettingsGenerator.cs`](files/ClientPlugin/Settings/SettingsGenerator.cs.md) | `ClientPlugin/Settings/SettingsGenerator.cs` | Reflects over `Config` properties and methods at startup to build an `AttributeInfo` list, then drives the active `Layout` to materialise GUI controls and wire them to the live config. |
| [`SettingsScreen.cs`](files/ClientPlugin/Settings/SettingsScreen.cs.md) | `ClientPlugin/Settings/SettingsScreen.cs` | `MyGuiScreenBase` subclass that hosts the plugin settings controls and triggers config persistence when the dialog is closed. |
| [`Binding.cs`](files/ClientPlugin/Settings/Tools/Binding.cs.md) | `ClientPlugin/Settings/Tools/Binding.cs` | Value type representing a keyboard shortcut (key + Ctrl/Alt/Shift modifiers) with press-detection helpers. |
| [`Tools.cs`](files/ClientPlugin/Settings/Tools/Tools.cs.md) | `ClientPlugin/Settings/Tools/Tools.cs` | Shared utility class providing automatic PascalCase-to-label conversion and hex-colour parsing/formatting helpers used across the settings element classes. |

## [Server Plugin Entry Point](modules/server-plugin.md)

| File | Path | Summary |
| --- | --- | --- |
| [`PerformanceConfig.cs`](files/ServerPlugin/Config/PerformanceConfig.cs.md) | `ServerPlugin/Config/PerformanceConfig.cs` | XML-serialized server configuration implementing `IPluginConfig.cs`; each property corresponds to one toggleable performance fix. |
| [`Plugin.cs`](files/ServerPlugin/Plugin.cs.md) | `ServerPlugin/Plugin.cs` | Dedicated-server plugin entry point: loads config, applies Harmony patches, and drives the per-tick update loop. |

## [Shared Plugin Core](modules/shared-plugin-core.md)

| File | Path | Summary |
| --- | --- | --- |
| [`IPluginConfig.cs`](files/Shared/Config/IPluginConfig.cs.md) | `Shared/Config/IPluginConfig.cs` | Shared configuration contract: one boolean toggle per performance fix, plus `INotifyPropertyChanged` so both platforms can react to live config updates. |
| [`Common.cs`](files/Shared/Plugin/Common.cs.md) | `Shared/Plugin/Common.cs` | Shared bootstrap that receives the plugin instance from either host and wires up the logger, config, filesystem directories, and Harmony patch infrastructure. |
| [`ICommonPlugin.cs`](files/Shared/Plugin/ICommonPlugin.cs.md) | `Shared/Plugin/ICommonPlugin.cs` | Contract that both client and server plugin classes must implement so `Common.cs` can accept either without a circular assembly reference. |

## [Logging](modules/logging.md)

| File | Path | Summary |
| --- | --- | --- |
| [`IPluginLogger.cs`](files/Shared/Logging/IPluginLogger.cs.md) | `Shared/Logging/IPluginLogger.cs` | Environment-agnostic logging contract used by all patch code in the `Shared` project. |
| [`LogFormatter.cs`](files/Shared/Logging/LogFormatter.cs.md) | `Shared/Logging/LogFormatter.cs` | Thread-safe, allocation-minimising log message formatter that prepends a plugin prefix and appends full exception chains. |
| [`PluginLogger.cs`](files/Shared/Logging/PluginLogger.cs.md) | `Shared/Logging/PluginLogger.cs` | Concrete `IPluginLogger.cs` implementation that writes to the game's `MyLog` via `LogFormatter.cs`. |

## [Shared Tools & Data Structures](modules/tools.md)

| File | Path | Summary |
| --- | --- | --- |
| [`Cache.cs`](files/Shared/Tools/Cache.cs.md) | `Shared/Tools/Cache.cs` | Generic time-bounded key/value cache protected by a reader/writer lock. |
| [`CacheStat.cs`](files/Shared/Tools/CacheStat.cs.md) | `Shared/Tools/CacheStat.cs` | Lightweight, non-thread-safe accumulator for cache hit-rate statistics used in DEBUG builds. |
| [`CodeChange.cs`](files/Shared/Tools/CodeChange.cs.md) | `Shared/Tools/CodeChange.cs` | Data class describing a detected mismatch between the expected and actual IL hash of a patched game method. |
| [`ConveyorStat.cs`](files/Shared/Tools/ConveyorStat.cs.md) | `Shared/Tools/ConveyorStat.cs` | Non-thread-safe call-count and failure-rate accumulator for the conveyor reachability cache. |
| [`EnsureCode.cs`](files/Shared/Tools/EnsureCode.cs.md) | `Shared/Tools/EnsureCode.cs` | Custom attribute that verifies the IL hash of a Harmony-patched game method at plugin startup, detecting game updates that would break a patch. |
| [`GameAssembliesToPublicize.cs`](files/Shared/Tools/GameAssembliesToPublicize.cs.md) | `Shared/Tools/GameAssembliesToPublicize.cs` | Assembly-level `[IgnoresAccessChecksTo]` declarations listing every game assembly publicized by Krafs.Publicizer. |
| [`Hashing.cs`](files/Shared/Tools/Hashing.cs.md) | `Shared/Tools/Hashing.cs` | Static utility providing FNV-1a string hashing, IL-body hashing for Harmony `MethodInfo`/`ConstructorInfo`, and a combining hash accumulator. |
| [`IgnoresAccessChecksToAttribute.cs`](files/Shared/Tools/IgnoresAccessChecksToAttribute.cs.md) | `Shared/Tools/IgnoresAccessChecksToAttribute.cs` | Provides the `IgnoresAccessChecksToAttribute` class required at runtime when the plugin is loaded by Pulsar/Magnetar rather than built directly in an IDE. |
| [`MySessionExtensions.cs`](files/Shared/Tools/MySessionExtensions.cs.md) | `Shared/Tools/MySessionExtensions.cs` | Extension methods on `MySession` exposing the internal `m_updateAllowed` flag through publicized direct access. |
| [`ObjectPools.cs`](files/Shared/Tools/ObjectPools.cs.md) | `Shared/Tools/ObjectPools.cs` | Shared `StringBuilder` pool backed by the game's `MyConcurrentBucketPool`, reducing GC pressure from frequent string formatting. |
| [`PreloaderHelpers.cs`](files/Shared/Tools/PreloaderHelpers.cs.md) | `Shared/Tools/PreloaderHelpers.cs` | Static helper class for Mono.Cecil-based preloader patches: IL index search, hash verification, and debug IL recording over `Collection<Instruction>`. |
| [`RateLimiter.cs`](files/Shared/Tools/RateLimiter.cs.md) | `Shared/Tools/RateLimiter.cs` | Simple token-bucket rate limiter that caps the number of log messages (or any action) permitted per reporting period. |
| [`RwLock.cs`](files/Shared/Tools/RwLock.cs.md) | `Shared/Tools/RwLock.cs` | Lightweight spin-based reader/writer lock implemented as static helpers over a shared `int` counter, plus a convenience wrapper class. |
| [`RwLockDictionary.cs`](files/Shared/Tools/RwLockDictionary.cs.md) | `Shared/Tools/RwLockDictionary.cs` | `Dictionary<TK, TV>` subclass with embedded spin-based reader/writer locking, used as the backing store for all plugin caches. |
| [`RwLockHashSet.cs`](files/Shared/Tools/RwLockHashSet.cs.md) | `Shared/Tools/RwLockHashSet.cs` | `HashSet<T>` subclass with embedded spin-based reader/writer locking, used for thread-safe set membership tracking. |
| [`TranspilerHelpers.cs`](files/Shared/Tools/TranspilerHelpers.cs.md) | `Shared/Tools/TranspilerHelpers.cs` | Static helper class for Harmony transpiler patches: IL search, hash verification, debug IL recording, and deep-clone utilities for `CodeInstruction` sequences. |
| [`TwoLayerCache.cs`](files/Shared/Tools/TwoLayerCache.cs.md) | `Shared/Tools/TwoLayerCache.cs` | Two-layer cache that combines a lock-free immutable read path with a synchronized mutable write path for minimal read overhead. |
| [`UintCache.cs`](files/Shared/Tools/UintCache.cs.md) | `Shared/Tools/UintCache.cs` | Specialised time-bounded cache that packs a `uint` value and a tick-based expiry into a single `ulong`, eliminating per-entry object allocation. |
| [`Workarounds.cs`](files/Shared/Tools/Workarounds.cs.md) | `Shared/Tools/Workarounds.cs` | Shim extension methods that paper over missing or broken game/framework APIs, currently providing a null-safe `GetValueOrDefault` for `Dictionary<MyDefinitionId, MyDefinitionBase>`. |

## [Patch Infrastructure](modules/patch-infrastructure.md)

| File | Path | Summary |
| --- | --- | --- |
| [`PatchHelpers.cs`](files/Shared/Patches/PatchHelpers.cs.md) | `Shared/Patches/PatchHelpers.cs` | Central patch engine: runs `EnsureCode.cs` verification, then applies all Harmony patch classes in the assembly, and provides per-tick update and configuration hooks for every patch module. |

## [Grid Merge & Paste Patches](modules/merge-and-paste.md)

| File | Path | Summary |
| --- | --- | --- |
| [`MyConveyorLinePatch.cs`](files/Shared/Patches/MergeAndPaste/MyConveyorLinePatch.cs.md) | `Shared/Patches/MergeAndPaste/MyConveyorLinePatch.cs` | Suppresses `MyConveyorLine.UpdateIsWorking` calls for the entire duration of a grid merge, deferring the work until the merge completes. |
| [`MyCubeGridPatchForMergeAndPaste.cs`](files/Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs.md) | `Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs` | Coordinates the merge and paste performance fixes on `MyCubeGrid`: gates conveyor updates during merges via a thread-local depth counter and suppresses all world updates during paste operations. |
| [`MyGroupsPatch.cs`](files/Shared/Patches/MergeAndPaste/MyGroupsPatch.cs.md) | `Shared/Patches/MergeAndPaste/MyGroupsPatch.cs` | Currently disabled (`#if DISABLED`): intended to track when grids are being moved between logical groups so resource-distributor updates can be suppressed during those transitions. |
| [`MyResourceDistributorComponentPatch.cs`](files/Shared/Patches/MergeAndPaste/MyResourceDistributorComponentPatch.cs.md) | `Shared/Patches/MergeAndPaste/MyResourceDistributorComponentPatch.cs` | Currently disabled (`#if DISABLED`): suppresses `MyResourceDistributorComponent.UpdateBeforeSimulation` during grid group changes and defers the update to a worker thread via `MarkForUpdate()`. |

## [Conveyor System Patches](modules/conveyor.md)

| File | Path | Summary |
| --- | --- | --- |
| [`MyAssemblerPatch.cs`](files/Shared/Patches/Conveyor/MyAssemblerPatch.cs.md) | `Shared/Patches/Conveyor/MyAssemblerPatch.cs` | Experimental (disabled) patch that replaces `MyAssembler.GetMasterAssembler` with a route-cache-based lookup, avoiding `Reachable` calls entirely. |
| [`MyCubeBlockPatchForConveyor.cs`](files/Shared/Patches/Conveyor/MyCubeBlockPatchForConveyor.cs.md) | `Shared/Patches/Conveyor/MyCubeBlockPatchForConveyor.cs` | Invalidates the conveyor reachability cache when a conveyor-endpoint block changes its functional state. |
| [`MyCubeGridPatchForConveyor.cs`](files/Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs.md) | `Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs` | Invalidates or drops the conveyor reachability cache in response to grid lifecycle and topology events. |
| [`MyGridConveyorSystemPatch.cs`](files/Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs.md) | `Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs` | Caches conveyor-network reachability results per logical grid group to eliminate redundant pathfinding on large ships and bases. |
| [`MyPathFindingSystemEnumeratorPatch.cs`](files/Shared/Patches/Conveyor/MyPathFindingSystemEnumeratorPatch.cs.md) | `Shared/Patches/Conveyor/MyPathFindingSystemEnumeratorPatch.cs` | Disabled debug-only patch that counted individual graph-traversal steps in `MyPathFindingSystem<IMyConveyorEndpoint>.Enumerator.MoveNext`. |
| [`MyPathFindingSystemPatch.cs`](files/Shared/Patches/Conveyor/MyPathFindingSystemPatch.cs.md) | `Shared/Patches/Conveyor/MyPathFindingSystemPatch.cs` | Disabled debug-only patch that counted `MyPathFindingSystem<IMyConveyorEndpoint>.Reachable` call and failure rates via `ConveyorStat.cs`. |
| [`MyPathfindingDataPatch.cs`](files/Shared/Patches/Conveyor/MyPathfindingDataPatch.cs.md) | `Shared/Patches/Conveyor/MyPathfindingDataPatch.cs` | Disabled experimental patch that replaced `MyPathfindingData.Timestamp` backing storage with a `ThreadLocal<long>` to avoid lock contention. |
| [`MyShipConnectorPatchForConveyor.cs`](files/Shared/Patches/Conveyor/MyShipConnectorPatchForConveyor.cs.md) | `Shared/Patches/Conveyor/MyShipConnectorPatchForConveyor.cs` | Drops the conveyor reachability cache when a ship connector's connection state changes. |
| [`PullItemStats.cs`](files/Shared/Patches/Conveyor/PullItemStats.cs.md) | `Shared/Patches/Conveyor/PullItemStats.cs` | Debug-only statistics helper that counts `PullItem` and `PullItems` calls dispatched through the conveyor system. |

## [Physics Patches](modules/physics.md)

| File | Path | Summary |
| --- | --- | --- |
| [`MyClusterTreePatch.cs`](files/Shared/Patches/Physics/MyClusterTreePatch.cs.md) | `Shared/Patches/Physics/MyClusterTreePatch.cs` | Replaces the O(N×M) nested loop in `MyClusterTree.ReorderClusters` with a set-union approach of lower time complexity. |
| [`MyPhysicsBodyPatch.cs`](files/Shared/Patches/Physics/MyPhysicsBodyPatch.cs.md) | `Shared/Patches/Physics/MyPhysicsBodyPatch.cs` | Optimizes the `MyPhysicsBody.RigidBody` property getter by removing a redundant parent-body load sequence. |
| [`MyPhysicsPatch.cs`](files/Shared/Patches/Physics/MyPhysicsPatch.cs.md) | `Shared/Patches/Physics/MyPhysicsPatch.cs` | Fixes the Havok thread count in `MyPhysics.LoadData` so all available CPU cores are used for physics simulation. |
| [`PhysicsFixes.cs`](files/Shared/Patches/Physics/PhysicsFixes.cs.md) | `Shared/Patches/Physics/PhysicsFixes.cs` | Shared utility class exposing a helper to reconfigure `MyClusterTree` cluster-size parameters at runtime. |

## [Safe Zone Patches](modules/safe-zone.md)

| File | Path | Summary |
| --- | --- | --- |
| [`MySafeZonePatch.cs`](files/Shared/Patches/SafeZone/MySafeZonePatch.cs.md) | `Shared/Patches/SafeZone/MySafeZonePatch.cs` | Caches `MySafeZone.IsSafe` and `IsActionAllowed` results for ~2 seconds and replaces `IsOutside(BoundingBoxD)` with an allocation-free distance check. |
| [`MySessionComponentSafeZonesPatch.cs`](files/Shared/Patches/SafeZone/MySessionComponentSafeZonesPatch.cs.md) | `Shared/Patches/SafeZone/MySessionComponentSafeZonesPatch.cs` | Caches `MySessionComponentSafeZones.IsActionAllowedForSafezone` results for ~2 seconds, reducing overhead from high-frequency safe-zone action checks. |

## [Keen Overhead Removal](modules/keen-overhead-removal.md)

| File | Path | Summary |
| --- | --- | --- |
| [`GcCollectPatch.cs`](files/Shared/Patches/Bullshit/GcCollectPatch.cs.md) | `Shared/Patches/Bullshit/GcCollectPatch.cs` | Removes explicit `GC.Collect` and `IVRageSystem.CollectGC` call sites from several game methods to eliminate multi-second GC pauses during world load, unload and gameplay. |
| [`MyP2PQoSAdapterPatch.cs`](files/Shared/Patches/Bullshit/MyP2PQoSAdapterPatch.cs.md) | `Shared/Patches/Bullshit/MyP2PQoSAdapterPatch.cs` | Rate-limits `VRage.EOS.MyP2PQoSAdapter.UpdateStats` by skipping 48 out of every 49 calls and sleeping for 1 ms instead, eliminating its ~50% constant CPU core load. |
| [`PerfCountingRewriterPatch.cs`](files/Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs.md) | `Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs` | Disables `VRage.Scripting.Rewriters.PerfCountingRewriter.Rewrite` so mod Roslyn syntax trees are returned unchanged, removing the Mod API call-statistics instrumentation overhead. |

## [Memory Allocation Patches](modules/memory-allocation.md)

| File | Path | Summary |
| --- | --- | --- |
| [`MyDefinitionIdToStringPatch.cs`](files/Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs.md) | `Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs` | Caches `MyDefinitionId.ToString()` results in a two-layer cache to eliminate repeated string allocations from a hot path. |
| [`MyPlayerCollectionPatch.cs`](files/Shared/Patches/Memory/MyPlayerCollectionPatch.cs.md) | `Shared/Patches/Memory/MyPlayerCollectionPatch.cs` | Rate-limits `MyPlayerCollection.SendDirtyBlockLimits` to once every 180 ticks (~3 seconds) to reduce network and CPU overhead from too-frequent block-limit syncs. |
| [`MyStorageExtensionsPatch.cs`](files/Shared/Patches/Voxel/MyStorageExtensionsPatch.cs.md) | `Shared/Patches/Voxel/MyStorageExtensionsPatch.cs` | Eliminates per-call `MyStorageData` allocations in `IMyStorageExtensions.GetMaterialAt` by maintaining a fixed-size pool of reusable storage objects. |

## [World Loading Patches](modules/world-loading.md)

| File | Path | Summary |
| --- | --- | --- |
| [`MyDefinitionManagerPatch.cs`](files/Shared/Patches/DefinitionManager/MyDefinitionManagerPatch.cs.md) | `Shared/Patches/DefinitionManager/MyDefinitionManagerPatch.cs` | Eliminates redundant double-lookup and log flooding in `MyDefinitionManager.GetBlueprintDefinition` via a transpiler that replaces the ContainsKey+indexer pattern with a single `GetValueOrDefault` call. |
| [`MyScriptCompilerPatch.cs`](files/Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) | `Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs` | Caches compiled mod and in-game script assemblies to disk, short-circuiting the Roslyn compilation step on subsequent world loads. |

## [Simulation & Block Patches](modules/simulation-and-blocks.md)

| File | Path | Summary |
| --- | --- | --- |
| [`MyLcdSurfaceComponentPatch.cs`](files/ServerPlugin/Patches/TextPanel/MyLcdSurfaceComponentPatch.cs.md) | `ServerPlugin/Patches/TextPanel/MyLcdSurfaceComponentPatch.cs` | Suppresses `MyLcdSurfaceComponent.UpdateVisibility` on dedicated servers, where LCD surface visibility updates have no effect. Lives in the server plugin only. |
| [`MyWheelPatch.cs`](files/ServerPlugin/Patches/Wheel/MyWheelPatch.cs.md) | `ServerPlugin/Patches/Wheel/MyWheelPatch.cs` | Suppresses wheel-trail generation on dedicated servers by short-circuiting `MyWheel.CheckTrail` when `Sync.IsDedicated` is true. Lives in the server plugin only. |
| [`MyCubeBlockPatch.cs`](files/Shared/Patches/Block/MyCubeBlockPatch.cs.md) | `Shared/Patches/Block/MyCubeBlockPatch.cs` | Caches the result of `MyCubeBlock.GetUserRelationToOwner` per entity/identity pair to avoid repeated ownership lookups (file is currently compiled out via `#if UNTESTED`). |
| [`MyTerminalBlockPatch.cs`](files/Shared/Patches/Block/MyTerminalBlockPatch.cs.md) | `Shared/Patches/Block/MyTerminalBlockPatch.cs` | Caches the result of `MyTerminalBlock.HasPlayerAccessReason` per entity/identity pair to avoid repeated terminal-access checks (file is currently compiled out via `#if UNTESTED`). |
| [`MyCharacterPatch.cs`](files/Shared/Patches/Character/MyCharacterPatch.cs.md) | `Shared/Patches/Character/MyCharacterPatch.cs` | Disables server-side character footprint rendering and body-contact audio by short-circuiting the relevant branches in `MyCharacter.RigidBody_ContactPointCallback` on dedicated servers. |
| [`MyShipConnectorPatch.cs`](files/Shared/Patches/Connector/MyShipConnectorPatch.cs.md) | `Shared/Patches/Connector/MyShipConnectorPatch.cs` | Fixes a connector state-synchronisation bug by replacing `UpdateConnectionState` with a patched version that avoids redundant re-initialization and stale state on the server (file is currently compiled out via `#if UNTESTED`). |
| [`MyEntityPatchForProjection.cs`](files/Shared/Patches/Projection/MyEntityPatchForProjection.cs.md) | `Shared/Patches/Projection/MyEntityPatchForProjection.cs` | Disables functional blocks on physics-less (projected) grids the moment they are added to the scene, preventing wasteful updates and projection-era bugs such as projected welders welding in creative mode. |
| [`MyLargeTurretTargetingSystemPatch.cs`](files/Shared/Patches/TargetingSystem/MyLargeTurretTargetingSystemPatch.cs.md) | `Shared/Patches/TargetingSystem/MyLargeTurretTargetingSystemPatch.cs` | Reduces GC pressure in `MyLargeTurretTargetingSystem.SortTargetRoots` by reusing per-turret arrays instead of allocating a new one every call (entire file is currently disabled via `#if false`). |
| [`MyGridTerminalSystemPatch.cs`](files/Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs.md) | `Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs` | Rate-limits `MyGridTerminalSystem.UpdateGridBlocksOwnership` to suppress redundant PB access-right syncs (file is currently compiled out via `#if BUGGY`). |
| [`MyWindTurbinePatch.cs`](files/Shared/Patches/WindTurbine/MyWindTurbinePatch.cs.md) | `Shared/Patches/WindTurbine/MyWindTurbinePatch.cs` | Caches the result of the `MyWindTurbine.IsInAtmosphere` property getter per turbine entity for approximately 30 seconds to avoid repeated atmosphere checks. |

## [Tests](modules/tests.md)

| File | Path | Summary |
| --- | --- | --- |
| [`RateLimiterTest.cs`](files/Tests/Shared/Tools/RateLimiterTest.cs.md) | `Tests/Shared/Tools/RateLimiterTest.cs` | xUnit test verifying the core quota-and-reset behaviour of `RateLimiter.cs`. |

