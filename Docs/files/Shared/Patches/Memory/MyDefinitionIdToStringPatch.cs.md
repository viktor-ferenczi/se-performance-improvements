# `Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs`

*Caches `MyDefinitionId.ToString()` results in a two-layer cache to eliminate repeated string allocations from a hot path.*

|  |  |
| --- | --- |
| **Module** | [Memory Allocation Patches](../../../../modules/memory-allocation.md) |
| **Source** | [`MyDefinitionIdToStringPatch.cs`](../../../../../Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs) (117 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyDefinitionId.ToString()` is called very frequently throughout the game and allocates a new string on every call. Keen attempted to address this in update 1.202.066 (Automaton) but their implementation introduced a deadlock on world load. This patch re-applies the caching fix independently.

The Prefix completely replaces `ToString`: it looks up the result by the definition's `GetHashCodeLong()` in a [`TwoLayerCache.cs`](../../Tools/TwoLayerCache.cs.md), returning the cached string if found, or calling the local `Format` helper, storing the result, and returning it if not. The original method body is never called.

The cache has two layers: a fast immutable snapshot promoted from a mutable layer every `FillPeriod` ticks (~17 seconds). `Update()` is called each tick and triggers `FillImmutableCache()` on schedule. The `enabled` flag controls only cache clearing on disable; the Prefix itself runs unconditionally (the comment in source notes the original is unlikely to change and always bypassed).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `TwoLayerCache` | `TwoLayerCache<long, string>` | Two-layer cache keyed on `GetHashCodeLong()`; immutable snapshot refreshed every ~17 s. |
| `FillPeriod` | `const long` | 17 × 60 ticks between immutable-layer promotions. |
| `Configure()` / `OnConfigChanged()` | Static methods | Set `enabled` from `Config.FixMemory`; clear cache on disable. |
| `Update()` | Static method | Triggers `FillImmutableCache()` on the configured schedule. |
| `ToStringPrefix` | Harmony Prefix | Fully replaces `MyDefinitionId.ToString` — serves from cache or formats and stores. |
| `Format(MyDefinitionId)` | Static method | Formats `TypeId/SubtypeName` with `(null)` substitution for missing parts. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyDefinitionId.ToString()` | Prefix | Completely replaces the method; serves results from a two-layer string cache, eliminating per-call allocations. |

## References

- [memory-allocation](../../../../modules/memory-allocation.md)
- [`TwoLayerCache.cs`](../../Tools/TwoLayerCache.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Memory Allocation Patches](../../../../modules/memory-allocation.md) · [Index](../../../../Index.md)*
