# `Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs`

*Disables `VRage.Scripting.Rewriters.PerfCountingRewriter.Rewrite` so mod Roslyn syntax trees are returned unchanged, removing the Mod API call-statistics instrumentation overhead.*

|  |  |
| --- | --- |
| **Module** | [Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) |
| **Source** | [`PerfCountingRewriterPatch.cs`](../../../../../Shared/Patches/Bullshit/PerfCountingRewriterPatch.cs) (40 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

As described in the "Mod API call statistics overhead" section of `Docs/PerformanceFixes.md`, `VRage.Scripting.Rewriters.PerfCountingRewriter.Rewrite` injects instrumentation code into the Roslyn syntax trees of every compiled mod script in order to gather per-call API statistics. This rewriting adds overhead that can become a significant performance hog when many mods are loaded. The fix was originally contributed by zznty and has been measured to reduce simulation CPU load by ~10% in heavily modded worlds.

The Prefix intercepts the `Rewrite` call. When either `DisableModApiStatistics` or `CacheMods` is enabled (and the plugin itself is enabled), the prefix sets `__result = syntaxTree` (passing the input tree through unchanged) and returns `false` to skip the original rewriter. This means API calls in mod code are never instrumented, removing the overhead entirely. A debug log message is emitted in DEBUG builds to confirm whether rewriting is skipped or kept. The target type name is given as a string because `PerfCountingRewriter` is not publicly accessible.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `RewritePrefix` | `static bool` (Harmony Prefix) | Returns `false` and sets `__result = syntaxTree` (no-op rewrite) when the fix is enabled; otherwise returns `true` to let the original rewriter run. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `VRage.Scripting.Rewriters.PerfCountingRewriter.Rewrite` | Prefix | Bypasses the mod syntax-tree rewriter, returning the original tree unchanged and removing API call-statistics instrumentation from all mod scripts. |

## References

- [keen-overhead-removal](../../../../modules/keen-overhead-removal.md) — parent module.
- [`PatchHelpers.cs`](../PatchHelpers.cs.md) — applies this patch via `harmony.PatchAll`.

---

*[Handbook](../../../../TOC.md) · [Module: Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) · [Index](../../../../Index.md)*
