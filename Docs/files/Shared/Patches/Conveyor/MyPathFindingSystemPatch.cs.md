# `Shared/Patches/Conveyor/MyPathFindingSystemPatch.cs`

*Disabled debug-only patch that counted `MyPathFindingSystem<IMyConveyorEndpoint>.Reachable` call and failure rates via [`ConveyorStat.cs`](../../Tools/ConveyorStat.cs.md).*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyPathFindingSystemPatch.cs`](../../../../../Shared/Patches/Conveyor/MyPathFindingSystemPatch.cs) (51 lines) |
| **Kind** | Static Harmony patch class (conditionally compiled `#if DEBUG` + `#if DISABLED`) |
| **Role** | Debug instrumentation (not active) |

## Purpose

This patch was written during investigation of conveyor pathfinding costs. It instruments `MyPathFindingSystem<IMyConveyorEndpoint>.Reachable` via a Prefix and Postfix pair to count total calls and failure (unreachable) outcomes, reporting through a [`ConveyorStat.cs`](../../Tools/ConveyorStat.cs.md) helper.

Patching generic types with Harmony no longer works in the current version of the game, so the inner `#if DISABLED` guard permanently excludes the code. The file is retained as a reference for the statistics-gathering approach.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Stat` | `ConveyorStat` | Accumulates call and failure counts; exposed via `Report(int period)`. |
| `ReachablePrefix` | Harmony Prefix | Increments the total call counter. |
| `ReachablePostfix` | Harmony Postfix | Increments the failure counter when result is `false`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyPathFindingSystem<IMyConveyorEndpoint>.Reachable` | Prefix | Count call (compiled only under `#if DEBUG && !DISABLED`). |
| `MyPathFindingSystem<IMyConveyorEndpoint>.Reachable` | Postfix | Count unreachable result (compiled only under `#if DEBUG && !DISABLED`). |

## References

- [`ConveyorStat.cs`](../../Tools/ConveyorStat.cs.md) — statistics accumulator used here
- [`MyPathFindingSystemEnumeratorPatch.cs`](MyPathFindingSystemEnumeratorPatch.cs.md) — companion enumerator-level stat patch
- [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) — the active reachability cache this was designed to support
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
