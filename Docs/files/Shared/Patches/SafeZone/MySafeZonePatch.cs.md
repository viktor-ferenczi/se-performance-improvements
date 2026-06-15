# `Shared/Patches/SafeZone/MySafeZonePatch.cs`

*Caches `MySafeZone.IsSafe` and `IsActionAllowed` results for ~2 seconds and replaces `IsOutside(BoundingBoxD)` with an allocation-free distance check.*

|  |  |
| --- | --- |
| **Module** | [Safe Zone Patches](../../../../modules/safe-zone.md) |
| **Source** | [`MySafeZonePatch.cs`](../../../../../Shared/Patches/SafeZone/MySafeZonePatch.cs) (180 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MySafeZone.IsSafe` and `MySafeZone.IsActionAllowed` are called very frequently on busy multiplayer servers with many small grids and safe zones, adding significant CPU overhead. The patch caches both results using [`UintCache.cs`](../../Tools/UintCache.cs.md) keyed on entity ID (for `IsSafe`) or a combined key of instance, entity, source and action IDs (for `IsActionAllowed`). Results are retained for approximately 128 simulation ticks (~2 seconds), so ownership changes propagate with at most 2 seconds of delay — acceptable given the performance savings.

`IsOutside(BoundingBoxD)` is replaced entirely via a Prefix that returns a fast conservative distance check: the squared distance from the AABB centre to the safe zone position is compared against the sum of the relevant radius/half-extents. No new bounding boxes are allocated on this path. The method returns `false` (run original) when the fast check cannot conclude the box is outside, so correctness is preserved.

`Configure()` reads `Config.FixSafeZone`, and `OnConfigChanged` re-checks the flag and clears both caches on disable. `Update()` is called every tick by the plugin to expire stale cache entries via `Cleanup()`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `IsSafeCache` | `UintCache<long>` | Tick-expiry cache for `IsSafe` results keyed by entity ID (capacity 256, TTL ≈120 ticks per entity). |
| `IsActionAllowedCache` | `UintCache<long>` | Tick-expiry cache for `IsActionAllowed` results keyed by a combined XOR key (capacity default). |
| `Configure()` | Static method | Sets `enabled` from `Config.FixSafeZone` and subscribes `OnConfigChanged`. |
| `Update()` | Static method | Calls `Cleanup()` on both caches every game tick to expire old entries. |
| `IsSafePrefix` / `IsSafePostfix` | Harmony Prefix + Postfix | Serve cache hits and populate the cache with the real result. |
| `IsOutsidePrefix` | Harmony Prefix | Allocation-free sphere/box distance check that short-circuits the original if the AABB is clearly outside. |
| `IsActionAllowedPrefix` / `IsActionAllowedPostfix` | Harmony Prefix + Postfix | Serve cache hits for `IsActionAllowed`; XOR encode the boolean result with the low 32 bits of the entity ID to detect rare key collisions. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MySafeZone.IsSafe` | Prefix + Postfix | Caches the boolean result per entity for ~2 seconds. |
| `MySafeZone.IsOutside(BoundingBoxD)` | Prefix | Replaces the original with an allocation-free distance check. |
| `MySafeZone.IsActionAllowed(MyEntity, MySafeZoneAction, long)` | Prefix + Postfix | Caches the boolean result per (zone, entity, source, action) tuple for ~2 seconds. |

## References

- [safe-zone](../../../../modules/safe-zone.md)
- [`UintCache.cs`](../../Tools/UintCache.cs.md)
- [`MySessionComponentSafeZonesPatch.cs`](MySessionComponentSafeZonesPatch.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Safe Zone Patches](../../../../modules/safe-zone.md) · [Index](../../../../Index.md)*
