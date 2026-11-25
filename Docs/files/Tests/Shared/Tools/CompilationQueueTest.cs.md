# `Tests/Shared/Tools/CompilationQueueTest.cs`

*Exercises the shared worker limit, fault and cancellation handling, and session shutdown.*

|  |  |
| --- | --- |
| **Module** | [Tests](../../../../modules/tests.md) |
| **Source** | [`CompilationQueueTest.cs`](../../../../../Tests/Shared/Tools/CompilationQueueTest.cs) (132 lines) |
| **Kind** | xUnit test class |
| **Role** | Unit test |

## Purpose

Mixed mod and programmable-block requests compete for the same capacity at limits of one, two and four. A controlled task holds the occupied slots while the test checks the waiting requests.

## Key members

`MixedRequestsShareOneLimit` checks worker admission. The remaining cases check configuration bounds, releasing a slot after a fault or cancellation, and draining the queue during session unload.

## References

[Compilation settings](../../../../PerformanceFixes.md#caching-compiled-mods-and-in-game-scripts)

---

*[Handbook](../../../../TOC.md) · [Module: Tests](../../../../modules/tests.md) · [Index](../../../../Index.md)*
