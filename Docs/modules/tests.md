# Tests

Unit tests for the shared tooling.

The project links shared source files into an xUnit test assembly. The compilation queue and rate limiter tests run without game binaries. Other tests use Harmony, Cecil, Roslyn or game references to exercise script rewriting and compatibility.

## Files

| File | Summary |
| --- | --- |
| [`RateLimiterTest.cs`](../files/Tests/Shared/Tools/RateLimiterTest.cs.md) | xUnit test verifying quota exhaustion, skip counting, and reset behaviour of [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md) |
| [`CompilationQueueTest.cs`](../files/Tests/Shared/Tools/CompilationQueueTest.cs.md) | Exercises the shared worker limit, fault and cancellation handling, and session shutdown. |

## How it fits together

The compilation queue tests hold active workers with a controlled task, submit excess requests and release the workers after checking the shared limit. They also cover configuration bounds, faults, cancellation and session shutdown. The rate limiter test drives a complete quota cycle.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
