# `Shared/Patches/Conveyor/PullItemStats.cs`

*Debug-only statistics helper that counts `PullItem` and `PullItems` calls dispatched through the conveyor system.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`PullItemStats.cs`](../../../../../Shared/Patches/Conveyor/PullItemStats.cs) (32 lines) |
| **Kind** | Class (conditionally compiled `#if DEBUG`) |
| **Role** | Debug instrumentation helper |

## Purpose

Used by the `#if DEBUG` blocks in [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) to accumulate call counts for `MyGridConveyorSystem.PullItem` and `PullItems`. `Report()` formats the current counts as a single-line string and resets both counters atomically, allowing periodic sampling without accumulation drift.

This helper has no effect in release builds and is not involved in the main reachability caching logic.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `PullItemCount` | `long` field | Running count of `PullItem` calls observed. |
| `PullItemsCount` | `long` field | Running count of `PullItems` calls observed. |
| `Report()` | Method | Returns a formatted count string and resets both counters. |

## References

- [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) — instantiates and calls this class in `#if DEBUG` blocks
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
