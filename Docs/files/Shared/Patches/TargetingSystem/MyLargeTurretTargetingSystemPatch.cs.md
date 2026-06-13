# `Shared/Patches/TargetingSystem/MyLargeTurretTargetingSystemPatch.cs`

*Reduces GC pressure in `MyLargeTurretTargetingSystem.SortTargetRoots` by reusing per-turret arrays instead of allocating a new one every call (entire file is currently disabled via `#if false`).*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyLargeTurretTargetingSystemPatch.cs`](../../../../../Shared/Patches/TargetingSystem/MyLargeTurretTargetingSystemPatch.cs) (318 lines) |
| **Kind** | Static Harmony patch class (currently disabled — entire file wrapped in `#if false`) |
| **Role** | Performance patch |

## Purpose

`MyLargeTurretTargetingSystem.SortTargetRoots` allocated a fresh `MyEntity[]` via `ToArray()` on every call, and resized a `m_distanceEntityKeys` float array alongside it. In worlds with many turrets and targets this produces significant GC pressure (see the "Reducing memory allocations in the turret targeting system" section in `Docs/PerformanceFixes.md`).

The transpiler (`SortTargetRootsTranspiler`) replaces the `ToArray()` call with `CopyEntitiesIntoArray`, which looks up a per-turret `MyEntity[]` in `ArrayCache` (keyed by the turret's `EntityId`) and reuses it if it is large enough but not excessively oversized (at most 4× the required count). It also resizes `m_distanceEntityKeys` in the same helper, removing the now-redundant `EnsureCapacity` call that followed. Entries expire after roughly 6 minutes of idleness (377 × 60 ticks).

A second region (visibility cache replacement) is present in the source but wrapped in an inner `#if false` block — it was disabled due to reported crashes and is not described further here.

The whole file is compiled out (`#if false`) pending a rewrite to target the Automaton (1.202.066) version of the game code.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `ArrayCache` | `Cache<long, MyEntity[]>` | Per-turret array cache keyed by entity ID; TTL ~6 min (377×60 ticks). |
| `SortTargetRootsTranspiler` | Transpiler | Replaces `ToArray()` with `CopyEntitiesIntoArray` and removes the subsequent redundant `EnsureCapacity` call. |
| `CopyEntitiesIntoArray` | Static method | Retrieves or creates the cached array, copies target roots into it, nulls trailing slots, and also resizes `m_distanceEntityKeys` if needed. |
| `Configure()` | Static method | Gates the patch on `Config.Enabled && Config.FixTargeting`. |
| `Update()` | Static method | Called each tick; currently a no-op (visibility cache cleanup call is commented out). |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyLargeTurretTargetingSystem.SortTargetRoots` | Transpiler | Replaces per-call `ToArray()` allocation with a pooled, per-turret array reused across calls. |

## References

- [`Cache.cs`](../../Tools/Cache.cs.md) — generic TTL cache used for `ArrayCache`
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
