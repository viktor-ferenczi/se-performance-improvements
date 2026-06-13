# `Shared/Patches/Physics/MyPhysicsPatch.cs`

*Fixes the Havok thread count in `MyPhysics.LoadData` so all available CPU cores are used for physics simulation.*

|  |  |
| --- | --- |
| **Module** | [Physics Patches](../../../../modules/physics.md) |
| **Source** | [`MyPhysicsPatch.cs`](../../../../../Shared/Patches/Physics/MyPhysicsPatch.cs) (51 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

Keen introduced `MyVRage.Platform.System.OptimalHavokThreadCount` but left it `null`. The updated logic in `MyPhysics.LoadData` that reads this property falls back to a single-threaded Havok job pool instead of the previous multi-threaded `HkJobThreadPool()` call. As a result all physics simulation ends up running on the main thread, eliminating any parallelism in the Havok engine.

The transpiler injects two IL instructions immediately after the existing `stloc.0` that stores the (faulty) thread count: it pushes `min(16, Environment.ProcessorCount)` and stores it into `loc_0`, overwriting the broken value before it is consumed by the Havok pool constructor. This restores the original multi-threaded behaviour without touching any other logic in `LoadData`.

Because this patch involves native Havok memory and causes corruption on .NET Core, it is enabled only when running on .NET Framework (i.e., the standalone game client on Windows).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Configure()` | Static method | Enables the patch only under .NET Framework when `Config.FixPhysics` is set. |
| `LoadDataTranspiler` | Harmony Transpiler | Injects `ldc.i4 <count>` + `stloc.0` after the existing thread-count assignment to override it with the correct core count. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyPhysics.LoadData` | Transpiler | Overwrites the Havok thread count with `min(16, processorCount)` so physics runs multi-threaded. |

## References

- [physics](../../../../modules/physics.md)
- [`TranspilerHelpers.cs`](../../Tools/TranspilerHelpers.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Physics Patches](../../../../modules/physics.md) · [Index](../../../../Index.md)*
