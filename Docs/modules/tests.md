# Tests

Unit tests for the shared tooling.

The tests module contains xUnit unit tests for the shared tooling utilities. It has no dependency on the game engine, the plugin SDK, or any Harmony infrastructure, so the tests run on plain .NET without the game installed.

Currently the module covers [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md), the token-bucket primitive used by the log-flooding and frequency-limiting patches. Additional tool tests would live here as the shared tooling grows.

## Files

| File | Summary |
| --- | --- |
| [`RateLimiterTest.cs`](../files/Tests/Shared/Tools/RateLimiterTest.cs.md) | xUnit test verifying quota exhaustion, skip counting, and reset behaviour of [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md) |

## How it fits together

`RateLimiterTests` is a single xUnit test class with a single `[Fact]`. It is entirely self-contained: it instantiates [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md) directly and drives it through a quota cycle — consume all slots, verify the next call is suppressed, reset and verify the skipped count, verify the slot is refilled. There are no mocks or shared fixtures.

The test assembly references only `Shared.Tools` (for `RateLimiter`) and `xunit` — no game DLLs, no plugin SDK, no Harmony. This makes it runnable in a standard CI environment.

As the shared tooling expands (e.g. caches, timers, other rate-limiting variants) corresponding test classes should be added to this module. Interactions with other modules are minimal: the module tests [`RateLimiter.cs`](../files/Shared/Tools/RateLimiter.cs.md) which lives in the shared tools layer consumed by various patches.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
