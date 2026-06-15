# Conveyor System Patches

Caches conveyor-network reachability and pathfinding results and avoids redundant `IsWorking` recomputation, cutting the cost of large conveyor systems on capital ships and bases.

On grids with long conveyor networks — capital ships, production bases — `MyGridConveyorSystem.Reachable` is invoked very frequently and each call can trigger an expensive BFS traversal of the endpoint graph. This overhead is most visible as lag spikes when players enter or leave cockpits and cryopods, and as slow load times for large production grids. The fix described in the *Cached MyGridConveyorSystem.Reachable* section of `Docs/PerformanceFixes.md` addresses this by introducing a per-logical-group reachability cache.

The module is divided into three layers: the **cache core** ([`MyGridConveyorSystemPatch.cs`](../files/Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs.md)) that intercepts `Reachable` calls and stores results; the **invalidation hooks** ([`MyCubeGridPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs.md), [`MyCubeBlockPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyCubeBlockPatchForConveyor.cs.md), [`MyShipConnectorPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyShipConnectorPatchForConveyor.cs.md)) that keep the cache correct by clearing or dropping it whenever the conveyor topology changes; and a set of **inactive files** ([`MyPathFindingSystemPatch.cs`](../files/Shared/Patches/Conveyor/MyPathFindingSystemPatch.cs.md), [`MyPathFindingSystemEnumeratorPatch.cs`](../files/Shared/Patches/Conveyor/MyPathFindingSystemEnumeratorPatch.cs.md), [`MyPathfindingDataPatch.cs`](../files/Shared/Patches/Conveyor/MyPathfindingDataPatch.cs.md), [`MyAssemblerPatch.cs`](../files/Shared/Patches/Conveyor/MyAssemblerPatch.cs.md), [`PullItemStats.cs`](../files/Shared/Patches/Conveyor/PullItemStats.cs.md)) that contain experimental or debug code compiled out with `#if DISABLED` / `#if UNTESTED` / `#if DEBUG` guards.

## Files

| File | Summary |
| --- | --- |
| [`MyAssemblerPatch.cs`](../files/Shared/Patches/Conveyor/MyAssemblerPatch.cs.md) | Experimental (disabled) patch replacing `GetMasterAssembler` with an endpoint-mapping lookup that avoids `Reachable` calls. |
| [`MyCubeBlockPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyCubeBlockPatchForConveyor.cs.md) | Invalidates the reachability cache when a conveyor-endpoint block changes functional state. |
| [`MyCubeGridPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs.md) | Invalidates or drops the reachability cache in response to grid lifecycle and topology events. |
| [`MyGridConveyorSystemPatch.cs`](../files/Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs.md) | Core patch: caches `Reachable` results per logical grid group using a [`RwLockDictionary.cs`](../files/Shared/Tools/RwLockDictionary.cs.md) of [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md) instances. |
| [`MyPathFindingSystemEnumeratorPatch.cs`](../files/Shared/Patches/Conveyor/MyPathFindingSystemEnumeratorPatch.cs.md) | Disabled debug patch counting BFS traversal steps in the conveyor pathfinder's enumerator. |
| [`MyPathFindingSystemPatch.cs`](../files/Shared/Patches/Conveyor/MyPathFindingSystemPatch.cs.md) | Disabled debug patch counting `Reachable` calls and failures on the generic pathfinding system. |
| [`MyPathfindingDataPatch.cs`](../files/Shared/Patches/Conveyor/MyPathfindingDataPatch.cs.md) | Disabled experimental patch replacing `MyPathfindingData.Timestamp` with `ThreadLocal<long>` to reduce contention. |
| [`MyShipConnectorPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyShipConnectorPatchForConveyor.cs.md) | Drops the reachability cache when a connector's connection state changes (lock/unlock). |
| [`PullItemStats.cs`](../files/Shared/Patches/Conveyor/PullItemStats.cs.md) | Debug-only helper that accumulates `PullItem`/`PullItems` call counts for [`MyGridConveyorSystemPatch.cs`](../files/Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs.md). |

## How it fits together

**Cache core.** [`MyGridConveyorSystemPatch.cs`](../files/Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs.md) holds a `RwLockDictionary<TLogicalGroup, UintCache<ulong>>` (`ReachableCaches`). A cache entry key is the XOR of the two endpoint `EntityId` values (a fast, order-independent hash). The Prefix for `Reachable(IMyConveyorEndpoint, IMyConveyorEndpoint)` looks up the pair in the group's [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md); on a hit it sets `__result` and returns `false` to skip the original BFS. The Postfix stores the computed result on a miss. The second `Reachable` overload (player + item) uses the cache only for the negative case: if a pair is already known unreachable it can skip the additional filter checks too.

**Invalidation design.** Two operations are distinguished:
- `InvalidateCache(MyCubeGrid)` — clears all entries in the existing cache object for the group. Used when the topology changes but the group identity is stable (block added/removed, group membership notification, scene add, functional state change).
- `DropCache(MyCubeGrid)` — removes the cache object from the dictionary entirely. Used when the group itself may have changed or be invalid (grid split, merge, mechanical-connection state, scene remove, connector lock/unlock, ownership change, grid deletion).

The three invalidation patches each observe a different scope:
- [`MyCubeGridPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs.md) covers the broadest set of events (12 hooks on `MyCubeGrid`).
- [`MyCubeBlockPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyCubeBlockPatchForConveyor.cs.md) covers per-block functional-state changes for conveyor-endpoint blocks.
- [`MyShipConnectorPatchForConveyor.cs`](../files/Shared/Patches/Conveyor/MyShipConnectorPatchForConveyor.cs.md) covers connector lock/unlock which changes group topology.

**Inactive code.** Three pathfinding files ([`MyPathFindingSystemPatch.cs`](../files/Shared/Patches/Conveyor/MyPathFindingSystemPatch.cs.md), [`MyPathFindingSystemEnumeratorPatch.cs`](../files/Shared/Patches/Conveyor/MyPathFindingSystemEnumeratorPatch.cs.md), [`MyPathfindingDataPatch.cs`](../files/Shared/Patches/Conveyor/MyPathfindingDataPatch.cs.md)) are disabled because patching generic types with Harmony is no longer reliable. [`MyAssemblerPatch.cs`](../files/Shared/Patches/Conveyor/MyAssemblerPatch.cs.md) is excluded with `#if UNTESTED` until Publicizer support is confirmed for all build targets. [`PullItemStats.cs`](../files/Shared/Patches/Conveyor/PullItemStats.cs.md) is active only in `#if DEBUG` builds. None of these affect release performance.

**Interaction with other modules.** The conveyor cache interacts with [merge-and-paste](merge-and-paste.md) because grid merge/paste triggers many of the same grid lifecycle events handled here; both modules share the same invalidation path through `MyCubeGrid`. Ownership-change invalidation also overlaps with the safe-zone and block-access modules that react to `NotifyBlockOwnershipChange`, but each module manages its own independent cache.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
