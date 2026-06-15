# `Shared/Patches/Block/MyTerminalBlockPatch.cs`

*Caches the result of `MyTerminalBlock.HasPlayerAccessReason` per entity/identity pair to avoid repeated terminal-access checks (file is currently compiled out via `#if UNTESTED`).*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyTerminalBlockPatch.cs`](../../../../../Shared/Patches/Block/MyTerminalBlockPatch.cs) (89 lines) |
| **Kind** | Static Harmony patch class (currently disabled — file wrapped in `#if UNTESTED`) |
| **Role** | Performance patch |

## Purpose

`MyTerminalBlock.HasPlayerAccessReason` is called frequently to determine whether a player may interact with a terminal block. The underlying access relation changes only on ownership or faction changes, so short-term caching is safe.

The implementation mirrors [`MyCubeBlockPatch.cs`](MyCubeBlockPatch.cs.md): a Prefix checks a [`UintCache.cs`](../../Tools/UintCache.cs.md) keyed by `EntityId ^ identityId ^ (long)defaultNoUser` and returns early on hit; a Postfix stores the result when a cache miss was detected (`__state != 0`). The encoded value is XOR'd with the lower 32 bits of `identityId` to cheaply guard against hash collisions — any decoded value outside the valid `AccessRightsResult` enum range falls through to the original. Entries expire in approximately 12 seconds (12 × 60 + jitter ticks). Governed by `Config.FixAccess`; the cache is cleared when the flag is toggled off.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Cache` | `UintCache<long>` | Stores encoded `AccessRightsResult` values; capacity 2048, TTL ~12 s. |
| `HasPlayerAccessReasonPrefix` | Prefix | Returns cached access result early; sets `__state` to the cache key on a miss. |
| `HasPlayerAccessReasonPostfix` | Postfix | Stores the freshly computed result when `__state != 0`. |
| `Configure()` / `OnConfigChanged` | Static methods | Gate the patch on `Config.FixAccess` and clear the cache on disable. |
| `Update()` | Static method | Advances cache TTL cleanup each simulation tick. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyTerminalBlock.HasPlayerAccessReason` | Prefix | Returns cached access result, skipping the original method on a cache hit. |
| `MyTerminalBlock.HasPlayerAccessReason` | Postfix | Stores the computed access result into the cache for future hits. |

## References

- [`MyCubeBlockPatch.cs`](MyCubeBlockPatch.cs.md) — companion patch caching `GetUserRelationToOwner` on the same config flag
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
