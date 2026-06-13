# Server Plugin Entry Point

The dedicated-server plugin entry point and its XML-serialized performance configuration, loaded by Magnetar / Quasar.

The server module is the Magnetar / Quasar entry point for the plugin. It owns two responsibilities: lifecycle management (load, tick, dispose) and server-side configuration. Both responsibilities are kept small by delegating all patch logic to the [shared-plugin-core](shared-plugin-core.md) and [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md). The project also hosts a few **server-only** Harmony patches under `ServerPlugin/Patches/` (relocated out of `Shared/` so they compile only into the server build); these are documented with the subsystem they belong to in [simulation-and-blocks](simulation-and-blocks.md).

[`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md) implements both the game's `IPlugin` contract and the shared [`ICommonPlugin.cs`](../files/Shared/Plugin/ICommonPlugin.cs.md) contract. On startup it loads or default-constructs a [`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md), persists a sparse copy to disk, then calls [`Common.cs`](../files/Shared/Plugin/Common.cs.md).`SetPlugin` which triggers patch registration. On every game tick it calls `PatchHelpers.PatchUpdates()` and increments `Tick`.

[`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md) uses PluginSdk attributes (`[Tab]`, `[Section]`, `[BoolOption]`) to declare its Quasar UI automatically. The server defaults are conservative: fixes with gameplay side-effects (conveyor caching, access caching, LCD visibility, log rate limiting, projected blocks) default to `false` and require the admin to opt in deliberately.

## Files

| File | Summary |
| --- | --- |
| [`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md) | XML-serialized, Quasar-rendered config class with one boolean toggle per performance fix |
| [`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md) | Dedicated-server plugin entry point: loads config, applies Harmony patches, drives the tick loop |

## How it fits together

`Plugin.Init` is the starting gun. It resolves the config path (case-insensitively via `PathResolver.Normalize`), loads `PerformanceConfig` via `ConfigStorage.LoadXml`, subscribes to `PropertyChanged` (so Quasar-pushed changes are immediately persisted), then calls `Common.SetPlugin(this, gameVersion, storageDir)`. That call propagates `this.Log` and `this.Config` — the two `IPluginConfig`/`IPluginLogger` views — into the static [`Common.cs`](../files/Shared/Plugin/Common.cs.md) properties consumed by every patch class.

`PerformanceConfig` raises `PropertyChanged` on any setter call; `Plugin.OnConfigChanged` catches it, logs the property name via the PluginSdk logger, and calls `TrySaveConfig`. The PluginSdk logger (structured JSON under Quasar) is kept separate from the game's `MyLog` logger ([`PluginLogger.cs`](../files/Shared/Logging/PluginLogger.cs.md)) used by the patches.

At shutdown, `Plugin.Dispose` unhooks the `PropertyChanged` subscription and nulls `Instance`. Patches that survive into the next session (e.g. after a `/reload`) will re-read `Common.Config` which will be null until the next `Init` — the [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) guard handles this safely.

For cross-module interactions see [shared-plugin-core](shared-plugin-core.md) (receives `ICommonPlugin`), [logging](logging.md) (provides `IPluginLogger`), and [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) (receives the `Harmony` instance).

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
