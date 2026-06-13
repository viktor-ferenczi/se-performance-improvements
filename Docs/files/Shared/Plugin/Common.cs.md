# `Shared/Plugin/Common.cs`

*Shared bootstrap that receives the plugin instance from either host and wires up the logger, config, filesystem directories, and Harmony patch infrastructure.*

|  |  |
| --- | --- |
| **Module** | [Shared Plugin Core](../../../modules/shared-plugin-core.md) |
| **Source** | [`Common.cs`](../../../../Shared/Plugin/Common.cs) (78 lines) |
| **Kind** | Static class |
| **Role** | Plugin bootstrap / shared state hub |

## Purpose

`Common` is the single static rendezvous point that both the client plugin and [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) call once during `Init`. By accepting an [`ICommonPlugin.cs`](ICommonPlugin.cs.md) it avoids any circular assembly dependency: the `Shared` project knows nothing about `ClientPlugin` or `ServerPlugin`; it only knows the interface.

`SetPlugin` extracts `Log` and `Config` from the provided plugin, records the game version string, derives three well-known filesystem directories (`DataDir`, `CacheDir`, `DebugDir`) under the platform storage path, and then calls `CleanupCache` and `CleanupDebug`. These helpers delete stale `.cache` files (any file older than 90 days, or all of them when the game version has changed) and stale `.il` debug dumps (when either the game or plugin version has changed). This ensures the compilation caches for mods and PB scripts are never reused across game updates. After cleanup, `PatchHelpers.Configure()` is called to allow patch classes to perform one-time setup before `HarmonyPatchAll` is invoked by the caller.

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
| `SetPlugin(ICommonPlugin, string, string)` | Static method | Called once by the host plugin to initialise all shared state and trigger `PatchHelpers.Configure()`. |

## References

- [`ICommonPlugin.cs`](ICommonPlugin.cs.md) — contract accepted by `SetPlugin`
- [`IPluginConfig.cs`](../Config/IPluginConfig.cs.md) — config contract held in `Config`
- [`IPluginLogger.cs`](../Logging/IPluginLogger.cs.md) — logger contract held in `Logger`
- [`PatchHelpers.cs`](../Patches/PatchHelpers.cs.md) — configured and subsequently used to apply all Harmony patches
- [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) — the server-side caller of `SetPlugin`

---

*[Handbook](../../../TOC.md) · [Module: Shared Plugin Core](../../../modules/shared-plugin-core.md) · [Index](../../../Index.md)*
