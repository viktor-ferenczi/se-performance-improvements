# `Shared/Patches/ScriptCompiler/CompilationWorkers.cs`

*Queues mod and programmable-block compilations on the same set of workers.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`CompilationWorkers.cs`](../../../../../Shared/Patches/ScriptCompiler/CompilationWorkers.cs) (55 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

The Harmony prefix on `MyVRageScriptingInternal.CompileAsync` snapshots an eligible request before handing it to the process-wide queue. Workers call the original patched compiler, including its disk cache and other plugins’ compiler hooks.

## Key members

`Prefix` captures sources and a fresh diagnostics list. `WaitForIdle` drains work before a session changes its compiler settings.

## References

[Compilation settings](../../../../PerformanceFixes.md#caching-compiled-mods-and-in-game-scripts)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
