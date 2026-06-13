# `Shared/Patches/Bullshit/GcCollectPatch.cs`

*Removes explicit `GC.Collect` and `IVRageSystem.CollectGC` call sites from several game methods to eliminate multi-second GC pauses during world load, unload and gameplay.*

|  |  |
| --- | --- |
| **Module** | [Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) |
| **Source** | [`GcCollectPatch.cs`](../../../../../Shared/Patches/Bullshit/GcCollectPatch.cs) (109 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

As described in the "GC.Collect calls" section of `Docs/PerformanceFixes.md`, the game makes explicit calls to `GC.Collect` in several methods. These force a synchronous, full-generation garbage collection that can pause the process for seconds, particularly during large world loads and unloads. There are also mid-gameplay call sites (e.g. in `MyPlanetTextureMapProvider` and `MySimpleProfiler.LogPerformanceTestResults`) that can cause stutters. The fix was originally contributed by zznty.

Rather than patching each method individually with a Prefix that runs at call sites, this patch uses a multi-target Transpiler: `TargetMethods()` yields each affected `MethodBase` (gated on `Config.FixGarbageCollection`), and `CollectRemovalTranspiler` rewrites the IL of each by replacing every `GC.Collect` call and every `IVRageSystem.CollectGC` virtual call with `Nop` + enough `Pop` instructions to discard the arguments from the stack. The patched and original instruction sequences are recorded via `RecordCustomCode` for [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md) tracking. A warning is logged if a targeted method contains no `GC.Collect` calls (indicating the game has changed).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `TargetMethods()` | `IEnumerable<MethodBase>` | Yields the game methods to transpile: `MyPlanetTextureMapProvider.GetHeightmap/GetDetailMap/GetMaps`, `MySession.Unload`, `HkBaseSystem.Quit`, `MySimpleProfiler.LogPerformanceTestResults`, and the `MySession` constructor. Returns nothing if `FixGarbageCollection` is disabled. |
| `CollectRemovalTranspiler` | `IEnumerable<CodeInstruction>` (Transpiler) | Iterates the IL, replaces each `GC.Collect` / `CollectGC` call-site with `Nop` + `Pop`s, records both versions for [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md), and warns if no replacements were made. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyPlanetTextureMapProvider.GetHeightmap` | Transpiler | Removes `GC.Collect` call(s) from the method body. |
| `MyPlanetTextureMapProvider.GetDetailMap` | Transpiler | Removes `GC.Collect` call(s) from the method body. |
| `MyPlanetTextureMapProvider.GetMaps` | Transpiler | Removes `GC.Collect` call(s) from the method body. |
| `MySession.Unload` | Transpiler | Removes `GC.Collect` / `CollectGC` call(s) to prevent GC pause on world unload. |
| `HkBaseSystem.Quit` | Transpiler | Removes `GC.Collect` call(s) from Havok shutdown. |
| `MySimpleProfiler.LogPerformanceTestResults` | Transpiler | Removes `GC.Collect` call(s) that could stutter during gameplay. |
| `MySession..ctor` | Transpiler | Removes `GC.Collect` call(s) from the session constructor (world load). |

## References

- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md) — used via `RecordCustomCode` to track the original and patched IL sequences.
- [keen-overhead-removal](../../../../modules/keen-overhead-removal.md) — parent module.
- [`PatchHelpers.cs`](../PatchHelpers.cs.md) — applies this patch via `harmony.PatchAll`.

---

*[Handbook](../../../../TOC.md) · [Module: Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) · [Index](../../../../Index.md)*
