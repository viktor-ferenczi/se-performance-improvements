# `Shared/Patches/Conveyor/MyShipConnectorPatchForConveyor.cs`

*Drops the conveyor reachability cache when a ship connector's connection state changes.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyShipConnectorPatchForConveyor.cs`](../../../../../Shared/Patches/Conveyor/MyShipConnectorPatchForConveyor.cs) (33 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch — cache invalidation |

## Purpose

Connector lock/unlock is one of the documented cache invalidation conditions: it changes which logical grid group two grids belong to. When `MyShipConnector.OnConnectionStateChanged` fires this patch calls `MyGridConveyorSystemPatch.DropCache` (full removal, not just clear) on the connector's grid, because the group topology itself has changed and the old cache object is no longer valid for the new group.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `OnConnectionStateChangedPostfix` | Harmony Postfix on `OnConnectionStateChanged` | Drops the cache for the connector's grid when the connector locks or unlocks. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyShipConnector.OnConnectionStateChanged` | Postfix | Calls `DropCache` on the connector's grid. |

## References

- [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) — cache owner; `DropCache` is called here
- [`MyCubeGridPatchForConveyor.cs`](MyCubeGridPatchForConveyor.cs.md) — sibling grid-level invalidation hooks
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
