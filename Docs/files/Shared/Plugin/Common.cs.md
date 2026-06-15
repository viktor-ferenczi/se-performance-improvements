# `Shared/Plugin/Common.cs`

*Shared bootstrap and static state hub: `SetPlugin` wires up the logger, config, filesystem directories and patch configuration once per process; `AttachPlugin` (re)points the shared accessors, letting the server swap its early stand-in for the live instance.*

|  |  |
| --- | --- |
| **Module** | [Shared Plugin Core](../../../modules/shared-plugin-core.md) |
| **Source** | [`Common.cs`](../../../../Shared/Plugin/Common.cs) (90 lines) |
| **Kind** | Static class |
| **Role** | Plugin bootstrap / shared state hub |

## Purpose

`Common` is the single static rendezvous point for the plugin's shared state. By accepting an [`ICommonPlugin.cs`](ICommonPlugin.cs.md) it avoids any circular assembly dependency: the `Shared` project knows nothing about `ClientPlugin` or `ServerPlugin`; it only knows the interface.

`SetPlugin` runs the one-time setup, once per process: it points the shared accessors at the given plugin (via `AttachPlugin`), records the game version string, derives three well-known filesystem directories (`DataDir`, `CacheDir`, `DebugDir`) under the platform storage path, and then calls `CleanupCache` and `CleanupDebug`. These helpers delete stale `.cache` files (any file older than 90 days, or all of them when the game version has changed) and stale `.il` debug dumps (when either the game or plugin version has changed), so the compilation caches for mods and PB scripts are never reused across game updates. Finally it calls `PatchHelpers.Configure()`, letting patch classes perform one-time setup before the patches are applied by the caller. The client calls `SetPlugin` from its `Init`; the dedicated server calls it from its early bootstrap with a lightweight stand-in plugin (see [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md).`EarlyStartup`), because the heavy setup must happen before world-load compilation.

`AttachPlugin` does just the accessor wiring — `Plugin`, `Logger`, `Config` — and nothing else. `SetPlugin` delegates to it, and the dedicated server calls it again from `Init` to swap the live `Plugin` instance in for the early stand-in, so per-tick code reaches the real `Tick` counter.

All patch code reads `Common.Logger`, `Common.Config`, and `Common.Tick` (via `Plugin.Tick`) through this class, which makes it the central dependency for the entire patch infrastructure.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Plugin` | Static property | The active [`ICommonPlugin.cs`](ICommonPlugin.cs.md) (client or server). |
| `Logger` | Static property | The [`IPluginLogger.cs`](../Logging/IPluginLogger.cs.md) extracted from `Plugin.Log`; used by all patch classes. |
| `Config` | Static property | The [`IPluginConfig.cs`](../Config/IPluginConfig.cs.md) extracted from `Plugin.Config`; gates every patch. |
| `GameVersion` | Static property | Game version string (including build number) used to detect cache invalidation. |
| `PluginVersion` | Const string | Plugin version string (`"1.11.22"`); triggers debug dump cleanup when changed. |
| `DataDir` / `CacheDir` / `DebugDir` | Static properties | Derived filesystem paths under the platform storage directory. |
| `SetPlugin(ICommonPlugin, string, string)` | Static method | One-time per-process setup: `AttachPlugin`, record game version, derive/clean the storage directories, then `PatchHelpers.Configure()`. Client calls it from `Init`; server from its early bootstrap with a stand-in. |
| `AttachPlugin(ICommonPlugin)` | Static method | Points `Plugin` / `Logger` / `Config` at the given instance. Called by `SetPlugin`, and again from the server's `Init` to swap the stand-in for the live instance. |

## References

- [`ICommonPlugin.cs`](ICommonPlugin.cs.md) — contract accepted by `SetPlugin`
- [`IPluginConfig.cs`](../Config/IPluginConfig.cs.md) — config contract held in `Config`
- [`IPluginLogger.cs`](../Logging/IPluginLogger.cs.md) — logger contract held in `Logger`
- [`PatchHelpers.cs`](../Patches/PatchHelpers.cs.md) — `Configure()` is called here; it later applies the patches (all at once on the client, in two phases on the server)
- [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) — the server caller: `SetPlugin` from the early bootstrap, `AttachPlugin` from `Init`

---

*[Handbook](../../../TOC.md) · [Module: Shared Plugin Core](../../../modules/shared-plugin-core.md) · [Index](../../../Index.md)*
