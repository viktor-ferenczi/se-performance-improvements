# `Shared/Patches/WindTurbine/MyWindTurbinePatch.cs`

*Caches the result of the `MyWindTurbine.IsInAtmosphere` property getter per turbine entity for approximately 30 seconds to avoid repeated atmosphere checks.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyWindTurbinePatch.cs`](../../../../../Shared/Patches/WindTurbine/MyWindTurbinePatch.cs) (83 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyWindTurbine.IsInAtmosphere` is evaluated on every turbine update tick to determine whether the turbine is inside a planet's atmosphere. The result changes only when the turbine moves in or out of an atmosphere, which is rare compared to the update rate.

A Prefix/Postfix pair caches the boolean result in a [`UintCache.cs`](../../Tools/UintCache.cs.md) keyed by `EntityId` (0 = false, 1 = true). On a cache hit the Prefix sets `__result` and returns `false` to skip the original; on a miss it sets `__state = true` so the Postfix knows to store the freshly computed value. Entries expire after approximately 30 seconds (30 × 60 + jitter ticks), where the jitter is derived from the entity ID to spread expiry across multiple turbines. The cache is cleared when `Config.FixWindTurbine` is toggled off.

See the "Caching the result of wind turbine atmosphere checks" section in `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Cache` | `UintCache<long>` | Stores per-turbine atmosphere check results; TTL ~30 s with per-entity jitter. |
| `IsInAtmosphereGetterPrefix` | Prefix | Returns cached value and skips original on hit; sets `__state` to signal a miss. |
| `IsInAtmosphereGetterPostfix` | Postfix | Stores the fresh result into the cache when `__state` indicates a miss. |
| `Configure()` / `OnConfigChanged` | Static methods | Gate the patch on `Config.FixWindTurbine` and clear the cache on disable. |
| `Update()` | Static method | Advances cache TTL cleanup each simulation tick. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyWindTurbine.IsInAtmosphere` (getter) | Prefix | Returns the cached atmosphere result, skipping the original check on a cache hit. |
| `MyWindTurbine.IsInAtmosphere` (getter) | Postfix | Stores the freshly computed atmosphere result for future cache hits. |

## References

- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
