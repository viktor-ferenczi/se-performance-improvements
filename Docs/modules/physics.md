# Physics Patches

Havok and physics-engine optimizations: a fixed Havok thread count, a faster `RigidBody` getter and a better-complexity cluster reordering algorithm.

The physics module addresses three distinct but related bottlenecks in the Havok-based physics engine used by Space Engineers.

The first is a Havok thread-count regression introduced when Keen added `MyVRage.Platform.System.OptimalHavokThreadCount` but left it `null`, causing the Havok job pool to fall back to single-threaded execution. [`MyPhysicsPatch.cs`](../files/Shared/Patches/Physics/MyPhysicsPatch.cs.md) corrects this by overwriting the thread count in `MyPhysics.LoadData` with `min(16, processorCount)`.

The second is a quadratic-time algorithm inside `MyClusterTree.ReorderClusters` that matches physics object data to clusters via an O(N×M) nested loop. [`MyClusterTreePatch.cs`](../files/Shared/Patches/Physics/MyClusterTreePatch.cs.md) replaces this with an O(N+M) set-union pass, reducing load time and mid-game lag when many grids are moving.

The third is a minor IL inefficiency in the frequently-called `MyPhysicsBody.RigidBody` getter, which loads a parent-body field twice. [`MyPhysicsBodyPatch.cs`](../files/Shared/Patches/Physics/MyPhysicsBodyPatch.cs.md) removes the redundant load with a transpiler.

[`PhysicsFixes.cs`](../files/Shared/Patches/Physics/PhysicsFixes.cs.md) is a shared utility that groups the `SetClusterSize` helper used to keep all `MyClusterTree` size statics in sync.

## Files

| File | Summary |
| --- | --- |
| [`MyClusterTreePatch.cs`](../files/Shared/Patches/Physics/MyClusterTreePatch.cs.md) | Transpiler replacing the O(N×M) nested loop in `ReorderClusters` with an O(N+M) set-union algorithm. |
| [`MyPhysicsBodyPatch.cs`](../files/Shared/Patches/Physics/MyPhysicsBodyPatch.cs.md) | Transpiler removing a redundant field load in the `RigidBody` property getter. |
| [`MyPhysicsPatch.cs`](../files/Shared/Patches/Physics/MyPhysicsPatch.cs.md) | Transpiler fixing the Havok thread count in `MyPhysics.LoadData` to use all available CPU cores. |
| [`PhysicsFixes.cs`](../files/Shared/Patches/Physics/PhysicsFixes.cs.md) | Shared utility: `SetClusterSize` keeps all `MyClusterTree` size statics consistent. |

## How it fits together

All three patch classes follow the same lifecycle pattern: a static `Configure()` method is called by the plugin on startup and config change, setting an `enabled` flag from `Config.FixPhysics`. [`MyPhysicsPatch.cs`](../files/Shared/Patches/Physics/MyPhysicsPatch.cs.md) additionally gates itself on `.NET Framework` to avoid native memory corruption on .NET Core.

[`MyClusterTreePatch.cs`](../files/Shared/Patches/Physics/MyClusterTreePatch.cs.md) is the most architecturally complex: its transpiler rewrites a non-trivial IL sequence and delegates the replacement logic to `OptimizedImplementation`, which uses a per-thread `HashSet<ulong>` (`CollidedObjectKeysPool`) from a `ThreadLocal` pool. This avoids allocations on the hot path while remaining thread-safe.

[`PhysicsFixes.cs`](../files/Shared/Patches/Physics/PhysicsFixes.cs.md) has no patch attributes of its own; it is a plain utility class called from outside the module (e.g., plugin configuration handlers) to adjust `MyClusterTree` cluster-size parameters as a group.

The `[EnsureCode]` attribute on each transpiler guards against game updates silently breaking the IL rewrite — if the target method's hash changes, the patch is skipped and a warning is logged rather than corrupting physics state. See [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md) and [`TranspilerHelpers.cs`](../files/Shared/Tools/TranspilerHelpers.cs.md) for the shared infrastructure.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
