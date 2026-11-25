# `Shared/Patches/ScriptCompiler/EditorCompilation.cs`

*Checks programmable-block editor code through the shared worker queue.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`EditorCompilation.cs`](../../../../../Shared/Patches/ScriptCompiler/EditorCompilation.cs) (145 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

The local editor keeps its progress screen while compilation runs. A remote check captures its sender before leaving the game thread so the server can return the correct player’s diagnostics.

## Key members

`CheckLocal` and `CheckRemote` handle the two requests. `Check` invokes the game-thread completion with diagnostics and success or failure.

## References

[Compilation settings](../../../../PerformanceFixes.md#caching-compiled-mods-and-in-game-scripts)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
