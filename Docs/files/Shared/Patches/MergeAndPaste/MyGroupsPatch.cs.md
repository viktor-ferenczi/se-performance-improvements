# `Shared/Patches/MergeAndPaste/MyGroupsPatch.cs`

*Currently disabled (`#if DISABLED`): intended to track when grids are being moved between logical groups so resource-distributor updates can be suppressed during those transitions.*

|  |  |
| --- | --- |
| **Module** | [Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) |
| **Source** | [`MyGroupsPatch.cs`](../../../../../Shared/Patches/MergeAndPaste/MyGroupsPatch.cs) (68 lines) |
| **Kind** | Static Harmony patch class (disabled — compiled out) |
| **Role** | Performance patch (pending) |

## Purpose

This file is wrapped in `#if DISABLED` and is not compiled. It represents the intended solution for the "Lag on grid group changes" problem described in `Docs/PerformanceFixes.md`: connector lock/unlock and rotor head attach/detach cause serious lag because grid group changes trigger heavy main-thread workload that could be deferred to worker threads.

The design uses two thread-local depth counters — `MergeGroupsCallDepth` and `BreakLinkCallDepth` — to expose `IsInMergeGroups` and `IsInBreakLink` boolean properties. These flags are read by [`MyResourceDistributorComponentPatch.cs`](MyResourceDistributorComponentPatch.cs.md) to skip `UpdateBeforeSimulation` and instead call `MarkForUpdate()` so the update happens later on a worker thread. The patch targets `MyGroups<MyCubeGrid, MyGridLogicalGroupData>.MergeGroups` and `.BreakLink`, but Harmony cannot patch generic methods, which is why this approach is currently disabled.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `MergeGroupsCallDepth` | `ThreadLocal<int>` (private static) | Tracks nested `MergeGroups` call depth per thread. |
| `BreakLinkCallDepth` | `ThreadLocal<int>` (private static) | Tracks nested `BreakLink` call depth per thread. |
| `IsInMergeGroups` | `static bool` (public) | `true` while `MergeGroups` is on the call stack for the current thread. |
| `IsInBreakLink` | `static bool` (public) | `true` while `BreakLink` is on the call stack for the current thread. |
| `MergeGroupsPrefix` / `MergeGroupsPostfix` | Harmony Prefix/Postfix | Increment/decrement `MergeGroupsCallDepth`. |
| `BreakLinkPrefix` / `BreakLinkPostfix` | Harmony Prefix/Postfix | Increment/decrement `BreakLinkCallDepth`. |

## Patch targets

> **Note:** This patch class is currently disabled (`#if DISABLED`) because Harmony cannot patch generic methods.

| Target | Patch | Effect |
| --- | --- | --- |
| `MyGroups<MyCubeGrid, MyGridLogicalGroupData>.MergeGroups` | Prefix | Increments `MergeGroupsCallDepth` to signal an active group merge. |
| `MyGroups<MyCubeGrid, MyGridLogicalGroupData>.MergeGroups` | Postfix | Decrements `MergeGroupsCallDepth`. |
| `MyGroups<MyCubeGrid, MyGridLogicalGroupData>.BreakLink` | Prefix | Increments `BreakLinkCallDepth` to signal an active link break. |
| `MyGroups<MyCubeGrid, MyGridLogicalGroupData>.BreakLink` | Postfix | Decrements `BreakLinkCallDepth`. |

## References

- [`MyResourceDistributorComponentPatch.cs`](MyResourceDistributorComponentPatch.cs.md) — the companion patch that reads `IsInMergeGroups` / `IsInBreakLink` to defer resource updates.
- [merge-and-paste](../../../../modules/merge-and-paste.md) — parent module.
- [`PatchHelpers.cs`](../PatchHelpers.cs.md) — would apply this patch via `harmony.PatchAll` if it were enabled.

---

*[Handbook](../../../../TOC.md) · [Module: Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) · [Index](../../../../Index.md)*
