# `Shared/Patches/TargetingSystem/MyGridTargetingPatch.cs`

*Makes the per-frame refresh of a turret grid's target groups linear in the number of grids in range instead of quadratic.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyGridTargetingPatch.cs`](../../../../../Shared/Patches/TargetingSystem/MyGridTargetingPatch.cs) (104 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

A grid with turrets refreshes its list of target groups once per frame in `MyGridTargeting.RefreshGridConnections`: every top-most entity in the turrets' range, grouped by physical connection. The game pops entities off the query result and, for every grid, removes each physically connected grid from that list with `List.Remove`, a linear search, so N grids in range cost N x N comparisons per turret grid per frame. In the "Many Lifters Slowness" test world (600 grids in range) that was 2% of the main thread per turret grid.

The Prefix replaces the method with the same grouping over a `HashSet` for the "not yet grouped" test, which makes it linear. It walks the query result in the same order (from the last entity backwards), asks `GetConnectedGrids` for the same groups, fills the same pooled lists and empties the shared query buffer the way the game does, so the result is identical (verified group by group against the game's own algorithm with a probe over thousands of refreshes). Gated by `FixTargeting`.

See the *Reducing memory allocations in the turret targeting system* section of `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `remaining` | `[ThreadStatic] HashSet<MyEntity>` | The grids not yet grouped, reused across calls so the refresh allocates nothing beyond what the game allocates. |
| `RefreshGridConnectionsPrefix` | Prefix | The linear regrouping; skips the original. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyGridTargeting.RefreshGridConnections` | Prefix | Replaces the quadratic regrouping with a linear one producing the same groups. |

## References

- [`MyLargeTurretTargetingSystemPatch.cs`](MyLargeTurretTargetingSystemPatch.cs.md) — the (disabled) allocation fix of the same subsystem, under the same option
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
