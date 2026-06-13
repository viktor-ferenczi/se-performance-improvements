# `Shared/Tools/Cache.cs`

*Generic time-bounded key/value cache protected by a reader/writer lock.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`Cache.cs`](../../../../Shared/Tools/Cache.cs) (147 lines) |
| **Kind** | Generic class `Cache<TK, TV>` |
| **Role** | Cache primitive |

## Purpose

`Cache<TK, TV>` is the primary time-bounded cache used throughout the plugin. Each entry carries an expiry tick and is evicted lazily during `Cleanup()`, which runs only once per `cleanupPeriod` ticks to keep overhead minimal. Concurrent access is guarded by [`RwLockDictionary.cs`](RwLockDictionary.cs.md), so the same cache instance can be read safely from worker threads while the main thread occasionally writes.

The design avoids heap allocations during eviction by pre-allocating a `keysToDelete` array at construction time. In `DEBUG` builds the cache exposes a [`CacheStat.cs`](CacheStat.cs.md) instance so callers can report hit-rate percentages to the log.

This primitive is the backbone of most per-entity caches in the plugin (safe-zone checks, conveyor reachability, wind-turbine atmosphere checks, etc.), all of which follow the pattern: call `Cleanup()` every simulation tick, then `TryGetValue` / `Store` per entity.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Cache(cleanupPeriod, maxDeleteCount)` | Constructor | Sets the eviction period (in ticks) and the maximum number of expired entries removed per cleanup pass. |
| `Cleanup()` | Method | Advances the internal clock; performs an eviction pass only when `nextCleanup` is reached. Must be called every simulation tick. |
| `Store(key, value, lifetime)` | Method | Inserts or replaces a cache entry with a tick-based expiry of `tick + lifetime`. |
| `TryGetValue(key, out value)` | Method | Returns `true` and the cached value if the entry exists and has not expired. |
| `Forget(key)` | Method | Removes a single entry immediately (e.g. on invalidation). |
| `Clear()` | Method | Evicts all entries at once (e.g. on grid group rebuild). |
| `Extend(key, lifetime)` | Method | Resets the expiry of an existing entry without changing its value. |
| `Stat` *(DEBUG only)* | Field | [`CacheStat.cs`](CacheStat.cs.md) accumulating hit/miss counts for diagnostic reporting. |

## References

- [`CacheStat.cs`](CacheStat.cs.md) — hit-rate statistics collected in DEBUG builds.
- [`RwLockDictionary.cs`](RwLockDictionary.cs.md) — thread-safe backing store.
- [`TwoLayerCache.cs`](TwoLayerCache.cs.md) — lock-free read-path variant for high-frequency lookups.
- [`UintCache.cs`](UintCache.cs.md) — specialised variant that packs a `uint` value and expiry into a single `ulong`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
