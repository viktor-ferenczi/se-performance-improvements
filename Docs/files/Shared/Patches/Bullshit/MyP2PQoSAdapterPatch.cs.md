# `Shared/Patches/Bullshit/MyP2PQoSAdapterPatch.cs`

*Rate-limits `VRage.EOS.MyP2PQoSAdapter.UpdateStats` by skipping 48 out of every 49 calls and sleeping for 1 ms instead, eliminating its ~50% constant CPU core load.*

|  |  |
| --- | --- |
| **Module** | [Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) |
| **Source** | [`MyP2PQoSAdapterPatch.cs`](../../../../../Shared/Patches/Bullshit/MyP2PQoSAdapterPatch.cs) (37 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

As described in the "EOS P2P UpdateStats" section of `Docs/PerformanceFixes.md`, `VRage.EOS.MyP2PQoSAdapter.UpdateStats` is called in a tight loop on its own dedicated thread and consumes approximately 50% of a CPU core continuously — even in offline single-player sessions. The fix eliminates 98% of that overhead.

The Prefix uses a static `counter` variable (intentionally non-thread-local, since the method runs on a single dedicated thread). On every call the counter is incremented; when it exceeds 47 it resets to 0 and lets the real method run (`return true`). For all other calls it executes `Thread.Sleep(1)` and returns `false` to skip the original. The initial value of `counter` is `−5*60 = −300`, which grants a warm-up period of 300 calls (~5 minutes at game start) during which the original method always runs. The `[HarmonyPatch]` class-level attribute has no type argument; the target is specified by a string name (`"VRage.EOS.MyP2PQoSAdapter"`, `"UpdateStats"`) because the EOS type is not directly referenceable.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `counter` | `static int` (private) | Call counter; starts at −300 to allow a warm-up period before rate limiting begins. Resets to 0 every 49 calls. |
| `UpdateStatsPrefix()` | `static bool` (Harmony Prefix) | Returns `true` (runs original) once every 49 calls; otherwise sleeps 1 ms and returns `false`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `VRage.EOS.MyP2PQoSAdapter.UpdateStats` | Prefix | Skips 48 out of 49 calls and inserts a 1 ms sleep, reducing the dedicated-thread CPU load from ~50% of a core to under 2%. |

## References

- [keen-overhead-removal](../../../../modules/keen-overhead-removal.md) — parent module.
- [`PatchHelpers.cs`](../PatchHelpers.cs.md) — applies this patch via `harmony.PatchAll`.

---

*[Handbook](../../../../TOC.md) · [Module: Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) · [Index](../../../../Index.md)*
