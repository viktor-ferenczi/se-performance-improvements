# `Shared/Patches/ScriptCompiler/ModCompilation.cs`

*Batches mod compilation at world startup and registers completed assemblies in discovery order.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`ModCompilation.cs`](../../../../../Shared/Patches/ScriptCompiler/ModCompilation.cs) (172 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

The loader can discover the next mod while the preceding one compiles. Before the game constructs factories, it waits for each result in discovery order and registers its assembly or diagnostics. Additional loader passes register inside their original compiler-symbol scope.

## Key members

`Defer` keeps each request’s context and diagnostics. `Register` drains the batch before factory initialization. `Unload` cancels waiting jobs and waits for active ones.

## References

[Compilation settings](../../../../PerformanceFixes.md#caching-compiled-mods-and-in-game-scripts)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
