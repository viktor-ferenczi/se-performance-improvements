# Keen Overhead Removal

Removes constant background overhead baked into the game: explicit `GC.Collect` pauses, the per-frame process memory read and the Mod API call-statistics rewriter.

This module targets several kinds of constant, unconditional overhead baked into the game binary. Each fix is independently toggleable and operates without any shared state between the patches.

[`GcCollectPatch.cs`](../files/Shared/Patches/Bullshit/GcCollectPatch.cs.md) eliminates multi-second GC pauses during world load/unload and mid-gameplay stutters by removing explicit `GC.Collect` call sites from several game methods via IL transpilation. [`PerfCountingRewriterPatch.cs`](../files/Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs.md) prevents the Roslyn mod compiler from injecting per-call API statistics counters into mod scripts, cutting simulation CPU load by ~10% in heavily modded worlds.

## Files

| File | Summary |
| --- | --- |
| [`GcCollectPatch.cs`](../files/Shared/Patches/Bullshit/GcCollectPatch.cs.md) | Transpiles multiple game methods to remove `GC.Collect` / `CollectGC` call sites, eliminating GC-induced pauses. |
| [`MyWindowsSystemPatchForStats.cs`](../files/Shared/Patches/Bullshit/MyWindowsSystemPatchForStats.cs.md) | Serves `ProcessPrivateMemory` from a cache refreshed once per second, so the per-frame statistics update stops reading procfs on Linux. |
| [`PerfCountingRewriterPatch.cs`](../files/Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs.md) | Short-circuits the Roslyn mod-script rewriter so API call-statistics instrumentation is never injected. |

## How it fits together

The patches in this module are independent of each other; there is no shared state or call ordering between them.

[`GcCollectPatch.cs`](../files/Shared/Patches/Bullshit/GcCollectPatch.cs.md) uses Harmony's multi-target Transpiler pattern (`TargetMethods()` + `[HarmonyTranspiler]`): it yields the set of affected `MethodBase` objects at patch-apply time (gated on `Config.FixGarbageCollection`), and the same transpiler is applied to every one of them. The IL rewriting walks each method's instruction list, replaces `GC.Collect` and `IVRageSystem.CollectGC` call instructions with `Nop` + `Pop`, and records the before/after sequences via `RecordCustomCode` for [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md) tracking.

[`PerfCountingRewriterPatch.cs`](../files/Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs.md) intercepts the Roslyn compilation pipeline at the point where mod syntax trees are rewritten. It activates when either `DisableModApiStatistics` or `CacheMods` is enabled, because cached compilation also benefits from skipping the rewrite step.

The patches are applied by [patch-infrastructure](patch-infrastructure.md) with the uncategorized ones.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
