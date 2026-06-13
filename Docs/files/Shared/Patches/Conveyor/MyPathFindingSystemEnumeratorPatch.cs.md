# `Shared/Patches/Conveyor/MyPathFindingSystemEnumeratorPatch.cs`

*Disabled debug-only patch that counted individual graph-traversal steps in `MyPathFindingSystem<IMyConveyorEndpoint>.Enumerator.MoveNext`.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyPathFindingSystemEnumeratorPatch.cs`](../../../../../Shared/Patches/Conveyor/MyPathFindingSystemEnumeratorPatch.cs) (41 lines) |
| **Kind** | Static Harmony patch class (conditionally compiled `#if DEBUG` + `#if DISABLED`) |
| **Role** | Debug instrumentation (not active) |

## Purpose

Companion to [`MyPathFindingSystemPatch.cs`](MyPathFindingSystemPatch.cs.md), this patch hooks the inner enumerator's `MoveNext` to count each graph-edge expansion during a BFS/DFS traversal. Combined with the call counter from [`MyPathFindingSystemPatch.cs`](MyPathFindingSystemPatch.cs.md) this reveals the average path length and the total traversal cost per reachability query.

Like [`MyPathFindingSystemPatch.cs`](MyPathFindingSystemPatch.cs.md), it is gated behind `#if DISABLED` because patching generic type nested classes no longer works reliably with Harmony.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Stat` | `ConveyorStat` | Accumulates `MoveNext` call counts; exposed via `Report(int period)`. |
| `MoveNextPrefix` | Harmony Prefix on `Enumerator.MoveNext` | Increments the traversal-step counter. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyPathFindingSystem<IMyConveyorEndpoint>.Enumerator.MoveNext` | Prefix | Count each traversal step (compiled only under `#if DEBUG && !DISABLED`). |

## References

- [`ConveyorStat.cs`](../../Tools/ConveyorStat.cs.md) — statistics accumulator used here
- [`MyPathFindingSystemPatch.cs`](MyPathFindingSystemPatch.cs.md) — companion call-level stat patch
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
