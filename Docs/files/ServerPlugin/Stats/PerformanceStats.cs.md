# `ServerPlugin/Stats/PerformanceStats.cs`

*Publishes each captured [`StatisticsSnapshot.cs`](../../Shared/Stats/StatisticsSnapshot.cs.md) through the Magnetar PluginSdk statistics API under the `Performance` provider name, so the Quasar Agent can collect and chart the plugin's cache hit rates and conveyor call counts.*

|  |  |
| --- | --- |
| **Module** | [Server Plugin Entry Point](../../../modules/server-plugin.md) |
| **Source** | [`PerformanceStats.cs`](../../../../ServerPlugin/Stats/PerformanceStats.cs) (88 lines) |
| **Kind** | Static class `PerformanceStats` with two nested POCO row classes |
| **Role** | Statistics publisher (dedicated server) |

## Purpose

This is the dedicated server's consumer for the shared statistics pipeline. `EarlyStartup` in [`Plugin.cs`](../Plugin.cs.md) registers `Publish` as [`Statistics.cs`](../../Shared/Stats/Statistics.cs.md)`.Publisher`, so once per `Statistics.PeriodTicks` [`PatchHelpers.cs`](../../Shared/Patches/PatchHelpers.cs.md)`.PatchUpdates` calls it on the main thread with the snapshot the shared code just captured. `Publish` maps the neutral snapshot onto the PluginSdk's self-describing shapes: each `CacheStatEntry` becomes a `CacheStatsRow` labelled by cache name, and the two conveyor counters become a single, unlabelled `ConveyorPullRow`. Both row groups are captured against their `StatsSchema` and published as one `StatsSnapshot` via `PluginStats.Publish("Performance", ...)`.

The row classes *are* the schema. The `[StatLabel]`, `[Gauge]` and `[Counter]` attributes tell the PluginSdk how to name, type and aggregate each property: the hit rate is a gauge in percent averaged across cache instances, lookups and hits are counters, and the item count is a gauge. `StatsSchema.Build` reflects over a POCO type once and caches the result, so the two schemas are held in static fields and each publish only walks the cached property list.

The class depends only on `Shared.Stats` and the PluginSdk, nothing else in `ServerPlugin`. It lives here purely because the client does not ship the PluginSdk yet; when it does, the file can move into `Shared` verbatim and be wired up from the client the same way. Collection is controlled by the `CollectStatistics` option of [`PerformanceConfig.cs`](../Config/PerformanceConfig.cs.md).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `ProviderName` | `const string` | `"Performance"`, the provider name a consumer pulls the snapshot by. |
| `CacheSchema` / `PullSchema` | `static readonly StatsSchema` | Cached PluginSdk schemas for the two row types. |
| `Publish(StatisticsSnapshot)` | Static method | Maps the snapshot to rows, captures the two groups and calls `PluginStats.Publish`. Registered as `Statistics.Publisher`. |
| `CacheStatsRow` | Nested sealed class | One row per cache: `Name` (label), `HitRate` (gauge, %, mean across instances), `Lookups` and `Hits` (counters), `ItemCount` (gauge). |
| `ConveyorPullRow` | Nested sealed class | Single row with the `PullItem` and `PullItems` call counters. |

## References

- [`StatisticsSnapshot.cs`](../../Shared/Stats/StatisticsSnapshot.cs.md) — the host-agnostic input shape.
- [`Statistics.cs`](../../Shared/Stats/Statistics.cs.md) — the driver that captures snapshots and calls this publisher.
- [`Plugin.cs`](../Plugin.cs.md) — `EarlyStartup` registers `Publish` as the publisher.
- [`PatchHelpers.cs`](../../Shared/Patches/PatchHelpers.cs.md) — `PatchUpdates` drives the capture period.
- [`PerformanceConfig.cs`](../Config/PerformanceConfig.cs.md) — the `CollectStatistics` toggle.
- [shared-plugin-core](../../../modules/shared-plugin-core.md) — the shared side of the statistics pipeline.

---

*[Handbook](../../../TOC.md) · [Module: Server Plugin Entry Point](../../../modules/server-plugin.md) · [Index](../../../Index.md)*
