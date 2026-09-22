# `Shared/Patches/Voxel/MySandboxGamePatchForVoxelPreload.cs`

*Marks the window in which `MySandboxGame.PerformPreloading` runs, so the vanilla asteroid voxel storages it would eagerly load on game start can be skipped.*

|  |  |
| --- | --- |
| **Module** | [Memory Allocation Patches](../../../../modules/memory-allocation.md) |
| **Source** | [`MySandboxGamePatchForVoxelPreload.cs`](../../../../../Shared/Patches/Voxel/MySandboxGamePatchForVoxelPreload.cs) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MySandboxGame.PerformPreloading` runs while the game starts, before any world is loaded. It walks every `VoxelMapStorage` definition which can take part in procedural asteroid generation and calls `MyStorageBase.LoadFromFile` on each, purely to warm the LRU cache inside `MyStorageBase`. The loop costs roughly a gigabyte of managed memory whether or not the session will ever contain an asteroid, and the dedicated server pays it too.

Skipping it is a lazy-versus-eager change: `LoadFromFile` *is* the cache lookup, so a caller which needs one of these storages later loads it then and gets the same shared object. The game itself treats the preloaded data as optional — it calls `ResetDataCache()` on what it has just loaded when the platform reports `IsMemoryLimited`.

This class only marks the window. The loads are dropped by [`MyStorageBasePatchForVoxelPreload.cs`](MyStorageBasePatchForVoxelPreload.cs.md), which asks it whether a given load falls inside the window, so voxels a session actually needs still load normally.

The patch belongs to the `Early` Harmony category: the preload starts before `IPlugin.Init` runs on the client, so both patches are applied from the plugin's `Preloader` through the `MyInitializer.InvokeBeforeRun` hook (see [`Plugin.cs`](../../../ClientPlugin/Plugin.cs.md) and [`PatchHelpers.cs`](../PatchHelpers.cs.md)).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `enabled` | `bool` | `Config.Enabled && Config.SkipVoxelPreload`, sampled when the preload starts. |
| `preloading` | `volatile bool` | True only while `PerformPreloading` is on the stack; read from the preload's `Parallel.For` worker threads. |
| `skipped` | `int` | Number of storage loads dropped in the window, logged when it closes (DEBUG). |
| `Configure()` / `OnConfigChanged()` | Static methods | Sync `enabled` from the config; subscribe once, since the early bootstrap may call this before the plugin is fully initialized. |
| `ShouldSkipStorageLoad()` | Internal method | Answers the `LoadFromFile` prefix and counts the skip. |
| `PerformPreloadingPrefix` | Harmony Prefix | Opens the window, unless the platform is memory limited (the preload would then dereference the null result). |
| `PerformPreloadingFinalizer` | Harmony Finalizer | Closes the window on both the normal and the exceptional exit, and logs the count. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MySandboxGame.PerformPreloading()` | Prefix | Opens the skip window for the duration of the preload. |
| `MySandboxGame.PerformPreloading()` | Finalizer | Closes it again, whatever the preload did. |

## References

- [memory-allocation](../../../../modules/memory-allocation.md)
- [`MyStorageBasePatchForVoxelPreload.cs`](MyStorageBasePatchForVoxelPreload.cs.md)
- [`PatchHelpers.cs`](../PatchHelpers.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Memory Allocation Patches](../../../../modules/memory-allocation.md) · [Index](../../../../Index.md)*
