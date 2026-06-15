# `Shared/Tools/UintCache.cs`

*Specialised time-bounded cache that packs a `uint` value and a tick-based expiry into a single `ulong`, eliminating per-entry object allocation.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`UintCache.cs`](../../../../Shared/Tools/UintCache.cs) (128 lines) |
| **Kind** | Generic class `UintCache<TK>` |
| **Role** | Cache primitive |

## Purpose

`UintCache<TK>` is a memory-compact variant of [`Cache.cs`](Cache.cs.md) for cases where the cached value fits in 32 bits. Instead of boxing the value in a heap-allocated `Item` object, it packs both the expiry tick and the value into a single `ulong` stored directly in the backing [`RwLockDictionary.cs`](RwLockDictionary.cs.md): the upper 32 bits hold the tick at which the entry expires, the lower 32 bits hold the `uint` value.

This eliminates one heap allocation per cache entry, which matters for high-cardinality caches (e.g. safe-zone check results for many small grids). The tick is also stored in `ulong` format (shifted left 32 bits) so the expiry comparison is a single unsigned comparison with no masking.

Otherwise the API mirrors [`Cache.cs`](Cache.cs.md): `Cleanup()` must be called every tick, `Store` / `TryGetValue` / `Forget` / `Extend` / `Clear` work the same way.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `UintCache(cleanupPeriod, maxDeleteCount)` | Constructor | Initialises cleanup cadence and the pre-allocated eviction key array. |
| `Cleanup()` | Method | Advances the clock; performs lazy eviction when `nextCleanup` is reached. Returns `true` if any entries were removed. |
| `Store(key, value, lifetime)` | Method | Packs `value` and expiry into a `ulong` and inserts it into the backing dictionary. |
| `TryGetValue(key, out uint)` | Method | Returns the `uint` value if the entry exists and the expiry tick has not passed. |
| `Extend(key, lifetime)` | Method | Updates the expiry without changing the stored value. |
| `Forget(key)` / `Clear()` | Methods | Remove one entry or all entries (write-locked). |

## References

- [`Cache.cs`](Cache.cs.md) — general-purpose variant using a generic value type with heap-allocated `Item` wrappers.
- [`RwLockDictionary.cs`](RwLockDictionary.cs.md) — thread-safe backing store.
- [`CacheStat.cs`](CacheStat.cs.md) — hit-rate statistics available in DEBUG builds.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
