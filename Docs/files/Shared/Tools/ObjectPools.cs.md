# `Shared/Tools/ObjectPools.cs`

*Shared `StringBuilder` pool backed by the game's `MyConcurrentBucketPool`, reducing GC pressure from frequent string formatting.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`ObjectPools.cs`](../../../../Shared/Tools/ObjectPools.cs) (50 lines) |
| **Kind** | Static utility class `ObjectPools` |
| **Role** | Misc utility / GC reduction |

## Purpose

One of the plugin's performance themes is reducing heap allocations in hot paths to lower GC pressure. `ObjectPools` provides a process-wide, bucket-based `StringBuilder` pool using `MyConcurrentBucketPool<StringBuilder>` from the game's own VRage collections. Buckets are sized in powers of two (16-byte increments); the allocator pre-sets `MaxCapacity` to `bucketSize + BucketSize` so pooled instances are not silently enlarged beyond their bucket slot.

Patches that need temporary string building (for example, the `MyDefinitionId.ToString` cache and similar string-formatting patches) borrow from this pool to avoid allocating a new `StringBuilder` on every call, then return it after use.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `StringBuilder` | Static field | The shared `MyConcurrentBucketPool<StringBuilder>` instance. Borrow with `Allocate` / return with `Deallocate`. |
| `StringBuilderAllocator` | Nested private class | `IMyElementAllocator<StringBuilder>` implementation; maps bucket IDs to initial capacities and clears instances on borrow and return. |

## References

- None.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
