# `ServerPlugin/Plugin.cs`

*Dedicated-server plugin entry point: applies the Harmony patches in two phases (uncategorized early via the Preloader bootstrap, the "Late" category from `Init`), loads config, and drives the per-tick update loop.*

|  |  |
| --- | --- |
| **Module** | [Server Plugin Entry Point](../../modules/server-plugin.md) |
| **Source** | [`Plugin.cs`](../../../ServerPlugin/Plugin.cs) (284 lines) |
| **Kind** | Class implementing `IPlugin`, `ICommonPlugin` |
| **Role** | Plugin entry point |

## Purpose

`Plugin` is the class Magnetar / Quasar instantiates when the plugin is loaded. It implements both the game's `IPlugin` contract and the shared [`ICommonPlugin.cs`](../Shared/Plugin/ICommonPlugin.cs.md) contract so [`Common.cs`](../Shared/Plugin/Common.cs.md) can hold a single typed reference used by all patch code.

On the dedicated server the world — including mod and in-game script compilation — loads *before* `IPlugin.Init`, so the patches that affect compilation (notably the script-compiler cache) must be applied earlier. The patching is therefore split into two phases under one Harmony id. The early phase is kicked off from the [`Preloader.cs`](Preloader.cs.md) `Finish()` hook, which calls `InstallEarlyBootstrap` to install a Harmony postfix (`OnGameInitialized`) on `MyInitializer.InvokeBeforeRun` — the earliest point where the game's filesystem (`MyFileSystem.UserDataPath`) and logging (`MyLog.Default`) are both ready, still well before world load. That postfix runs `EarlyStartup`.

`EarlyStartup` is one-shot (guarded by `earlyStarted`; main-thread, so a plain flag suffices). It calls `LoadConfig`, then [`Common.cs`](../Shared/Plugin/Common.cs.md).`SetPlugin` with the lightweight `EarlyPlugin` stand-in (so `Common.Plugin` is non-null for patches that read it during world load), force-loads `typeof(VRage.Scripting.MyScriptCompiler)` so the by-name target verification resolves, and applies the uncategorized patches via [`PatchHelpers.cs`](../Shared/Patches/PatchHelpers.cs.md).`HarmonyPatchUncategorized` — everything except the deferred `"Late"` category.

`Init` runs much later, after the world/session has loaded. It calls `EarlyStartup` again as an idempotent fallback (for the case the preloader path did not run, e.g. Magnetar safe mode — the cache will not have helped that load, but the runtime optimizations still apply), then [`Common.cs`](../Shared/Plugin/Common.cs.md).`AttachPlugin(this)` to swap the live instance in for the stand-in (so per-tick code reaches the real `Tick`), then [`PatchHelpers.cs`](../Shared/Patches/PatchHelpers.cs.md).`HarmonyPatchCategory` for the `"Late"` category, whose target assemblies (e.g. VRage.EOS) are loaded only by now.

The dual-logger design keeps lifecycle messages (load, save, errors) in the PluginSdk-structured log (`SdkLog` — the Magnetar game log when standalone, or JSON when managed by Quasar) while patch code writes to the game's `MyLog` via [`PluginLogger.cs`](../Shared/Logging/PluginLogger.cs.md). The early bootstrap necessarily logs via `SdkLog`, because it runs before `MyLog.Default` exists.

On each game `Update` call it delegates to `PatchHelpers.PatchUpdates()` and increments `Tick`. In release builds all update exceptions are caught and the plugin is disabled to avoid crashing the server; in debug builds exceptions propagate so the debugger can catch them.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Instance` | Static property | Singleton reference; set on `Init`, cleared on `Dispose`. |
| `Tick` | Property | Monotonically increasing update counter; read by rate-limited patches. |
| `Log` | Property | The shared [`IPluginLogger.cs`](../Shared/Logging/IPluginLogger.cs.md) forwarded to [`Common.cs`](../Shared/Plugin/Common.cs.md).`Logger`. |
| `Config` | Property | The [`PerformanceConfig.cs`](Config/PerformanceConfig.cs.md) instance forwarded to [`Common.cs`](../Shared/Plugin/Common.cs.md).`Config`. |
| `InstallEarlyBootstrap()` | Static method | Called from [`Preloader.cs`](Preloader.cs.md).`Finish()`. Installs a Harmony postfix (`OnGameInitialized`) on `MyInitializer.InvokeBeforeRun` under a separate `"{Name}.Bootstrap"` id. |
| `OnGameInitialized()` | Static method | The `InvokeBeforeRun` postfix; runs `EarlyStartup` (catching and recording failure). Not `[HarmonyPatch]`-decorated, so the patch scan never re-applies it. |
| `EarlyStartup()` | Static method | One-shot early init (guarded by `earlyStarted`): `LoadConfig`, `Common.SetPlugin` with the stand-in, force-load `MyScriptCompiler`, then `HarmonyPatchUncategorized` — all before world-load compilation. |
| `LoadConfig()` | Static method | Resolves the config path (`PathResolver.Normalize`), loads or default-constructs [`PerformanceConfig.cs`](Config/PerformanceConfig.cs.md), persists a sparse copy, subscribes to `PropertyChanged`. |
| `GetGameVersion()` | Static method | Builds the `APP_VERSION b<build>` string fed to the cache hash; degrades gracefully if `BasicGameInfo` is not populated. |
| `Init(object)` | Method | The `IPlugin` entry, after world load. Runs `EarlyStartup` as a fallback, `Common.AttachPlugin(this)`, then applies the `"Late"` patch category. |
| `Update()` | Method | Per-tick driver; calls `PatchHelpers.PatchUpdates()` and increments `Tick`. |
| `TrySaveConfig()` | Static method | Persists `PerformanceConfig` to disk; called on init and on every `PropertyChanged` event. |
| `OnConfigChanged` | Static method | `PropertyChanged` handler: logs the changed property name and calls `TrySaveConfig`. |
| `Dispose()` | Method | Unsubscribes from `PropertyChanged` and clears `Instance`. |
| `EarlyPlugin` | Nested class | Lightweight `ICommonPlugin` stand-in (`Tick => 0`) used during early bootstrap, before the live `Plugin` instance is attached. |

## References

- [`Preloader.cs`](Preloader.cs.md) — namespace-less loader hook whose `Finish()` triggers `InstallEarlyBootstrap`
- [`ICommonPlugin.cs`](../Shared/Plugin/ICommonPlugin.cs.md) — interface this class implements (shared contract with the client plugin)
- [`PerformanceConfig.cs`](Config/PerformanceConfig.cs.md) — the XML-serialized server configuration
- [`Common.cs`](../Shared/Plugin/Common.cs.md) — shared bootstrap; `SetPlugin` (early) and `AttachPlugin` (from `Init`)
- [`PatchHelpers.cs`](../Shared/Patches/PatchHelpers.cs.md) — applies the patches in two phases and drives per-tick updates
- [`PluginLogger.cs`](../Shared/Logging/PluginLogger.cs.md) — the `IPluginLogger` implementation used for patch-side logging

---

*[Handbook](../../TOC.md) · [Module: Server Plugin Entry Point](../../modules/server-plugin.md) · [Index](../../Index.md)*
