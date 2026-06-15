# Keen Overhead Removal

Removes constant background overhead baked into the game: explicit `GC.Collect` pauses, the EOS P2P `UpdateStats` core load and the Mod API call-statistics rewriter.

This module targets three categories of constant, unconditional overhead baked into the game binary. Each fix is independently toggleable and operates without any shared state between the patches.

[`GcCollectPatch.cs`](../files/Shared/Patches/Bullshit/GcCollectPatch.cs.md) eliminates multi-second GC pauses during world load/unload and mid-gameplay stutters by removing explicit `GC.Collect` call sites from several game methods via IL transpilation. [`MyP2PQoSAdapterPatch.cs`](../files/Shared/Patches/Bullshit/MyP2PQoSAdapterPatch.cs.md) reduces the ~50% constant CPU core load from the EOS P2P statistics loop by rate-limiting it to run only once every 49 calls, sleeping 1 ms in between. [`PerfCountingRewriterPatch.cs`](../files/Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs.md) prevents the Roslyn mod compiler from injecting per-call API statistics counters into mod scripts, cutting simulation CPU load by ~10% in heavily modded worlds.

## Files

| File | Summary |
| --- | --- |
| [`GcCollectPatch.cs`](../files/Shared/Patches/Bullshit/GcCollectPatch.cs.md) | Transpiles multiple game methods to remove `GC.Collect` / `CollectGC` call sites, eliminating GC-induced pauses. |
| [`MyP2PQoSAdapterPatch.cs`](../files/Shared/Patches/Bullshit/MyP2PQoSAdapterPatch.cs.md) | Rate-limits `UpdateStats` to one real call in 49, inserting `Thread.Sleep(1)` for the rest to kill the constant EOS CPU load. |
| [`PerfCountingRewriterPatch.cs`](../files/Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs.md) | Short-circuits the Roslyn mod-script rewriter so API call-statistics instrumentation is never injected. |

## How it fits together

The three patches in this module are independent of each other; there is no shared state or call ordering between them.

[`GcCollectPatch.cs`](../files/Shared/Patches/Bullshit/GcCollectPatch.cs.md) uses Harmony's multi-target Transpiler pattern (`TargetMethods()` + `[HarmonyTranspiler]`): it yields the set of affected `MethodBase` objects at patch-apply time (gated on `Config.FixGarbageCollection`), and the same transpiler is applied to every one of them. The IL rewriting walks each method's instruction list, replaces `GC.Collect` and `IVRageSystem.CollectGC` call instructions with `Nop` + `Pop`, and records the before/after sequences via `RecordCustomCode` for [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md) tracking.

[`MyP2PQoSAdapterPatch.cs`](../files/Shared/Patches/Bullshit/MyP2PQoSAdapterPatch.cs.md) operates on a single background thread (the EOS stats thread) and uses a plain `static int counter` (not thread-local, since there is only one caller). The initial value of −300 provides a warm-up window before rate limiting activates, ensuring the stats system can initialise properly on startup.

[`PerfCountingRewriterPatch.cs`](../files/Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs.md) intercepts the Roslyn compilation pipeline at the point where mod syntax trees are rewritten. It activates when either `DisableModApiStatistics` or `CacheMods` is enabled, because cached compilation also benefits from skipping the rewrite step.

All three patches are applied by [patch-infrastructure](patch-infrastructure.md); none require `Configure()` or `PatchUpdates()` hooks. On the dedicated server `MyP2PQoSAdapterPatch` is deferred to the `"Late"` category and applied from `Init` (its EOS target assembly is not loaded at the early bootstrap), while `GcCollectPatch` and `PerfCountingRewriterPatch` are applied early with the rest; on the client all three are applied together via `harmony.PatchAll`.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
