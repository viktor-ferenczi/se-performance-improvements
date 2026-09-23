# `Shared/Stats/Statistics.cs`

*Central switch and driver for the plugin's runtime statistics: gates collection on the `CollectStatistics` option, captures the patch and cache counters into a [`StatisticsSnapshot.cs`](StatisticsSnapshot.cs.md) once per period and hands it to the host's `Publisher`.*

|  |  |
| --- | --- |
| **Module** | [Shared Plugin Core](../../../modules/shared-plugin-core.md) |
| **Source** | [`Statistics.cs`](../../../../Shared/Stats/Statistics.cs) (87 lines) |
| **Kind** | Static class `Statistics` |
| **Role** | Statistics driver |

## Purpose

The counters themselves live in the patch classes and their caches: the [`UintCache.cs`](../Tools/UintCache.cs.md) instances created with `collectStats: true` and the conveyor [`PullItemStats.cs`](../Patches/Conveyor/PullItemStats.cs.md). `Statistics` decides whether they are collected at all, reads them into a host-agnostic snapshot on a fixed period and delivers that snapshot to whichever consumer the host installed. It deliberately has no dependency on the Magnetar PluginSdk, so it compiles into both plugins: the dedicated server sets `Publisher` to [`PerformanceStats.cs`](../../ServerPlugin/Stats/PerformanceStats.cs.md)`.Publish` from `EarlyStartup` in [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md), while the client leaves it null.

`Enabled` is a plain static field rather than a config lookup because it is read on every lookup of an opted-in cache (`UintCache.TryGetValue`) and on every patched `PullItem` / `PullItems` call. `RefreshEnabled` keeps it in sync with `Common.Config.CollectStatistics` through the config's `PropertyChanged` event and through the `Publisher` setter. In a release build the gate is also closed while no `Publisher` is set: without a consumer nothing would ever read the counters, so the client pays no collection overhead until it ships a PluginSdk consumer. A debug build collects whenever the option is on, since [`PatchHelpers.cs`](../Patches/PatchHelpers.cs.md) then logs every snapshot as well.

The period is driven by `PatchHelpers.PatchUpdates`: every `PeriodTicks` (600 simulation ticks, ten seconds at full simulation speed) it calls `Capture` and invokes `Publisher` with the result. `Capture` is the single reader of the counters. Each `CaptureStatistics` call it fans out to samples *and resets* that patch's counters, so the debug log and the publisher both consume the one returned snapshot and no counter is read twice per period.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `PeriodTicks` | `const int` | Capture period in simulation ticks (`10 * 60`). |
| `Enabled` | `static bool` field | Runtime collection gate read on the cache lookup hot path; mirrors `CollectStatistics` and, in release builds, whether a `Publisher` exists. |
| `Publisher` | `static Action<StatisticsSnapshot>` | Optional host consumer of each snapshot; the setter re-evaluates `Enabled`. The dedicated server sets it to `PerformanceStats.Publish`; null on the client. |
| `Configure()` | Static method | Subscribes to `Common.Config.PropertyChanged` (idempotently) and computes the initial `Enabled`. Called from `PatchHelpers.Configure`. |
| `RefreshEnabled()` | Private static method | Recomputes `Enabled` from the config option and, outside `DEBUG`, from `Publisher != null`. |
| `Capture()` | Static method | Creates a fresh `StatisticsSnapshot` and fills it via the `CaptureStatistics` methods of `MySafeZonePatch`, `MySessionComponentSafeZonesPatch`, `MyWindTurbinePatch` and `MyGridConveyorSystemPatch`, resetting their counters. |

## References

- [`StatisticsSnapshot.cs`](StatisticsSnapshot.cs.md) — the neutral shape `Capture` produces.
- [`PerformanceStats.cs`](../../ServerPlugin/Stats/PerformanceStats.cs.md) — the dedicated server's `Publisher`, mapping each snapshot onto the PluginSdk statistics API.
- [`PatchHelpers.cs`](../Patches/PatchHelpers.cs.md) — calls `Configure()` and drives the capture period from `PatchUpdates`.
- [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) — sets `Publisher` in `EarlyStartup`.
- [`IPluginConfig.cs`](../Config/IPluginConfig.cs.md) — declares the `CollectStatistics` option this class mirrors.
- [`UintCache.cs`](../Tools/UintCache.cs.md) — the opt-in (`collectStats: true`) cache whose lookups check `Enabled`.
- [`CacheStat.cs`](../Tools/CacheStat.cs.md) — the per-cache counter whose `Sample()` feeds each `CacheStatEntry`.
- [`MySafeZonePatch.cs`](../Patches/SafeZone/MySafeZonePatch.cs.md), [`MySessionComponentSafeZonesPatch.cs`](../Patches/SafeZone/MySessionComponentSafeZonesPatch.cs.md), [`MyWindTurbinePatch.cs`](../Patches/WindTurbine/MyWindTurbinePatch.cs.md), [`MyGridConveyorSystemPatch.cs`](../Patches/Conveyor/MyGridConveyorSystemPatch.cs.md) — the instrumented patches implementing `CaptureStatistics`.
- [shared-plugin-core](../../../modules/shared-plugin-core.md) — how the statistics pipeline fits the host-agnostic design.

---

*[Handbook](../../../TOC.md) · [Module: Shared Plugin Core](../../../modules/shared-plugin-core.md) · [Index](../../../Index.md)*
