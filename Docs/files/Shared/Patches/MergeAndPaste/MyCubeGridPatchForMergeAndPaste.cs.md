# `Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs`

*Coordinates the merge and paste performance fixes on `MyCubeGrid`: gates conveyor updates during merges via a thread-local depth counter and suppresses all world updates during paste operations.*

|  |  |
| --- | --- |
| **Module** | [Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) |
| **Source** | [`MyCubeGridPatchForMergeAndPaste.cs`](../../../../../Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs) (85 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

This class implements two distinct fixes from `Docs/PerformanceFixes.md`.

**Conveyor updates while merging grids.** A thread-local `CallDepth` counter is incremented by the `MergeGridInternal` Prefix and decremented in its Postfix. As long as the depth is greater than zero, the public property `IsInMergeGridInternal` returns `true`, which [`MyConveyorLinePatch.cs`](MyConveyorLinePatch.cs.md) reads to skip every `UpdateIsWorking` call. When the outermost merge finishes (`CallDepth` returns to zero), the Postfix calls `GridSystems.ConveyorSystem.FlagForRecomputation()` on the merged grid so all `IsWorking` values are recalculated correctly. Using a thread-local counter means nested merges on the same thread are handled safely without any locking.

**Updates while pasting grids.** The `PasteBlocksServer` Prefix saves the current value of `MySession.Static.IsUpdateAllowed()` to the Harmony `__state` variable, then sets it to `false`, disabling all world updates for the duration of the paste. The Postfix restores the original value. According to `Docs/PerformanceFixes.md`, this combined with the conveyor fix makes merge and paste operations ~60–70% faster in heavy test worlds. The `[EnsureCode]` attribute is omitted for `PasteBlocksServer` because other plugins also patch that method.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `CallDepth` | `ThreadLocal<int>` (private static) | Tracks nested `MergeGridInternal` call depth per thread. |
| `IsInMergeGridInternal` | `static bool` (public) | Returns `true` while any `MergeGridInternal` call is active on the current thread. Read by [`MyConveyorLinePatch.cs`](MyConveyorLinePatch.cs.md). |
| `MergeGridInternalPrefix` | `static bool` (Harmony Prefix) | Increments `CallDepth` and sets `__state = true` when the merge fix is enabled. |
| `MergeGridInternalPostfix` | `static void` (Harmony Postfix) | Decrements `CallDepth`; when depth returns to zero calls `FlagForRecomputation()` on the merged grid. |
| `PasteBlocksServerPrefix` | `static bool` (Harmony Prefix) | Saves `IsUpdateAllowed()` to `__state` and sets it to `false` to suppress updates during paste. |
| `PasteBlocksServerPostfix` | `static void` (Harmony Postfix) | Restores `IsUpdateAllowed` to the saved value. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyCubeGrid.MergeGridInternal` | Prefix | Increments the merge-depth counter so [`MyConveyorLinePatch.cs`](MyConveyorLinePatch.cs.md) knows to suppress conveyor updates. |
| `MyCubeGrid.MergeGridInternal` | Postfix | Decrements the counter; triggers `FlagForRecomputation()` when the outermost merge completes. |
| `MyCubeGrid.PasteBlocksServer` | Prefix | Sets `MySession.IsUpdateAllowed = false` to suppress all world updates during paste. |
| `MyCubeGrid.PasteBlocksServer` | Postfix | Restores `MySession.IsUpdateAllowed` to its pre-paste value. |

## References

- [`MyConveyorLinePatch.cs`](MyConveyorLinePatch.cs.md) — reads `IsInMergeGridInternal` to suppress conveyor line updates.
- [merge-and-paste](../../../../modules/merge-and-paste.md) — parent module.
- [`PatchHelpers.cs`](../PatchHelpers.cs.md) — applies this patch via `harmony.PatchAll`.

---

*[Handbook](../../../../TOC.md) · [Module: Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) · [Index](../../../../Index.md)*
