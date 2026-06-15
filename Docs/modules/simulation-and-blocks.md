# Simulation & Block Patches

Assorted per-block and per-subsystem fixes: turret targeting allocations, block access-right caching, wind-turbine atmosphere caching, PB block-ownership sync, disabled projected blocks, server-side footprint/wheel-trail removal and LCD surface tweaks.

This module collects ten independent, single-file Harmony patches that each target a specific CPU or GC hot-spot in the game's simulation loop. The fixes fall into three broad categories: caching frequently re-evaluated results that change rarely (`MyWindTurbinePatch`, `MyCubeBlockPatch`, `MyTerminalBlockPatch`, `MyGridTerminalSystemPatch`), eliminating server-side work that has no effect without a renderer (`MyCharacterPatch`, `MyWheelPatch`, `MyLcdSurfaceComponentPatch`), and one-time or structural corrections (`MyEntityPatchForProjection`, `MyShipConnectorPatch`, `MyLargeTurretTargetingSystemPatch`).

Several patches (`MyCubeBlockPatch`, `MyTerminalBlockPatch`, `MyGridTerminalSystemPatch`, `MyShipConnectorPatch`, `MyLargeTurretTargetingSystemPatch`) are currently compiled out (`#if UNTESTED`, `#if BUGGY`, or `#if false`) while they await further testing or a rewrite targeting the current game version. The remaining patches (`MyWindTurbinePatch`, `MyCharacterPatch`, `MyWheelPatch`, `MyLcdSurfaceComponentPatch`, `MyEntityPatchForProjection`) are active.

## Files

| File | Summary |
| --- | --- |
| [`MyCubeBlockPatch.cs`](../files/Shared/Patches/Block/MyCubeBlockPatch.cs.md) | Caches `GetUserRelationToOwner` results per entity/identity pair to avoid repeated ownership lookups (UNTESTED). |
| [`MyTerminalBlockPatch.cs`](../files/Shared/Patches/Block/MyTerminalBlockPatch.cs.md) | Caches `HasPlayerAccessReason` results per entity/identity pair to avoid repeated terminal-access checks (UNTESTED). |
| [`MyCharacterPatch.cs`](../files/Shared/Patches/Character/MyCharacterPatch.cs.md) | Skips server-side footprint rendering and contact-sound dispatch on dedicated servers. |
| [`MyShipConnectorPatch.cs`](../files/Shared/Patches/Connector/MyShipConnectorPatch.cs.md) | Replaces per-tick connector state re-evaluation with an event-driven approach to cut redundant CPU work (UNTESTED). |
| [`MyEntityPatchForProjection.cs`](../files/Shared/Patches/Projection/MyEntityPatchForProjection.cs.md) | Disables functional blocks on physics-less grids at scene-add time to prevent wasteful updates and projection bugs. |
| [`MyLargeTurretTargetingSystemPatch.cs`](../files/Shared/Patches/TargetingSystem/MyLargeTurretTargetingSystemPatch.cs.md) | Reduces GC pressure in `SortTargetRoots` by reusing per-turret arrays instead of allocating a new one every call (disabled). |
| [`MyGridTerminalSystemPatch.cs`](../files/Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs.md) | Rate-limits `UpdateGridBlocksOwnership` to suppress redundant PB block-access syncs (BUGGY). |
| [`MyLcdSurfaceComponentPatch.cs`](../files/ServerPlugin/Patches/TextPanel/MyLcdSurfaceComponentPatch.cs.md) | Skips `UpdateVisibility` for LCD surface components on dedicated servers. |
| [`MyWheelPatch.cs`](../files/ServerPlugin/Patches/Wheel/MyWheelPatch.cs.md) | Skips wheel-trail generation on dedicated servers where it has no visual effect. |
| [`MyWindTurbinePatch.cs`](../files/Shared/Patches/WindTurbine/MyWindTurbinePatch.cs.md) | Caches the `IsInAtmosphere` property result per turbine for ~30 seconds to avoid repeated atmosphere checks. |

## How it fits together

The patches in this module share no runtime state with each other; each one is self-contained. Their common infrastructure is the [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md) primitive used by the four caching patches ([`MyCubeBlockPatch.cs`](../files/Shared/Patches/Block/MyCubeBlockPatch.cs.md), [`MyTerminalBlockPatch.cs`](../files/Shared/Patches/Block/MyTerminalBlockPatch.cs.md), [`MyGridTerminalSystemPatch.cs`](../files/Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs.md), and [`MyWindTurbinePatch.cs`](../files/Shared/Patches/WindTurbine/MyWindTurbinePatch.cs.md)), all of which call `Cache.Cleanup()` from a per-frame `Update()` method to advance TTL expiry.

The three server-only no-ops all test `Sync.IsDedicated` and are pure skip-if-server guards that neither allocate nor cache. [`MyCharacterPatch.cs`](../files/Shared/Patches/Character/MyCharacterPatch.cs.md) stays in `Shared/` and injects its dedicated-server check via a transpiler. [`MyWheelPatch.cs`](../files/ServerPlugin/Patches/Wheel/MyWheelPatch.cs.md) and [`MyLcdSurfaceComponentPatch.cs`](../files/ServerPlugin/Patches/TextPanel/MyLcdSurfaceComponentPatch.cs.md) were moved out of `Shared/` into the server plugin (`ServerPlugin/Patches/`) — replacing their old `#if TORCH || DEDICATED` compile guard — so they now compile only into the server build and check `Sync.IsDedicated` at runtime. Their matching client config flags (`FixWheelTrail`, `FixTextPanel`) are inert stubs on the client (see [`Config.cs`](../files/ClientPlugin/Config.cs.md)).

[`MyEntityPatchForProjection.cs`](../files/Shared/Patches/Projection/MyEntityPatchForProjection.cs.md) operates at scene-add time and sets `Enabled = false` once, so it has no per-tick cost after the initial disable.

The three disabled patches ([`MyCubeBlockPatch.cs`](../files/Shared/Patches/Block/MyCubeBlockPatch.cs.md), [`MyTerminalBlockPatch.cs`](../files/Shared/Patches/Block/MyTerminalBlockPatch.cs.md), [`MyGridTerminalSystemPatch.cs`](../files/Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs.md), [`MyShipConnectorPatch.cs`](../files/Shared/Patches/Connector/MyShipConnectorPatch.cs.md), [`MyLargeTurretTargetingSystemPatch.cs`](../files/Shared/Patches/TargetingSystem/MyLargeTurretTargetingSystemPatch.cs.md)) are compiled out but their design is complete; they will be re-enabled once stability is confirmed. [`MyCubeBlockPatch.cs`](../files/Shared/Patches/Block/MyCubeBlockPatch.cs.md) and [`MyTerminalBlockPatch.cs`](../files/Shared/Patches/Block/MyTerminalBlockPatch.cs.md) share the same `Config.FixAccess` config flag.

This module interacts with [world-loading](world-loading.md) only through the shared [`Common.cs`](../files/Shared/Plugin/Common.cs.md) plugin singleton; there are no direct dependencies between the two modules.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
