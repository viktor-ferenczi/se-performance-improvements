# `Shared/Patches/MergeAndPaste/MyResourceDistributorComponentPatch.cs`

*Currently disabled (`#if DISABLED`): suppresses `MyResourceDistributorComponent.UpdateBeforeSimulation` during grid group changes and defers the update to a worker thread via `MarkForUpdate()`.*

|  |  |
| --- | --- |
| **Module** | [Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) |
| **Source** | [`MyResourceDistributorComponentPatch.cs`](../../../../../Shared/Patches/MergeAndPaste/MyResourceDistributorComponentPatch.cs) (37 lines) |
| **Kind** | Static Harmony patch class (disabled — compiled out) |
| **Role** | Performance patch (pending) |

## Purpose

This file is wrapped in `#if DISABLED` and is not compiled. It is the second half of the intended "Lag on grid group changes" fix described in `Docs/PerformanceFixes.md`. The symptom is serious main-thread lag on connector lock/unlock and rotor head attach/detach, caused by `MyResourceDistributorComponent.UpdateBeforeSimulation` running synchronously on the main thread during every grid group transition.

The patch prefix checks the flags set by [`MyGroupsPatch.cs`](MyGroupsPatch.cs.md) (`IsInMergeGroups` or `IsInBreakLink`). If either is `true` and the `FixGridGroups` config flag is enabled, it skips `UpdateBeforeSimulation` and instead calls `__instance.MarkForUpdate()`, deferring the actual resource recomputation to a worker thread later. This allows the group-change bookkeeping on the main thread to complete quickly without paying the full cost of resource redistribution synchronously. The approach is disabled because [`MyGroupsPatch.cs`](MyGroupsPatch.cs.md) itself cannot be applied (Harmony cannot patch generic methods).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `UpdateBeforeSimulation` | `static bool` (Harmony Prefix) | Skips `UpdateBeforeSimulation` during group changes and calls `MarkForUpdate()` on the instance to schedule deferred processing. |

## Patch targets

> **Note:** This patch class is currently disabled (`#if DISABLED`) because [`MyGroupsPatch.cs`](MyGroupsPatch.cs.md) cannot be applied (Harmony cannot patch generic methods).

| Target | Patch | Effect |
| --- | --- | --- |
| `MyResourceDistributorComponent.UpdateBeforeSimulation` | Prefix | Skips the synchronous resource update during group changes; calls `MarkForUpdate()` to defer the work to a worker thread. |

## References

- [`MyGroupsPatch.cs`](MyGroupsPatch.cs.md) — provides the `IsInMergeGroups` / `IsInBreakLink` flags this patch depends on.
- [merge-and-paste](../../../../modules/merge-and-paste.md) — parent module.
- [`PatchHelpers.cs`](../PatchHelpers.cs.md) — would apply this patch via `harmony.PatchAll` if it were enabled.

---

*[Handbook](../../../../TOC.md) · [Module: Grid Merge & Paste Patches](../../../../modules/merge-and-paste.md) · [Index](../../../../Index.md)*
