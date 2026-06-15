# `Shared/Tools/RateLimiter.cs`

*Simple token-bucket rate limiter that caps the number of log messages (or any action) permitted per reporting period.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`RateLimiter.cs`](../../../../Shared/Tools/RateLimiter.cs) (34 lines) |
| **Kind** | Class `RateLimiter` |
| **Role** | Misc utility |

## Purpose

`RateLimiter` guards hot-path logging that would otherwise flood the log file. The primary use case is the "Rate limited excessive logging" fix (see `Docs/PerformanceFixes.md`): `MyDefinitionManager.GetBlueprintDefinition` was generating ~11 000 "No blueprint with Id" log messages per minute from certain PB scripts, risking log file bloat and extra CPU load.

The limiter holds a `quota` counter. Each call to `Check()` returns `true` (allow) while the remaining quota is positive and `false` (suppress) when exhausted, incrementing `skipped` in the latter case. `Reset()` refills the quota and returns the number of skipped messages since the last reset, which the caller typically appends to a "… and N more suppressed" log line.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `RateLimiter(quota)` | Constructor | Sets the maximum number of allowed calls per period. |
| `Check()` | Method | Returns `true` and decrements the quota if calls remain; returns `false` and increments `skipped` otherwise. |
| `Reset()` | Method | Refills the quota and returns the number of suppressed calls accumulated since the last reset. |

## References

- None.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
