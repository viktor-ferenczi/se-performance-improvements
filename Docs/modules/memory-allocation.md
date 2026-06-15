# Memory Allocation Patches

Reduces GC pressure from hot paths: a cached `MyDefinitionId.ToString`, less frequent block-limit sync and allocation-free voxel material lookups.

The game allocates memory in several hot paths that run every tick, creating sustained GC pressure that causes periodic pauses. This module targets three independent sources of such allocations.

`MyDefinitionId.ToString()` is called throughout the game to log and format definition identifiers. It constructs a new string every call. Keen attempted a fix in update 1.202.066 but introduced a deadlock; [`MyDefinitionIdToStringPatch.cs`](../files/Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs.md) re-applies the caching approach using a [`TwoLayerCache.cs`](../files/Shared/Tools/TwoLayerCache.cs.md) keyed on the definition's 64-bit hash.

`MyPlayerCollection.SendDirtyBlockLimits` runs every tick to synchronise block-count limits to clients. [`MyPlayerCollectionPatch.cs`](../files/Shared/Patches/Memory/MyPlayerCollectionPatch.cs.md) reduces this to once every 180 ticks (~3 seconds) with a one-line tick-modulo Prefix.

`IMyStorageExtensions.GetMaterialAt` allocates a `MyStorageData` object per call during voxel surface queries. [`MyStorageExtensionsPatch.cs`](../files/Shared/Patches/Voxel/MyStorageExtensionsPatch.cs.md) replaces this with a pool of 8 reusable objects claimed via `Interlocked.CompareExchange`, making the lookup entirely allocation-free in the common case.

## Files

| File | Summary |
| --- | --- |
| [`MyDefinitionIdToStringPatch.cs`](../files/Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs.md) | Caches `MyDefinitionId.ToString()` results in a two-layer cache, eliminating repeated string allocations. |
| [`MyPlayerCollectionPatch.cs`](../files/Shared/Patches/Memory/MyPlayerCollectionPatch.cs.md) | Rate-limits `SendDirtyBlockLimits` to once every 180 ticks via a tick-modulo Prefix. |
| [`MyStorageExtensionsPatch.cs`](../files/Shared/Patches/Voxel/MyStorageExtensionsPatch.cs.md) | Replaces per-call `MyStorageData` allocation in `GetMaterialAt` with an 8-slot object pool. |

## How it fits together

The three files in this module address unrelated call sites and share no runtime state with each other. Each is independently controlled by a separate config flag (`Config.FixMemory`, `Config.FixBlockLimit`, `Config.FixVoxel`).

[`MyDefinitionIdToStringPatch.cs`](../files/Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs.md) is the most sophisticated: the [`TwoLayerCache.cs`](../files/Shared/Tools/TwoLayerCache.cs.md) it uses has a mutable insertion layer and a periodically-promoted immutable snapshot layer. The immutable layer enables lock-free reads on the hottest path. The `Update()` method drives the promotion schedule.

[`MyPlayerCollectionPatch.cs`](../files/Shared/Patches/Memory/MyPlayerCollectionPatch.cs.md) is deliberately minimal — it has no cache or state and is reset-free; the rate limit is naturally maintained by the tick-modulo condition.

[`MyStorageExtensionsPatch.cs`](../files/Shared/Patches/Voxel/MyStorageExtensionsPatch.cs.md) uses array-based spinlocks (`Interlocked.CompareExchange` on `int[]`) rather than `lock` to avoid blocking the calling thread if all slots are busy, falling back gracefully to the allocating original. The pool size of 8 is chosen to handle expected concurrency on a typical server without wasting memory.

For GC-pressure context see also the safe-zone allocation fix in [safe-zone](safe-zone.md) (`IsOutside` bounding-box elimination) and the cluster-reordering set reuse in [physics](physics.md).

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
