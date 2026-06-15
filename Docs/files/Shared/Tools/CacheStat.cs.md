# `Shared/Tools/CacheStat.cs`

*Lightweight, non-thread-safe accumulator for cache hit-rate statistics used in DEBUG builds.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`CacheStat.cs`](../../../../Shared/Tools/CacheStat.cs) (82 lines) |
| **Kind** | Class `CacheStat` |
| **Role** | Diagnostics helper |

## Purpose

`CacheStat` tracks lookup and hit counts together with the current item count for a single cache instance. It is intentionally not thread-safe — the comment notes that approximate results are acceptable here, and the overhead of synchronisation would undermine its purpose as a zero-cost-in-Release diagnostic.

In `DEBUG` builds each [`Cache.cs`](Cache.cs.md) and [`TwoLayerCache.cs`](TwoLayerCache.cs.md) instance holds a `CacheStat`; `CountLookup` and `CountHit` are called on every `TryGetValue`. The `Report` property formats and resets the counters in one step, making it easy to emit periodic log messages without double-counting.

`AddRange` allows an owner to aggregate statistics from multiple per-entity cache instances into a single report line.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `CountLookup(size)` | Method | Records one lookup and updates `Size` to the current item count. |
| `CountHit()` | Method | Records a cache hit (called after `CountLookup` on a successful retrieval). |
| `AddRange(stats)` | Method | Merges a collection of `CacheStat` instances into this one, resetting each source. |
| `Report` | Property | Formats `HitRate = …% = hits/lookups; ItemCount = …`, then resets all counters. |
| `Reset(size)` / `Clear()` | Methods | Zero the counters; `Reset` also sets `Size` explicitly. |

## References

- [`Cache.cs`](Cache.cs.md) — primary consumer of `CacheStat`.
- [`TwoLayerCache.cs`](TwoLayerCache.cs.md) — holds two `CacheStat` instances (immutable layer + mutable layer).
- [`ConveyorStat.cs`](ConveyorStat.cs.md) — analogous statistics class for the conveyor reachability cache.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
