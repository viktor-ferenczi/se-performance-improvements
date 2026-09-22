# `Shared/Config/IPluginConfig.cs`

*Shared configuration contract: one toggle per performance fix (plus the Havok thread count mode and number), and `INotifyPropertyChanged` so both platforms can react to live config updates.*

|  |  |
| --- | --- |
| **Module** | [Shared Plugin Core](../../../modules/shared-plugin-core.md) |
| **Source** | [`IPluginConfig.cs`](../../../../Shared/Config/IPluginConfig.cs) (87 lines) |
| **Kind** | Interface extending `INotifyPropertyChanged` |
| **Role** | Configuration contract |

## Purpose

`IPluginConfig` is the sole configuration dependency of every Harmony patch in the `Shared` project. Patch prefix/postfix methods gate their work with a guard like `if (!Common.Config.Enabled || !Common.Config.FixXxx) return;`, reading through this interface so the patch code compiles and runs identically on both platforms.

The client implements this interface with its in-game settings dialog class (`ClientPlugin.Config`); the server implements it with [`PerformanceConfig.cs`](../../ServerPlugin/Config/PerformanceConfig.cs.md). Both implementations notify via `INotifyPropertyChanged` when a value changes, allowing [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) to re-persist the config to disk on Quasar-pushed updates.

Every property maps to a distinct performance fix described in `Docs/PerformanceFixes.md`. The interface lists them in the same logical groups used by the server's UI tabs: world load / networking, simulation, restart-required, and optional (off by default on the server).

Most are `bool`. The exception is the Havok physics thread count, which is a `HavokThreadCountMode` (`Auto` or `Manual`) plus an `int`; both are declared in this file, together with the `HavokThreads` helper holding the range (2..64) and the automatic value (`min(16, processorCount)`). That helper lives here rather than in the patch which applies it ([`MyWindowsSystemPatch.cs`](../Patches/Physics/MyWindowsSystemPatch.cs.md)), so a config class can use it in an attribute or a property default without touching `Common` while it is still being constructed.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Enabled` | Property | Master kill-switch; when `false` all patches skip their logic. |
| `FixGridMerge` / `FixGridPaste` | Properties | Suppress conveyor/world updates during merge and paste operations. |
| `FixGridGroups` | Property | Suppress resource updates during grid-group transitions. |
| `CacheMods` / `CacheScripts` | Properties | Enable compiled-mod and PB-script caching. |
| `FixGarbageCollection` | Property | Skip selected `GC.Collect` calls. |
| `FixSafeZone` / `FixSafeAction` | Properties | Cache safe-zone membership and action-allowed results. |
| `FixConveyor` | Property | Cache conveyor network reachability lookups. |
| `FixAccess` / `FixTerminal` | Properties | Cache block access rights and PB access checks. |
| `FixLogFlooding` | Property | Rate-limit `GetBlueprintDefinition` log flooding. |
| `FixProjection` | Property | Disable functional blocks in projected grids. |

## References

- [`PerformanceConfig.cs`](../../ServerPlugin/Config/PerformanceConfig.cs.md) — server-side implementation
- [`Common.cs`](../Plugin/Common.cs.md) — exposes the active implementation via `Common.Config`
- [`ICommonPlugin.cs`](../Plugin/ICommonPlugin.cs.md) — aggregates `IPluginConfig` as `Config`

---

*[Handbook](../../../TOC.md) · [Module: Shared Plugin Core](../../../modules/shared-plugin-core.md) · [Index](../../../Index.md)*
