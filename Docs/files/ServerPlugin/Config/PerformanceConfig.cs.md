# `ServerPlugin/Config/PerformanceConfig.cs`

*XML-serialized server configuration implementing [`IPluginConfig.cs`](../../Shared/Config/IPluginConfig.cs.md); each property corresponds to one toggleable performance fix.*

|  |  |
| --- | --- |
| **Module** | [Server Plugin Entry Point](../../../modules/server-plugin.md) |
| **Source** | [`PerformanceConfig.cs`](../../../../ServerPlugin/Config/PerformanceConfig.cs) (108 lines) |
| **Kind** | Class extending `PluginSdk.Config.PluginConfig`, implementing `IPluginConfig` |
| **Role** | Configuration |

## Purpose

`PerformanceConfig` is the single source of truth for all performance fix toggles on the server. It derives from `PluginSdk.Config.PluginConfig` (which supplies `INotifyPropertyChanged` via `SetField`) and implements [`IPluginConfig.cs`](../../Shared/Config/IPluginConfig.cs.md) so Harmony patches in the `Shared` project can gate on it through [`Common.cs`](../../Shared/Plugin/Common.cs.md).`Config` without any direct reference to the server assembly.

The class is decorated with `[Tab]` and `[Section]` attributes from PluginSdk so Quasar can render a structured admin UI automatically. Properties are grouped into five sections: **Core** (global enable), **World load & networking**, **Simulation**, **Requires server restart**, and **Optional (off by default)**. The "optional" group contains fixes that have visible gameplay consequences (conveyor caching, block access caching, LCD visibility, log rate limiting, projected block disabling) and therefore default to `false` on the server, while the client defaults everything to `true`. Fixes that require a server restart are grouped separately to make the constraint explicit in the UI.

Each property uses a C# 13 field keyword (`set => SetField(ref field, value)`) to fire `PropertyChanged` on mutation, which [`Plugin.cs`](../Plugin.cs.md) subscribes to in order to persist the updated file immediately via `ConfigStorage.SaveXml`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Enabled` | Property | Master switch; disables all patches when `false`. Default: `true`. |
| `FixGridMerge` / `FixGridPaste` | Properties | Suppress redundant conveyor/world updates during merge and paste. Default: `true`. |
| `FixGridGroups` | Property | Suppresses resource updates while grids move between groups. Default: `true`. |
| `CacheMods` / `CacheScripts` | Properties | Cache compiled mods and PB scripts across restarts. Default: `true`. |
| `FixGarbageCollection` | Property | Skips selected `GC.Collect` calls to avoid long pauses. Default: `true`. |
| `FixSafeZone` / `FixSafeAction` | Properties | Cache safe-zone `IsSafe` and `IsActionAllowed` results. Default: `true`. |
| `FixConveyor` | Property | Caches conveyor network reachability lookups. Default: `false` (optional). |
| `FixAccess` / `FixTerminal` | Properties | Cache block access rights and PB access checks. Default: `false` (optional). |
| `FixLogFlooding` | Property | Rate-limits `GetBlueprintDefinition` log spam. Default: `false` (optional). |
| `FixProjection` | Property | Disables functional blocks in projected grids. Default: `false` (optional). |

## References

- [`IPluginConfig.cs`](../../Shared/Config/IPluginConfig.cs.md) — shared interface this class implements
- [`Plugin.cs`](../Plugin.cs.md) — loads, persists, and subscribes to this config
- [`Common.cs`](../../Shared/Plugin/Common.cs.md) — receives the config via `SetPlugin` and exposes it to all patches

---

*[Handbook](../../../TOC.md) · [Module: Server Plugin Entry Point](../../../modules/server-plugin.md) · [Index](../../../Index.md)*
