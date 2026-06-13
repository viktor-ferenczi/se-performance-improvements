# `Shared/Patches/Projection/MyEntityPatchForProjection.cs`

*Disables functional blocks on physics-less (projected) grids the moment they are added to the scene, preventing wasteful updates and projection-era bugs such as projected welders welding in creative mode.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyEntityPatchForProjection.cs`](../../../../../Shared/Patches/Projection/MyEntityPatchForProjection.cs) (42 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

Projected grids have no physics body (`CubeGrid.Physics == null`). Despite this, all functional blocks on the projection are added to the scene and updated every tick, which wastes CPU. Worse, some of them can actually function (e.g., projected welders weld in creative mode) due to game bugs.

The Postfix on `MyEntity.OnAddedToScene` checks whether the newly added entity is a `MyFunctionalBlock` on a physics-less grid and, if so, sets `Enabled = false` immediately. This happens exactly once, at add-time, so there is no per-tick overhead. The check cannot use `functionalBlock.CubeGrid?.Projector` because the projector back-reference is set only after the preview grid has been added to the scene.

The fix has a visual side-effect: all functional blocks appear disabled in the projection preview (see the "Disabled functional blocks in projected grids" section in `Docs/PerformanceFixes.md`). It is controlled by `Config.FixProjection`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `OnAddedToScenePostfix` | Postfix | Disables any `MyFunctionalBlock` whose grid has no physics at the moment it is added to the scene. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyEntity.OnAddedToScene` | Postfix | Disables functional blocks on physics-less (projected) grids on first scene add. |

## References

- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
