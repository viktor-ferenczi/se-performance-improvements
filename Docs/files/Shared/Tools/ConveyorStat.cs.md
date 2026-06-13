# `Shared/Tools/ConveyorStat.cs`

*Non-thread-safe call-count and failure-rate accumulator for the conveyor reachability cache.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`ConveyorStat.cs`](../../../../Shared/Tools/ConveyorStat.cs) (78 lines) |
| **Kind** | Class `ConveyorStat` |
| **Role** | Diagnostics helper |

## Purpose

`ConveyorStat` records how often `MyGridConveyorSystem.Reachable` is called and how many of those calls fail to obtain a lock on the conveyor graph — the two metrics most useful for diagnosing the conveyor reachability cache (see *Cached MyGridConveyorSystem.Reachable* in `Docs/PerformanceFixes.md`).

Like [`CacheStat.cs`](CacheStat.cs.md) it is intentionally not thread-safe: approximate counts are sufficient for diagnostic reporting, and avoiding synchronisation keeps the hot path overhead negligible. `FullReport` and `CountReport` both consume and reset the counters and express the rate normalised to per-second values based on the reporting period in ticks.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `CountCall()` | Method | Increments the call counter on every `Reachable` invocation. |
| `CountFailure()` | Method | Increments the failure counter when a lock cannot be acquired. |
| `AddRange(stats)` | Method | Merges a collection of per-grid `ConveyorStat` instances for aggregate reporting. |
| `FullReport(period)` | Method | Returns `Count = … (/s); Failed = … (%)` and resets counters. |
| `CountReport(period)` | Method | Returns only the call count and rate (no failure info) and resets counters. |

## References

- [`CacheStat.cs`](CacheStat.cs.md) — analogous hit-rate statistics class for generic caches.
- [`Cache.cs`](Cache.cs.md) — general-purpose cache whose hit-rate is tracked by `CacheStat`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
