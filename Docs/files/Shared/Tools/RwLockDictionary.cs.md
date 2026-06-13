# `Shared/Tools/RwLockDictionary.cs`

*`Dictionary<TK, TV>` subclass with embedded spin-based reader/writer locking, used as the backing store for all plugin caches.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`RwLockDictionary.cs`](../../../../Shared/Tools/RwLockDictionary.cs) (58 lines) |
| **Kind** | Generic class `RwLockDictionary<TKey, TValue> : Dictionary<TKey, TValue>` |
| **Role** | Concurrency primitive / Cache backing store |

## Purpose

`RwLockDictionary<TKey, TValue>` inherits from the standard `Dictionary` and adds four inline methods — `BeginReading`, `FinishReading`, `BeginWriting`, `FinishWriting` — that delegate to [`RwLock.cs`](RwLock.cs.md) with the dictionary's own embedded `int counter`. This keeps the lock counter co-located with the dictionary payload, improving cache locality compared to a separate lock object.

Callers must bracket dictionary operations with the matching `Begin`/`Finish` pair. All [`Cache.cs`](Cache.cs.md), [`TwoLayerCache.cs`](TwoLayerCache.cs.md), and [`UintCache.cs`](UintCache.cs.md) instances use this type as their backing store.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `BeginReading()` / `FinishReading()` | Methods | Acquire/release a shared (read) lock via [`RwLock.cs`](RwLock.cs.md). |
| `BeginWriting()` / `FinishWriting()` | Methods | Acquire/release the exclusive (write) lock via [`RwLock.cs`](RwLock.cs.md). |

## References

- [`RwLock.cs`](RwLock.cs.md) — provides the actual spin-wait lock primitives.
- [`RwLockHashSet.cs`](RwLockHashSet.cs.md) — analogous wrapper for `HashSet<T>`.
- [`Cache.cs`](Cache.cs.md), [`TwoLayerCache.cs`](TwoLayerCache.cs.md), [`UintCache.cs`](UintCache.cs.md) — consumers of `RwLockDictionary`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
