# `Shared/Patches/Physics/MyClusterTreePatch.cs`

*Replaces the O(N×M) nested loop in `MyClusterTree.ReorderClusters` with a set-union approach of lower time complexity.*

|  |  |
| --- | --- |
| **Module** | [Physics Patches](../../../../modules/physics.md) |
| **Source** | [`MyClusterTreePatch.cs`](../../../../../Shared/Patches/Physics/MyClusterTreePatch.cs) (122 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyClusterTree.ReorderClusters` contains a nested loop that iterates over every cluster in `m_resultList` and for each cluster scans the entire `m_objectsData` dictionary to find matching keys. On servers with many moving grids the number of clusters and objects can be large, making this O(N×M) work a measurable simulation cost.

The transpiler removes the inner loop body and replaces it with a call to `OptimizedImplementation`. That function first collects all object keys from every collided cluster into a thread-local `HashSet<ulong>` (`CollidedObjectKeysPool`) using `UnionWith`, then does a single O(K) pass over just those keys to look up objects in `m_objectsData`. The result is O(N+M) instead of O(N×M), improving load time and reducing lag when grids move around.

The patch is gated on `Config.FixPhysics` and is controlled by the shared `Configure()` / `enabled` pattern used across the physics module.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Configure()` | Static method | Reads `Config.FixPhysics` and sets the `enabled` flag. |
| `ReorderClustersTranspiler` | Harmony Transpiler | Removes the O(N×M) nested loop and injects a call to `OptimizedImplementation`. Uses [`TranspilerHelpers.cs`](../../Tools/TranspilerHelpers.cs.md) (`FindAllIndex`, `RecordOriginalCode`, `RecordPatchedCode`). |
| `CollidedObjectKeysPool` | `ThreadLocal<HashSet<ulong>>` | Per-thread key accumulator reused across calls to avoid allocations. |
| `OptimizedImplementation` | Static method | Collects cluster object keys via `UnionWith`, then resolves each key against `m_objectsData` in a single pass. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyClusterTree.ReorderClusters` | Transpiler | Replaces O(N×M) nested loop with O(N+M) set-union algorithm. |

## References

- [physics](../../../../modules/physics.md)
- [`PhysicsFixes.cs`](PhysicsFixes.cs.md)
- [`TranspilerHelpers.cs`](../../Tools/TranspilerHelpers.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Physics Patches](../../../../modules/physics.md) · [Index](../../../../Index.md)*
