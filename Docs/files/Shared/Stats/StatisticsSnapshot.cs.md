# `Shared/Stats/StatisticsSnapshot.cs`

*Host-agnostic, point-in-time capture of the plugin's runtime statistics: one `CacheStatEntry` per instrumented cache plus the conveyor `PullItem` / `PullItems` call counts for the period.*

|  |  |
| --- | --- |
| **Module** | [Shared Plugin Core](../../../modules/shared-plugin-core.md) |
| **Source** | [`StatisticsSnapshot.cs`](../../../../Shared/Stats/StatisticsSnapshot.cs) (41 lines) |
| **Kind** | Sealed class `StatisticsSnapshot` and readonly struct `CacheStatEntry` |
| **Role** | Statistics data transfer object |

## Purpose

`StatisticsSnapshot` is the only thing that crosses the boundary between the shared collection code and a host-specific consumer. [`Statistics.cs`](Statistics.cs.md)`.Capture` creates one per period and the instrumented patches fill it; the dedicated server's [`PerformanceStats.cs`](../../ServerPlugin/Stats/PerformanceStats.cs.md) then maps it onto the Magnetar PluginSdk `StatsSnapshot` shapes. The class deliberately depends on nothing but the BCL. Keeping the PluginSdk out of `Shared` is what lets the same collection code compile into the client, and it means the server-side consumer can move into `Shared` unchanged once the client also ships the PluginSdk.

`CacheStatEntry` is the per-cache record: a name such as `SafeZone.IsSafe` or `Conveyor.Reachable`, the lookups and hits counted over the period and the current item count. It is built from a [`CacheStat.cs`](../Tools/CacheStat.cs.md)`.Sample()` (or, for the conveyor reachability caches, from the sum over all per-group caches in [`MyGridConveyorSystemPatch.cs`](../Patches/Conveyor/MyGridConveyorSystemPatch.cs.md)) and computes the hit rate on demand. A cache with no lookups in the period reports 100 % rather than dividing by zero, matching `CacheStatSample`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Caches` | `List<CacheStatEntry>` | One entry per instrumented cache, appended by each patch's `CaptureStatistics`. |
| `PullItem` / `PullItems` | `long` fields | `MyGridConveyorSystem.PullItem` / `PullItems` call counts over the period, taken from [`PullItemStats.cs`](../Patches/Conveyor/PullItemStats.cs.md). |
| `CacheStatEntry` | `readonly struct` | `Name`, `Lookups`, `Hits` and `Size` of one cache for one period. |
| `CacheStatEntry.HitRatePercent` | Property | `100 * Hits / Lookups`, or 100 when there were no lookups. |

## References

- [`Statistics.cs`](Statistics.cs.md) — creates and fills the snapshot once per period.
- [`PerformanceStats.cs`](../../ServerPlugin/Stats/PerformanceStats.cs.md) — the dedicated server's consumer, mapping it onto PluginSdk rows.
- [`CacheStat.cs`](../Tools/CacheStat.cs.md) — `CacheStatSample`, the per-cache counters each `CacheStatEntry` is built from.
- [`PullItemStats.cs`](../Patches/Conveyor/PullItemStats.cs.md) — source of the two conveyor call counts.
- [`MyGridConveyorSystemPatch.cs`](../Patches/Conveyor/MyGridConveyorSystemPatch.cs.md) — aggregates the per-group reachability caches into the `Conveyor.Reachable` entry.

---

*[Handbook](../../../TOC.md) · [Module: Shared Plugin Core](../../../modules/shared-plugin-core.md) · [Index](../../../Index.md)*
