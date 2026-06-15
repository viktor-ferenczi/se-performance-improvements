# `Shared/Patches/MergeAndPaste/MyConveyorLinePatch.cs`

*Suppresses `MyConveyorLine.UpdateIsWorking` calls for the entire duration of a grid merge, deferring the work until the merge completes.*

|  |  |
| --- | --- |
| **Module** | [Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) |
| **Source** | [`MyConveyorLinePatch.cs`](../../../../../Shared/Patches/MergeAndPaste/MyConveyorLinePatch.cs) (24 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

As described in the "Conveyor updates while merging grids" section of `Docs/PerformanceFixes.md`, calling `MyConveyorLine.UpdateIsWorking` repeatedly during a merge of grids with long conveyor systems is the primary bottleneck that makes merge operations slow.

This patch adds a Prefix to `MyConveyorLine.UpdateIsWorking` that returns `false` (skipping the original method) whenever [`MyCubeGridPatchForMergeAndPaste.cs`](MyCubeGridPatchForMergeAndPaste.cs.md).`IsInMergeGridInternal` is `true`. That flag is set by the [`MyCubeGridPatchForMergeAndPaste.cs`](MyCubeGridPatchForMergeAndPaste.cs.md) prefix on `MergeGridInternal` and cleared in its postfix once the merge finishes and `ConveyorSystem.FlagForRecomputation()` has been called. The config check is intentionally omitted here (it is too hot a path) and is done upstream in [`MyCubeGridPatchForMergeAndPaste.cs`](MyCubeGridPatchForMergeAndPaste.cs.md) instead.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `UpdateIsWorkingPrefix()` | `static bool` (Harmony Prefix) | Returns `false` (skips original) while a `MergeGridInternal` call is on the stack; otherwise returns `true`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyConveyorLine.UpdateIsWorking` | Prefix | Skips the method body while a grid merge is in progress. |

## References

- [`MyCubeGridPatchForMergeAndPaste.cs`](MyCubeGridPatchForMergeAndPaste.cs.md) — owns the `IsInMergeGridInternal` flag this patch reads and calls `FlagForRecomputation()` on exit.
- [merge-and-paste](../../../../modules/merge-and-paste.md) — parent module.
- [`PatchHelpers.cs`](../PatchHelpers.cs.md) — applies this patch via `harmony.PatchAll`.

---

*[Handbook](../../../../TOC.md) · [Module: Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) · [Index](../../../../Index.md)*
