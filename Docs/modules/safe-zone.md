# Safe Zone Patches

Caches the frequently-called safe-zone checks (`IsSafe`, `IsActionAllowed`) and replaces `IsOutside` with an allocation-free implementation.

In multiplayer worlds with many grids and safe zones, the game calls `MySafeZone.IsSafe`, `MySafeZone.IsActionAllowed`, and `MySessionComponentSafeZones.IsActionAllowedForSafezone` on every relevant entity every tick. Each call traverses zone ownership and access lists, adding measurable CPU overhead that scales with the number of zones and entities.

This module caches the boolean results of all three methods using [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md) instances, retaining values for approximately 2 seconds (~120 ticks). A minor jitter on the TTL (`entityId & 15`) staggers expiry across frames to avoid spikes. The effect of safe-zone configuration changes or grid ownership changes is therefore delayed by up to 2 seconds, which is acceptable given the performance benefit on busy servers.

`MySafeZone.IsOutside(BoundingBoxD)` is an additional target: the original implementation allocates intermediate bounding boxes. [`MySafeZonePatch.cs`](../files/Shared/Patches/SafeZone/MySafeZonePatch.cs.md) replaces only the `BoundingBoxD` overload with a direct squared-distance check, eliminating the allocations without changing observable behaviour.

## Files

| File | Summary |
| --- | --- |
| [`MySafeZonePatch.cs`](../files/Shared/Patches/SafeZone/MySafeZonePatch.cs.md) | Caches `IsSafe` and `IsActionAllowed`; replaces `IsOutside(BoundingBoxD)` with an allocation-free distance check. |
| [`MySessionComponentSafeZonesPatch.cs`](../files/Shared/Patches/SafeZone/MySessionComponentSafeZonesPatch.cs.md) | Caches `IsActionAllowedForSafezone` at the session-component level for ~2 seconds. |

## How it fits together

The two patch classes are independent but complementary. [`MySafeZonePatch.cs`](../files/Shared/Patches/SafeZone/MySafeZonePatch.cs.md) is controlled by `Config.FixSafeZone` and owns caches for per-zone `IsSafe` and `IsActionAllowed`. [`MySessionComponentSafeZonesPatch.cs`](../files/Shared/Patches/SafeZone/MySessionComponentSafeZonesPatch.cs.md) is controlled by the separate `Config.FixSafeAction` flag and caches the session-level `IsActionAllowedForSafezone`. This split lets administrators enable caching at one level without the other.

Both classes follow the same runtime lifecycle: `Configure()` reads the config flag; `OnConfigChanged` re-reads and clears the cache on disable; `Update()` is called every simulation tick to expire stale entries via [`UintCache.cs`](../files/Shared/Tools/UintCache.cs.md)`.Cleanup()`.

The key-collision defence in both `IsActionAllowed` caches XORs the stored value with the low 32 bits of the entity ID. A decoded value greater than 1 is physically impossible for a boolean, so it signals a collision and forces a cache miss — the original method is called instead. This makes the caches correct under all circumstances at negligible cost.

The `IsOutside` fast-path in [`MySafeZonePatch.cs`](../files/Shared/Patches/SafeZone/MySafeZonePatch.cs.md) runs unconditionally (no `enabled` guard is needed because it is conservative: it only replaces the call when the result is definitely `true`). See also [memory-allocation](memory-allocation.md) for allocation-reduction patterns used across the plugin.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
