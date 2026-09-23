# `Shared/Patches/Voxel/MyStorageBasePatchForVoxelPreload.cs`

*Drops the voxel storage loads the startup preload asks for, and only those, by returning `null` from `MyStorageBase.LoadFromFile`.*

|  |  |
| --- | --- |
| **Module** | [Memory Allocation Patches](../../../../modules/memory-allocation.md) |
| **Source** | [`MyStorageBasePatchForVoxelPreload.cs`](../../../../../Shared/Patches/Voxel/MyStorageBasePatchForVoxelPreload.cs) (37 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

The other half of [`MySandboxGamePatchForVoxelPreload.cs`](MySandboxGamePatchForVoxelPreload.cs.md). A prefix on `MyStorageBase.LoadFromFile` returns `null` for the loads issued inside the preload window and lets every other call through.

`null` is a value the preload already handles: `LoadFromFile` returns `null` for a definition whose file is missing, and the preload's only use of the result is the `ResetDataCache()` call on the `IsMemoryLimited` path, which the window patch excludes.

The call shape is part of the condition. The preload is the only caller in the game which passes `logInfo: false`, so a voxel load running on another thread while the window is open is left alone rather than relying on the window being exclusive.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `LoadFromFilePrefix` | Harmony Prefix | Returns `null` for a cached, non-logging load inside the preload window; otherwise runs the original. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyStorageBase.LoadFromFile(string, Dictionary<byte, byte>, bool, bool)` | Prefix | Skips the load when the preload window is open and the call is the preload's own. |

## References

- [memory-allocation](../../../../modules/memory-allocation.md)
- [`MySandboxGamePatchForVoxelPreload.cs`](MySandboxGamePatchForVoxelPreload.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Memory Allocation Patches](../../../../modules/memory-allocation.md) · [Index](../../../../Index.md)*
