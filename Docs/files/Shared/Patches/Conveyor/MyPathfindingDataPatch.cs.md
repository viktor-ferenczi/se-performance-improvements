# `Shared/Patches/Conveyor/MyPathfindingDataPatch.cs`

*Disabled experimental patch that replaced `MyPathfindingData.Timestamp` backing storage with a `ThreadLocal<long>` to avoid lock contention.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyPathfindingDataPatch.cs`](../../../../../Shared/Patches/Conveyor/MyPathfindingDataPatch.cs) (54 lines) |
| **Kind** | Static Harmony patch class (conditionally compiled `#if DISABLED`) |
| **Role** | Experimental performance patch (not active) |

## Purpose

`MyPathfindingData.Timestamp` is used as a visited-node marker during BFS traversal of the conveyor graph. The original implementation uses a shared lock object for synchronisation, which can become a contention point when multiple threads run pathfinding concurrently.

This experimental patch replaces the lock object allocated in the constructor with a `ThreadLocal<long>` and redirects the `Timestamp` getter and setter to read/write the thread-local value. Benchmarking showed no measurable performance improvement over the original, so the patch is permanently disabled with `#if DISABLED`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `TimestampCtorPostfix` | Harmony Postfix on `MyPathfindingData..ctor(object)` | Replaces `m_lockObject` with a `ThreadLocal<long>`. |
| `TimestampSetterPrefix` | Harmony Prefix on `Timestamp` setter | Writes the value into the thread-local storage; skips original. |
| `TimestampGetterPrefix` | Harmony Prefix on `Timestamp` getter | Reads the value from thread-local storage; skips original. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyPathfindingData..ctor(object)` | Postfix | Replaces the lock object with `ThreadLocal<long>` (compiled only under `!DISABLED`). |
| `MyPathfindingData.Timestamp` (setter) | Prefix | Writes via thread-local; skips original (compiled only under `!DISABLED`). |
| `MyPathfindingData.Timestamp` (getter) | Prefix | Reads via thread-local; skips original (compiled only under `!DISABLED`). |

## References

- [`MyPathFindingSystemPatch.cs`](MyPathFindingSystemPatch.cs.md) — companion traversal instrumentation patch
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
