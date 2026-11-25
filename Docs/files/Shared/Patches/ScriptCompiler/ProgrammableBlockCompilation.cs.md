# `Shared/Patches/ScriptCompiler/ProgrammableBlockCompilation.cs`

*Submits live programmable-block compiles without waiting on the simulation thread.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`ProgrammableBlockCompilation.cs`](../../../../../Shared/Patches/ScriptCompiler/ProgrammableBlockCompilation.cs) (197 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

A source change supersedes the pending result for that block. Repeated requests for the same source reuse the pending compilation. A successful result activates on the game thread after the block and session are checked again. Compilation failure writes the diagnostics into the terminal.

## Key members

`Recompile` coalesces duplicate requests at the server event handler. `ExecuteCode` skips a run while the block is waiting for its assembly. `Compile` submits the current program and schedules completion. `Forget` and `Terminated` invalidate pending results when a block closes or terminates.

## References

[Compilation settings](../../../../PerformanceFixes.md#caching-compiled-mods-and-in-game-scripts)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
