# `Tests/Shared/Tools/RateLimiterTest.cs`

*xUnit test verifying the core quota-and-reset behaviour of [`RateLimiter.cs`](../../../Shared/Tools/RateLimiter.cs.md).*

|  |  |
| --- | --- |
| **Module** | [Tests](../../../../modules/tests.md) |
| **Source** | [`RateLimiterTest.cs`](../../../../../Tests/Shared/Tools/RateLimiterTest.cs) (22 lines) |
| **Kind** | xUnit test class |
| **Role** | Unit test |

## Purpose

`RateLimiterTests` exercises the fundamental contract of [`RateLimiter.cs`](../../../Shared/Tools/RateLimiter.cs.md): that `Check()` returns `true` exactly `quota` times before returning `false`, that `Reset()` returns the count of skipped (suppressed) calls and restores the quota, and that `Check()` succeeds again immediately after a reset.

The single test `RateLimiter_RateLimit` constructs a `RateLimiter(3)`, exhausts the three allowed slots, asserts the next `Check()` is `false`, calls `Reset()` and asserts it returns `1` (one suppressed call), then verifies one more `Check()` succeeds.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `RateLimiter_RateLimit` | xUnit `[Fact]` | Verifies quota exhaustion, skip counting, reset, and re-admission in sequence. |

## References

- [`RateLimiter.cs`](../../../Shared/Tools/RateLimiter.cs.md) — the production class under test

---

*[Handbook](../../../../TOC.md) · [Module: Tests](../../../../modules/tests.md) · [Index](../../../../Index.md)*
