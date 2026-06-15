# `Shared/Patches/Block/MyCubeBlockPatch.cs`

*Caches the result of `MyCubeBlock.GetUserRelationToOwner` per entity/identity pair to avoid repeated ownership lookups (file is currently compiled out via `#if UNTESTED`).*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyCubeBlockPatch.cs`](../../../../../Shared/Patches/Block/MyCubeBlockPatch.cs) (88 lines) |
| **Kind** | Static Harmony patch class (currently disabled — file wrapped in `#if UNTESTED`) |
| **Role** | Performance patch |

## Purpose

`MyCubeBlock.GetUserRelationToOwner` is called frequently to check whether a player has rights over a block. The relationship between a player and a block's owner changes rarely (only on ownership transfer), so the result can safely be cached for a short period.

A Prefix/Postfix pair on `GetUserRelationToOwner` implements a read-through cache using [`UintCache.cs`](../../Tools/UintCache.cs.md). The cache key is `EntityId ^ identityId ^ (long)defaultNoUser`. To detect collisions (the key space is intentionally compact) the stored value is XOR-encoded with the lower 32 bits of `identityId`, and any decoded value outside the valid enum range falls through to the original method. Entries expire in approximately 15 seconds (15 × 60 + jitter ticks). The cache is cleared whenever the `FixAccess` config flag is toggled off.

This fix is off by default on the server as noted in the "Less frequent update of block access rights" section of `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Cache` | `UintCache<long>` | Stores encoded relation values keyed by the XOR cache key; capacity 2048, TTL ~15 s. |
| `GetUserRelationToOwnerPrefix` | Prefix | Returns cached result early; sets `__state` to the key so the postfix knows to store it. |
| `GetUserRelationToOwnerPostfix` | Postfix | Stores the freshly computed result into the cache when `__state` is non-zero. |
| `Configure()` / `OnConfigChanged` | Static methods | Gate the patch on `Config.FixAccess` and clear the cache on disable. |
| `Update()` | Static method | Advances cache TTL cleanup each simulation tick. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyCubeBlock.GetUserRelationToOwner` | Prefix | Returns cached relation, skipping the original method on a cache hit. |
| `MyCubeBlock.GetUserRelationToOwner` | Postfix | Stores the computed relation into the cache for future hits. |

## References

- [`MyTerminalBlockPatch.cs`](MyTerminalBlockPatch.cs.md) — companion patch caching `HasPlayerAccessReason` on the same config flag
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
