# `Shared/Tools/TwoLayerCache.cs`

*Two-layer cache that combines a lock-free immutable read path with a synchronized mutable write path for minimal read overhead.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`TwoLayerCache.cs`](../../../../Shared/Tools/TwoLayerCache.cs) (109 lines) |
| **Kind** | Generic class `TwoLayerCache<TK, TV>` |
| **Role** | Cache primitive |

## Purpose

`TwoLayerCache<TK, TV>` is optimised for workloads where the same set of keys is looked up repeatedly many times before the set changes. It maintains two internal stores:

1. **Immutable layer** — a plain `IReadOnlyDictionary<TK, TV>` (swapped in atomically via a reference assignment). Reads require no locking at all, so the common hit path is a single dictionary lookup with no synchronisation cost.
2. **Mutable layer** — a [`RwLockDictionary.cs`](RwLockDictionary.cs.md) that accumulates newly stored entries. Reads fall through to this layer only on an immutable miss.

`FillImmutableCache()` promotes the mutable layer into a new immutable snapshot when the two counts differ; it must be called periodically (e.g. once per tick) by the owning system. This approach is ideal for lookup tables whose contents stabilise quickly and then change only on invalidation events (such as grid-group rebuilds for the conveyor reachability fix).

In `DEBUG` builds with `COLLECT_STATS` defined, separate [`CacheStat.cs`](CacheStat.cs.md) instances track hit rates for each layer independently.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Store(key, value)` | Method | Writes the entry to the mutable [`RwLockDictionary.cs`](RwLockDictionary.cs.md) layer. |
| `Forget(key)` | Method | Removes the entry from the mutable layer (does not remove from the immutable snapshot). |
| `TryGetValue(key, out value)` | Method | Checks the immutable layer first (lock-free), then falls through to the mutable layer if not found. |
| `FillImmutableCache()` | Method | Replaces the immutable snapshot with a new copy of the mutable layer; no-op if counts already match. |
| `Clear()` | Method | Clears both layers atomically (acquires the write lock). |

## References

- [`Cache.cs`](Cache.cs.md) — tick-expiring alternative suitable for time-bounded entries.
- [`RwLockDictionary.cs`](RwLockDictionary.cs.md) — backing store for the mutable layer.
- [`CacheStat.cs`](CacheStat.cs.md) — optional per-layer statistics in DEBUG builds.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
