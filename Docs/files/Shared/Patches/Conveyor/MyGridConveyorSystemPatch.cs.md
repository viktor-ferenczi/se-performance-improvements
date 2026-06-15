# `Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs`

*Caches conveyor-network reachability results per logical grid group to eliminate redundant pathfinding on large ships and bases.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyGridConveyorSystemPatch.cs`](../../../../../Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs) (319 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyGridConveyorSystem.Reachable` is called very frequently on grids with long conveyor networks — capital ships, production bases — and each call can trigger an expensive pathfinding traversal. This patch adds a [`UintCache.cs`](../../Tools/UintCache.cs.md) keyed by XOR of the two endpoint `EntityId` values, with one cache instance per logical grid group stored in a [`RwLockDictionary.cs`](../../Tools/RwLockDictionary.cs.md).

On a cache hit, the Prefix skips the original method entirely and returns the stored boolean result. On a miss, the original method runs and the Postfix stores the result. The overload that takes a player-id and item-definition additionally uses the cache to short-circuit the entire call whenever the endpoint pair is already known to be unreachable (it cannot short-circuit known-reachable pairs because the player/item filters may differ).

Cache invalidation is not done inside this class directly; instead the sibling patches ([`MyCubeGridPatchForConveyor.cs`](MyCubeGridPatchForConveyor.cs.md), [`MyCubeBlockPatchForConveyor.cs`](MyCubeBlockPatchForConveyor.cs.md), [`MyShipConnectorPatchForConveyor.cs`](MyShipConnectorPatchForConveyor.cs.md)) call `InvalidateCache` (clears entries but keeps the cache object) or `DropCache` (removes the cache object entirely from the dictionary) whenever a topology-changing event occurs. Turning the fix off at runtime causes `Update()` to clear and discard all cache objects so the game reverts to unpatched behaviour immediately.

This fix eliminates most of the lag when players enter or leave cockpits and cryopods, and reduces conveyor overhead while loading large production grids. See the *Cached MyGridConveyorSystem.Reachable* section of `Docs/PerformanceFixes.md` for the full rationale.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `ReachableCaches` | `RwLockDictionary<TLogicalGroup, UintCache<ulong>>` | One [`UintCache.cs`](../../Tools/UintCache.cs.md) per logical grid group; guarded by a [`RwLockDictionary.cs`](../../Tools/RwLockDictionary.cs.md) reader-writer lock. |
| `InvalidateCache(MyCubeGrid)` | Static method | Clears the cache for the logical group that contains `grid` (keeps the cache object). Called when topology changes but the group is stable. |
| `DropCache(MyCubeGrid)` | Static method | Removes the cache object for the group entirely. Called on grid split/merge, group membership changes, ownership change, and connector state change. |
| `Update()` | Static method | Called every game tick; drops all caches when `FixConveyor` is toggled off. |
| `ReachablePrefix` / `ReachablePostfix` | Harmony Prefix/Postfix | Cache-backed shortcut for `Reachable(IMyConveyorEndpoint, IMyConveyorEndpoint)`. |
| `ReachableByPlayerPrefix` | Harmony Prefix | Negative-result shortcut for `Reachable(IMyConveyorEndpointBlock, IMyConveyorEndpointBlock, long, MyDefinitionId, Predicate<>)`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyGridConveyorSystem.Reachable(IMyConveyorEndpoint, IMyConveyorEndpoint)` | Prefix | Returns cached result and skips original method on hit. |
| `MyGridConveyorSystem.Reachable(IMyConveyorEndpoint, IMyConveyorEndpoint)` | Postfix | Stores the original result in the cache after a miss. |
| `MyGridConveyorSystem.Reachable(IMyConveyorEndpointBlock, IMyConveyorEndpointBlock, long, MyDefinitionId, Predicate<IMyConveyorEndpoint>)` | Prefix | Short-circuits with `false` if the endpoint pair is cached as unreachable. |

## References

- [`UintCache.cs`](../../Tools/UintCache.cs.md) — the fixed-capacity cache primitive used per logical group
- [`RwLockDictionary.cs`](../../Tools/RwLockDictionary.cs.md) — thread-safe dictionary wrapper for the per-group cache map
- [`MyCubeGridPatchForConveyor.cs`](MyCubeGridPatchForConveyor.cs.md) — grid lifecycle invalidation hooks
- [`MyCubeBlockPatchForConveyor.cs`](MyCubeBlockPatchForConveyor.cs.md) — block functional-state invalidation hook
- [`MyShipConnectorPatchForConveyor.cs`](MyShipConnectorPatchForConveyor.cs.md) — connector lock/unlock invalidation hook
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
