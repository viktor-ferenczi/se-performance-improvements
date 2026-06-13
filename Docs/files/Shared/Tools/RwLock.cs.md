# `Shared/Tools/RwLock.cs`

*Lightweight spin-based reader/writer lock implemented as static helpers over a shared `int` counter, plus a convenience wrapper class.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`RwLock.cs`](../../../../Shared/Tools/RwLock.cs) (81 lines) |
| **Kind** | Static utility class `RwLock` + class `RwLockCounter` |
| **Role** | Concurrency primitive |

## Purpose

The plugin's caches are accessed from both the main simulation thread and background worker threads. `RwLock` provides a minimal reader/writer lock that avoids the overhead of `ReaderWriterLockSlim` by using a single `int` counter and `Interlocked.CompareExchange` spin loops.

The protocol: the counter is `0` when idle, positive (count of active readers) when one or more readers hold the lock, and `-1` when a writer holds it. Readers spin until the counter is non-negative, then CAS it to `previous + 1`. The writer spins until the counter is `0`, then CAS it to `-1`. Both release paths are a single interlocked or volatile write.

`RwLock` exposes these operations as static methods taking a `ref int counter`, while `RwLockCounter` wraps the counter as an instance field for cases where embedding the counter in the owning object is inconvenient. [`RwLockDictionary.cs`](RwLockDictionary.cs.md) and [`RwLockHashSet.cs`](RwLockHashSet.cs.md) embed this same counter inline.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `AcquireForReading(ref int)` | Static method | Spin-waits until no writer holds the lock, then increments the reader count. |
| `ReleaseAfterReading(ref int)` | Static method | Decrements the reader count. |
| `AcquireForWriting(ref int)` | Static method | Spin-waits until the counter is `0`, then sets it to `-1`. |
| `ReleaseAfterWriting(ref int)` | Static method | Resets the counter to `0`. |
| `RwLockCounter` | Class | Object-oriented wrapper holding its own `int counter`; exposes the same four methods as instance methods. |

## References

- [`RwLockDictionary.cs`](RwLockDictionary.cs.md) — `Dictionary<TK,TV>` subclass with embedded `RwLock` operations.
- [`RwLockHashSet.cs`](RwLockHashSet.cs.md) — `HashSet<T>` subclass with embedded `RwLock` operations.
- [`Cache.cs`](Cache.cs.md) — uses `RwLockDictionary` for thread-safe cache access.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
