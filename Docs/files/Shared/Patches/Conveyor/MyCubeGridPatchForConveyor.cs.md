# `Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs`

*Invalidates or drops the conveyor reachability cache in response to grid lifecycle and topology events.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyCubeGridPatchForConveyor.cs`](../../../../../Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs) (183 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch — cache invalidation |

## Purpose

This class hooks `MyCubeGrid` at every point where the cached conveyor-reachability data kept by [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) could become stale. Its sole responsibility is deciding whether to call `InvalidateCache` (clear entries, keep object) or `DropCache` (remove the cache object entirely) and then delegating to [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md).

The rule is: use `InvalidateCache` when the grid's logical group identity is preserved and only the reachability graph changes (e.g. a block with conveyor ports is added or removed, the grid enters or leaves the scene, group membership notification arrives). Use `DropCache` when the group itself may change or become invalid (grid split, merge, mechanical connection change, scene removal, ownership change, grid deletion).

All patches are Postfixes so they fire after the game's own logic has completed, ensuring that the logical group has already been updated before the cache is touched.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `AddCubeBlockPostfix` | Harmony Postfix on `AddCubeBlock` | Invalidates cache when a block with conveyor ports is added. |
| `RemoveBlockPostfix` | Harmony Postfix on `RemoveBlock` | Invalidates cache when a block with conveyor ports is removed. |
| `CreateSplitPostfix` | Harmony Postfix on `CreateSplit` | Drops cache on grid split. |
| `MergeGridInternalPostfix` | Harmony Postfix on `MergeGridInternal` | Drops cache on both the surviving and merging grid. |
| `MechanicalConnectionBlockAttachUpdateStatusChangedPostfix` | Harmony Postfix on `MechanicalConnectionBlockAttachUpdateStatusChanged` | Drops cache on rotor/hinge/piston attach-state change. |
| `OnAddedToScenePostfix` / `OnRemovedFromScenePostfix` | Harmony Postfix | Invalidates / drops cache as grid enters or leaves the scene. |
| `OnAddedToGroupPostfix` / `OnRemovedFromGroupPostfix` | Harmony Postfix on group events | Invalidates cache when logical group membership changes. |
| `NotifyBlockAddedPostfix` / `NotifyBlockRemovedPostfix` | Harmony Postfix | Invalidates cache on block notification (conveyor blocks only). |
| `NotifyBlockOwnershipChangePostfix` | Harmony Postfix | Invalidates cache on any ownership change (block not passed; conservative). |
| `BeforeDeletePostfix` | Harmony Postfix on `BeforeDelete` | Drops cache before grid is deleted. |
| `ChangeGridOwnershipPostfix` | Harmony Postfix on `ChangeGridOwnership` | Drops cache when full-grid ownership changes. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyCubeGrid.AddCubeBlock` | Postfix | `InvalidateCache` if added block is a conveyor endpoint. |
| `MyCubeGrid.RemoveBlock` | Postfix | `InvalidateCache` if removed block is a conveyor endpoint. |
| `MyCubeGrid.CreateSplit` | Postfix | `DropCache` for the split grid. |
| `MyCubeGrid.MergeGridInternal` | Postfix | `DropCache` for both grids involved. |
| `MyCubeGrid.MechanicalConnectionBlockAttachUpdateStatusChanged` | Postfix | `DropCache` for grid and top-grid. |
| `MyCubeGrid.OnAddedToScene` | Postfix | `InvalidateCache` when grid enters scene. |
| `MyCubeGrid.OnRemovedFromScene` | Postfix | `DropCache` when grid leaves scene. |
| `MyCubeGrid.OnAddedToGroup(MyGridLogicalGroupData)` | Postfix | `InvalidateCache` on group join. |
| `MyCubeGrid.OnRemovedFromGroup(MyGridLogicalGroupData)` | Postfix | `InvalidateCache` on group leave. |
| `MyCubeGrid.NotifyBlockAdded` | Postfix | `InvalidateCache` if block is a conveyor endpoint. |
| `MyCubeGrid.NotifyBlockRemoved` | Postfix | `InvalidateCache` if block is a conveyor endpoint. |
| `MyCubeGrid.NotifyBlockOwnershipChange` | Postfix | `InvalidateCache` (conservative; block not passed). |
| `MyCubeGrid.BeforeDelete` | Postfix | `DropCache` before deletion. |
| `MyCubeGrid.ChangeGridOwnership` | Postfix | `DropCache` on full-grid ownership change. |

## References

- [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) — cache owner; `InvalidateCache` and `DropCache` are called here
- [`MyCubeBlockPatchForConveyor.cs`](MyCubeBlockPatchForConveyor.cs.md) — sibling invalidation hook for block functional-state changes
- [`MyShipConnectorPatchForConveyor.cs`](MyShipConnectorPatchForConveyor.cs.md) — sibling invalidation hook for connector lock/unlock
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
