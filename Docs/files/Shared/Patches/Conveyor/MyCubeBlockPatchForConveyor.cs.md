# `Shared/Patches/Conveyor/MyCubeBlockPatchForConveyor.cs`

*Invalidates the conveyor reachability cache when a conveyor-endpoint block changes its functional state.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyCubeBlockPatchForConveyor.cs`](../../../../../Shared/Patches/Conveyor/MyCubeBlockPatchForConveyor.cs) (34 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch — cache invalidation |

## Purpose

A block that implements `IMyConveyorEndpointBlock` participates in the conveyor graph. If its functional state changes (e.g. it is damaged, powered down, or repaired) the reachability through that endpoint may change. This patch hooks `MyCubeBlock.ComponentStack_IsFunctionalChanged` and calls `MyGridConveyorSystemPatch.InvalidateCache` on the block's grid so cached results are discarded.

The guard `__instance is IMyConveyorEndpointBlock` avoids unnecessary invalidations for the vast majority of blocks that have no conveyor ports.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `ComponentStack_IsFunctionalChangePostfix` | Harmony Postfix on `ComponentStack_IsFunctionalChanged` | Invalidates the grid's reachability cache when a conveyor-endpoint block changes functional state. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyCubeBlock.ComponentStack_IsFunctionalChanged` | Postfix | Calls `InvalidateCache` on the block's grid if the block is a conveyor endpoint. |

## References

- [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) — cache owner
- [`MyCubeGridPatchForConveyor.cs`](MyCubeGridPatchForConveyor.cs.md) — sibling invalidation hooks for grid-level events
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
