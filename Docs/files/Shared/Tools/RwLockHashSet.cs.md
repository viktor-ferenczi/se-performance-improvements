# `Shared/Tools/RwLockHashSet.cs`

*`HashSet<T>` subclass with embedded spin-based reader/writer locking, used for thread-safe set membership tracking.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`RwLockHashSet.cs`](../../../../Shared/Tools/RwLockHashSet.cs) (58 lines) |
| **Kind** | Generic class `RwLockHashSet<T> : HashSet<T>` |
| **Role** | Concurrency primitive |

## Purpose

`RwLockHashSet<T>` is the set counterpart to [`RwLockDictionary.cs`](RwLockDictionary.cs.md). It inherits from `HashSet<T>` and adds the same four `BeginReading` / `FinishReading` / `BeginWriting` / `FinishWriting` methods, delegating to [`RwLock.cs`](RwLock.cs.md) using an inline `int counter`. The lock counter is embedded in the `HashSet` instance for cache-locality.

It is used in patches that need to track a set of active entities (for example, grids currently undergoing a merge or paste operation) from multiple threads simultaneously.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `BeginReading()` / `FinishReading()` | Methods | Acquire/release a shared (read) lock via [`RwLock.cs`](RwLock.cs.md). |
| `BeginWriting()` / `FinishWriting()` | Methods | Acquire/release the exclusive (write) lock via [`RwLock.cs`](RwLock.cs.md). |

## References

- [`RwLock.cs`](RwLock.cs.md) — provides the spin-wait lock primitives.
- [`RwLockDictionary.cs`](RwLockDictionary.cs.md) — analogous wrapper for `Dictionary<TKey, TValue>`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
