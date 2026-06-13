# Grid Merge & Paste Patches

Suppresses redundant conveyor and world updates while grids are merged, pasted or moved between grid groups, making those operations dramatically faster.

This module addresses three performance problems documented in `Docs/PerformanceFixes.md`: (1) redundant `MyConveyorLine.UpdateIsWorking` calls flooding in during grid merges, (2) unnecessary world updates running throughout the `PasteBlocksServer` call, and (3) heavy main-thread resource-distributor work triggered by every grid group change (connector lock/unlock, rotor attach/detach). Together the first two fixes deliver ~60–70% faster merge and paste operations in heavy worlds.

The module contains four files. Two of them ([`MyCubeGridPatchForMergeAndPaste.cs`](../files/Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs.md) and [`MyConveyorLinePatch.cs`](../files/Shared/Patches/MergeAndPaste/MyConveyorLinePatch.cs.md)) are active patches that cooperate via a shared thread-local flag. The other two ([`MyGroupsPatch.cs`](../files/Shared/Patches/MergeAndPaste/MyGroupsPatch.cs.md) and [`MyResourceDistributorComponentPatch.cs`](../files/Shared/Patches/MergeAndPaste/MyResourceDistributorComponentPatch.cs.md)) are currently disabled because Harmony cannot patch generic methods, but they contain the intended design for the grid-group-changes fix.

## Files

| File | Summary |
| --- | --- |
| [`MyConveyorLinePatch.cs`](../files/Shared/Patches/MergeAndPaste/MyConveyorLinePatch.cs.md) | Skips `MyConveyorLine.UpdateIsWorking` while a grid merge is in progress. |
| [`MyCubeGridPatchForMergeAndPaste.cs`](../files/Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs.md) | Sets the merge-depth flag and suppresses world updates during paste; triggers conveyor recomputation after merge. |
| [`MyGroupsPatch.cs`](../files/Shared/Patches/MergeAndPaste/MyGroupsPatch.cs.md) | Disabled — would track grid group merge/break-link depth to gate resource updates (Harmony cannot patch generics). |
| [`MyResourceDistributorComponentPatch.cs`](../files/Shared/Patches/MergeAndPaste/MyResourceDistributorComponentPatch.cs.md) | Disabled — would defer `UpdateBeforeSimulation` to a worker thread during group changes. |

## How it fits together

**Active path — merge fix:** [`MyCubeGridPatchForMergeAndPaste.cs`](../files/Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs.md) owns the `IsInMergeGridInternal` property backed by a `ThreadLocal<int>` depth counter. Its Prefix on `MyCubeGrid.MergeGridInternal` increments the counter; [`MyConveyorLinePatch.cs`](../files/Shared/Patches/MergeAndPaste/MyConveyorLinePatch.cs.md) reads `IsInMergeGridInternal` in its own Prefix and short-circuits `UpdateIsWorking` for the duration. When the Postfix of `MergeGridInternal` decrements the counter back to zero it calls `ConveyorSystem.FlagForRecomputation()` to recalculate all `IsWorking` states correctly after the merge. The thread-local counter handles recursive/nested merges transparently.

**Active path — paste fix:** [`MyCubeGridPatchForMergeAndPaste.cs`](../files/Shared/Patches/MergeAndPaste/MyCubeGridPatchForMergeAndPaste.cs.md) also wraps `MyCubeGrid.PasteBlocksServer`. Its Prefix saves `MySession.IsUpdateAllowed()` and sets it to `false`; the Postfix restores it. No other file in this module is involved.

**Disabled path — grid group changes:** [`MyGroupsPatch.cs`](../files/Shared/Patches/MergeAndPaste/MyGroupsPatch.cs.md) was designed to expose `IsInMergeGroups` / `IsInBreakLink` flags from `MyGroups<MyCubeGrid,MyGridLogicalGroupData>.MergeGroups` and `.BreakLink`, which [`MyResourceDistributorComponentPatch.cs`](../files/Shared/Patches/MergeAndPaste/MyResourceDistributorComponentPatch.cs.md) would read to skip `UpdateBeforeSimulation` and call `MarkForUpdate()` instead. This pair is disabled because Harmony cannot apply transpilers or prefixes to generic method instantiations.

The module interacts with [patch-infrastructure](patch-infrastructure.md) (for patch application) and at runtime touches the conveyor module's recomputation path in the game itself.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
