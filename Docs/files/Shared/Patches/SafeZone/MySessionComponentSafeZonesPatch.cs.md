# `Shared/Patches/SafeZone/MySessionComponentSafeZonesPatch.cs`

*Caches `MySessionComponentSafeZones.IsActionAllowedForSafezone` results for ~2 seconds, reducing overhead from high-frequency safe-zone action checks.*

|  |  |
| --- | --- |
| **Module** | [Safe Zone Patches](../../../../modules/safe-zone.md) |
| **Source** | [`MySessionComponentSafeZonesPatch.cs`](../../../../../Shared/Patches/SafeZone/MySessionComponentSafeZonesPatch.cs) (92 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MySessionComponentSafeZones.IsActionAllowedForSafezone` is a session-level method that aggregates the allowed-action state across all safe zones for a given entity. Like the per-zone counterpart in [`MySafeZonePatch.cs`](MySafeZonePatch.cs.md), it is called very frequently on busy servers, so the result is cached using a [`UintCache.cs`](../../Tools/UintCache.cs.md) keyed on `entityId ^ sourceEntityId ^ (long)action`.

Cache entries live for approximately 120 ticks, with a small per-entity jitter (`entityIdLow32Bits & 15`) to spread expiry across frames. As with `IsActionAllowed`, the stored value XORs the boolean result with the low 32 bits of the entity ID so that rare key collisions are detectable (value > 1 forces a cache bypass).

The patch is controlled by a separate config flag, `Config.FixSafeAction`, allowing it to be toggled independently of the per-zone `IsSafe` / `IsOutside` fixes. Disabling it at runtime clears the cache immediately via `OnConfigChanged`. `Update()` is called every tick to expire old entries.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Cache` | `UintCache<long>` | Tick-expiry cache for `IsActionAllowedForSafezone` results (capacity 128, TTL ≈120 ticks). |
| `Configure()` | Static method | Sets `enabled` from `Config.FixSafeAction` and subscribes `OnConfigChanged`. |
| `Update()` | Static method | Calls `Cache.Cleanup()` every game tick to expire stale entries. |
| `IsActionAllowedForSafezonePrefix` / `IsActionAllowedForSafezonePostfix` | Harmony Prefix + Postfix | Serve cache hits and populate the cache; XOR-encode result with `entityIdLow32Bits` to detect key collisions. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MySessionComponentSafeZones.IsActionAllowedForSafezone(MyEntity, MySafeZoneAction, long)` | Prefix + Postfix | Caches the boolean result per (entity, source, action) tuple for ~2 seconds. |

## References

- [safe-zone](../../../../modules/safe-zone.md)
- [`UintCache.cs`](../../Tools/UintCache.cs.md)
- [`MySafeZonePatch.cs`](MySafeZonePatch.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Safe Zone Patches](../../../../modules/safe-zone.md) · [Index](../../../../Index.md)*
