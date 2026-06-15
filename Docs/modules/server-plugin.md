# Server Plugin Entry Point

The dedicated-server plugin entry point and its XML-serialized performance configuration, loaded by Magnetar / Quasar.

The server module is the Magnetar / Quasar entry point for the plugin. It owns two responsibilities: lifecycle management (early patch bootstrap, load, tick, dispose) and server-side configuration. Both responsibilities are kept small by delegating all patch logic to the [shared-plugin-core](shared-plugin-core.md) and [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md). The project also hosts a few **server-only** Harmony patches under `ServerPlugin/Patches/` (relocated out of `Shared/` so they compile only into the server build); these are documented with the subsystem they belong to in [simulation-and-blocks](simulation-and-blocks.md).

[`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md) implements both the game's `IPlugin` contract and the shared [`ICommonPlugin.cs`](../files/Shared/Plugin/ICommonPlugin.cs.md) contract. Because the dedicated server loads the world — including mod and script compilation — before `IPlugin.Init`, the patching is bootstrapped early from [`Preloader.cs`](../files/ServerPlugin/Preloader.cs.md): its `Finish()` hook installs a Harmony postfix on `MyInitializer.InvokeBeforeRun`, which loads the config, calls [`Common.cs`](../files/Shared/Plugin/Common.cs.md).`SetPlugin` (with a stand-in plugin) and applies the uncategorized patches before compilation. `Init` then runs after world load: it attaches the live instance via `Common.AttachPlugin` and applies the deferred `"Late"` patch category. On every game tick it calls `PatchHelpers.PatchUpdates()` and increments `Tick`.

[`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md) uses PluginSdk attributes (`[Tab]`, `[Section]`, `[BoolOption]`) to declare its Quasar UI automatically. The server defaults are conservative: fixes with gameplay side-effects (conveyor caching, access caching, LCD visibility, log rate limiting, projected blocks) default to `false` and require the admin to opt in deliberately.

## Files

| File | Summary |
| --- | --- |
| [`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md) | XML-serialized, Quasar-rendered config class with one boolean toggle per performance fix |
| [`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md) | Dedicated-server plugin entry point: applies Harmony patches in two phases, loads config, drives the tick loop |
| [`Preloader.cs`](../files/ServerPlugin/Preloader.cs.md) | Namespace-less loader hook (called before the game starts) that installs the early Harmony bootstrap |

## How it fits together

`Plugin.EarlyStartup` is the starting gun — reached from the [`Preloader.cs`](../files/ServerPlugin/Preloader.cs.md) bootstrap before world load (and again from `Init` as an idempotent fallback). It resolves the config path (case-insensitively via `PathResolver.Normalize`), loads `PerformanceConfig` via `ConfigStorage.LoadXml`, subscribes to `PropertyChanged` (so Quasar-pushed changes are immediately persisted), then calls `Common.SetPlugin(EarlyPlugin.Instance, gameVersion, storageDir)` with a lightweight stand-in plugin. That call propagates the stand-in's `Log` and `Config` — the `IPluginLogger`/`IPluginConfig` views — into the static [`Common.cs`](../files/Shared/Plugin/Common.cs.md) properties consumed by every patch class. When `Init` later runs, `Common.AttachPlugin(this)` swaps in the live `Plugin` instance so per-tick code reaches the real `Tick`.

`PerformanceConfig` raises `PropertyChanged` on any setter call; `Plugin.OnConfigChanged` catches it, logs the property name via the PluginSdk logger, and calls `TrySaveConfig`. The PluginSdk logger (structured JSON under Quasar) is kept separate from the game's `MyLog` logger ([`PluginLogger.cs`](../files/Shared/Logging/PluginLogger.cs.md)) used by the patches.

At shutdown, `Plugin.Dispose` unhooks the `PropertyChanged` subscription and nulls `Instance`. Patches that survive into the next session (e.g. after a `/reload`) will re-read `Common.Config` which will be null until the next `Init` — the [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) guard handles this safely.

For cross-module interactions see [shared-plugin-core](shared-plugin-core.md) (receives `ICommonPlugin`), [logging](logging.md) (provides `IPluginLogger`), and [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) (receives the `Harmony` instance).

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
