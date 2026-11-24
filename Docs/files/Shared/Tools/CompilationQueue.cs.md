# `Shared/Tools/CompilationQueue.cs`

*Bounds concurrent asynchronous work and admits waiting requests in FIFO order.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`CompilationQueue.cs`](../../../../Shared/Tools/CompilationQueue.cs) (85 lines) |
| **Kind** | Generic queue |
| **Role** | Shared utility |

## Purpose

An admitted operation holds a slot through all its asynchronous continuations. Requests waiting for capacity consume no worker thread.

## Key members

`Submit` returns a task for one operation. `WaitForIdle` can cancel queued operations and waits for active operations to finish.

## References

[Compilation settings](../../../PerformanceFixes.md#caching-compiled-mods-and-in-game-scripts)

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
