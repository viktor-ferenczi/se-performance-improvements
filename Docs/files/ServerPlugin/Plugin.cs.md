# `ServerPlugin/Plugin.cs`

*Dedicated-server plugin entry point: loads config, applies Harmony patches, and drives the per-tick update loop.*

|  |  |
| --- | --- |
| **Module** | [Server Plugin Entry Point](../../modules/server-plugin.md) |
| **Source** | [`Plugin.cs`](../../../ServerPlugin/Plugin.cs) (145 lines) |
| **Kind** | Sealed class implementing `IPlugin`, `ICommonPlugin` |
| **Role** | Plugin entry point |

## Purpose

`Plugin` is the class Magnetar / Quasar instantiates when the plugin is loaded. It implements both the game's `IPlugin` contract and the shared [`ICommonPlugin.cs`](../Shared/Plugin/ICommonPlugin.cs.md) contract so [`Common.cs`](../Shared/Plugin/Common.cs.md) can hold a single typed reference used by all patch code.

On `Init` it resolves the config file path via `PathResolver.Normalize` (case-insensitive on Linux), loads or default-constructs a [`PerformanceConfig.cs`](Config/PerformanceConfig.cs.md) via `ConfigStorage`, persists a sparse on-disk copy immediately, subscribes to `PropertyChanged` for live pushes from Quasar, then hands off to [`Common.cs`](../Shared/Plugin/Common.cs.md).`SetPlugin` which triggers [`PatchHelpers.cs`](../Shared/Patches/PatchHelpers.cs.md).`HarmonyPatchAll`. The dual-logger design keeps lifecycle messages (load, save, errors) in the PluginSdk-structured log while patch code writes to the game's `MyLog` via [`PluginLogger.cs`](../Shared/Logging/PluginLogger.cs.md).

On each game `Update` call it delegates to `PatchHelpers.PatchUpdates()` and increments `Tick`. In release builds all update exceptions are caught and the plugin is disabled to avoid crashing the server; in debug builds exceptions propagate so the debugger can catch them.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Instance` | Static property | Singleton reference; set on `Init`, cleared on `Dispose`. |
| `Tick` | Property | Monotonically increasing update counter; read by rate-limited patches. |
| `Log` | Property | The shared [`IPluginLogger.cs`](../Shared/Logging/IPluginLogger.cs.md) forwarded to [`Common.cs`](../Shared/Plugin/Common.cs.md).`Logger`. |
| `Config` | Property | The [`PerformanceConfig.cs`](Config/PerformanceConfig.cs.md) instance forwarded to [`Common.cs`](../Shared/Plugin/Common.cs.md).`Config`. |
| `Init(object)` | Method | Loads config, wires Quasar callbacks, invokes `Common.SetPlugin`, applies all Harmony patches. |
| `Update()` | Method | Per-tick driver; calls `PatchHelpers.PatchUpdates()` and increments `Tick`. |
| `TrySaveConfig()` | Static method | Persists `PerformanceConfig` to disk; called on init and on every `PropertyChanged` event. |
| `OnConfigChanged` | Static method | `PropertyChanged` handler: logs the changed property name and calls `TrySaveConfig`. |
| `Dispose()` | Method | Unsubscribes from `PropertyChanged` and clears `Instance`. |

## References

- [`ICommonPlugin.cs`](../Shared/Plugin/ICommonPlugin.cs.md) — interface this class implements (shared contract with the client plugin)
- [`PerformanceConfig.cs`](Config/PerformanceConfig.cs.md) — the XML-serialized server configuration
- [`Common.cs`](../Shared/Plugin/Common.cs.md) — shared bootstrap called during `Init`
- [`PatchHelpers.cs`](../Shared/Patches/PatchHelpers.cs.md) — applies all Harmony patches and drives per-tick updates
- [`PluginLogger.cs`](../Shared/Logging/PluginLogger.cs.md) — the `IPluginLogger` implementation used for patch-side logging

---

*[Handbook](../../TOC.md) · [Module: Server Plugin Entry Point](../../modules/server-plugin.md) · [Index](../../Index.md)*
