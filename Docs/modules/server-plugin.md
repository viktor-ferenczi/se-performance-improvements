# Server Plugin Entry Point

The dedicated-server plugin entry point and its XML-serialized performance configuration, loaded by Magnetar / Quasar.

The server module is the Magnetar / Quasar entry point for the plugin. It owns two responsibilities: lifecycle management (early patch bootstrap, load, tick, dispose) and server-side configuration. Both responsibilities are kept small by delegating all patch logic to the [shared-plugin-core](shared-plugin-core.md) and [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md). The project also hosts a few **server-only** Harmony patches under `ServerPlugin/Patches/` (relocated out of `Shared/` so they compile only into the server build); these are documented with the subsystem they belong to in [simulation-and-blocks](simulation-and-blocks.md). The project also hosts [`PerformanceStats.cs`](../files/ServerPlugin/Stats/PerformanceStats.cs.md), the server's consumer for the shared runtime statistics: it maps each periodic [`StatisticsSnapshot.cs`](../files/Shared/Stats/StatisticsSnapshot.cs.md) onto the Magnetar PluginSdk statistics API so the Quasar Agent can collect and chart the plugin's cache hit rates and conveyor call counts.

[`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md) implements both the game's `IPlugin` contract and the shared [`ICommonPlugin.cs`](../files/Shared/Plugin/ICommonPlugin.cs.md) contract. Because the dedicated server loads the world — including mod and script compilation — before `IPlugin.Init`, the patching is bootstrapped early from [`Preloader.cs`](../files/ServerPlugin/Preloader.cs.md): its `Finish()` hook installs a Harmony postfix on `MyInitializer.InvokeBeforeRun`, which loads the config, calls [`Common.cs`](../files/Shared/Plugin/Common.cs.md).`SetPlugin` (with a stand-in plugin) and applies the uncategorized patches before compilation. `Init` then runs after world load: it attaches the live instance via `Common.AttachPlugin` and applies the deferred `"Late"` patch category. On every game tick it calls `PatchHelpers.PatchUpdates()` and increments `Tick`.

[`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md) uses PluginSdk attributes (`[Tab]`, `[Section]`, `[BoolOption]`, `[EnumOption]`, `[IntOption]`) to declare its Quasar UI automatically. The server defaults are conservative: fixes with gameplay side-effects (conveyor caching, access caching, PB access caching, LCD visibility, projected blocks) and the recently added ones which are not yet proven on production servers (asteroid voxel preload skip, target group refresh, memory statistics, ImageSharp upgrade) default to `false` and require the admin to opt in deliberately. The Havok thread count mode defaults to `Game` for the same reason, which leaves the sizing of the physics thread pool to the game.

## Files

| File | Summary |
| --- | --- |
| [`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md) | XML-serialized, Quasar-rendered config class with one toggle per performance fix, plus the Havok thread count mode and number |
| [`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md) | Dedicated-server plugin entry point: applies Harmony patches in two phases, loads config, drives the tick loop |
| [`Preloader.cs`](../files/ServerPlugin/Preloader.cs.md) | Namespace-less loader hook (called before the game starts) that installs the early Harmony bootstrap |
| [`PerformanceStats.cs`](../files/ServerPlugin/Stats/PerformanceStats.cs.md) | Publishes each runtime statistics snapshot through the PluginSdk statistics API under the `Performance` provider name |

## How it fits together

`Plugin.EarlyStartup` is the starting gun — reached from the [`Preloader.cs`](../files/ServerPlugin/Preloader.cs.md) bootstrap before world load (and again from `Init` as an idempotent fallback). It resolves the config path (case-insensitively via `PathResolver.Normalize`), loads `PerformanceConfig` via `ConfigStorage.LoadXml`, subscribes to `PropertyChanged` (so Quasar-pushed changes are immediately persisted), then calls `Common.SetPlugin(EarlyPlugin.Instance, gameVersion, storageDir)` with a lightweight stand-in plugin. That call propagates the stand-in's `Log` and `Config` — the `IPluginLogger`/`IPluginConfig` views — into the static [`Common.cs`](../files/Shared/Plugin/Common.cs.md) properties consumed by every patch class. When `Init` later runs, `Common.AttachPlugin(this)` swaps in the live `Plugin` instance so per-tick code reaches the real `Tick`.

Right after `SetPlugin`, `EarlyStartup` sets [`Statistics.cs`](../files/Shared/Stats/Statistics.cs.md)`.Publisher = PerformanceStats.Publish`. This is the only place in the plugin that knows the shared statistics pipeline ([shared-plugin-core](shared-plugin-core.md)) has a PluginSdk consumer: the shared code captures the counters once per `Statistics.PeriodTicks` from `PatchHelpers.PatchUpdates`, and [`PerformanceStats.cs`](../files/ServerPlugin/Stats/PerformanceStats.cs.md) turns the snapshot into `[Gauge]` / `[Counter]` rows for `PluginStats.Publish`. `Preloader.Finish` also calls [`ModRewriterVersions.cs`](../files/Shared/Tools/ModRewriterVersions.cs.md)`.Initialize()` before installing the bootstrap, so the mod-rewriting plugins are detected while still inside Magnetar's loader, where a failure terminates the loader instead of poisoning the compilation caches.

`PerformanceConfig` raises `PropertyChanged` on any setter call; `Plugin.OnConfigChanged` catches it, logs the property name via the PluginSdk logger, and calls `TrySaveConfig`. The PluginSdk logger (structured JSON under Quasar) is kept separate from the game's `MyLog` logger ([`PluginLogger.cs`](../files/Shared/Logging/PluginLogger.cs.md)) used by the patches.

At shutdown, `Plugin.Dispose` unhooks the `PropertyChanged` subscription and nulls `Instance`. Patches that survive into the next session (e.g. after a `/reload`) will re-read `Common.Config` which will be null until the next `Init` — the [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) guard handles this safely.

For cross-module interactions see [shared-plugin-core](shared-plugin-core.md) (receives `ICommonPlugin`), [logging](logging.md) (provides `IPluginLogger`), and [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) (receives the `Harmony` instance).

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
