# `Shared/Patches/Voxel/MyStorageExtensionsPatch.cs`

*Eliminates per-call `MyStorageData` allocations in `IMyStorageExtensions.GetMaterialAt` by maintaining a fixed-size pool of reusable storage objects.*

|  |  |
| --- | --- |
| **Module** | [Memory Allocation Patches](../../../../modules/memory-allocation.md) |
| **Source** | [`MyStorageExtensionsPatch.cs`](../../../../../Shared/Patches/Voxel/MyStorageExtensionsPatch.cs) (86 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`IMyStorageExtensions.GetMaterialAt` is called frequently during voxel material lookups (e.g., surface queries for landing gear, character footsteps). The original implementation allocates a `MyStorageData` object on every call, creating significant GC pressure in voxel-heavy worlds.

This Prefix replaces the method entirely. It maintains a static pool of 8 pre-allocated `MyStorageData` objects (each pre-sized to `Vector3I.One`). On entry the Prefix atomically claims a slot using `Interlocked.CompareExchange` on the `Used` array, reads the material from voxel storage via `ReadRange`, converts the material index to a `MyVoxelMaterialDefinition`, releases the slot, and returns `false` to skip the original. If all 8 slots are in use (very unlikely contention), it falls back to the original method by returning `true`.

The pool is initialized in the static constructor. The fix is controlled by `Config.FixVoxel` and reacts to config changes via `OnConfigChanged`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Pool` | `MyStorageData[8]` | Pre-allocated storage objects, each sized to one voxel cell. |
| `Used` | `int[8]` | Spinlock flags per pool slot; `1` = in use, `0` = free (CAS-operated). |
| `Configure()` / `OnConfigChanged()` | Static methods | Sync `enabled` from `Config.FixVoxel` on start and config changes. |
| `GetMaterialAtVector3IPrefix` | Harmony Prefix | Claims a pool slot, reads material from storage, converts index to definition, releases slot. Falls back on contention. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `IMyStorageExtensions.GetMaterialAt(IMyStorage, ref Vector3I)` | Prefix | Replaces the per-call-allocating original with a pool-based, allocation-free implementation. |

## References

- [memory-allocation](../../../../modules/memory-allocation.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Memory Allocation Patches](../../../../modules/memory-allocation.md) · [Index](../../../../Index.md)*
