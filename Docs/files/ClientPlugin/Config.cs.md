# `ClientPlugin/Config.cs`

*Declares all client-side performance-fix toggles as `bool` properties decorated with settings-framework attributes, forming the single source of truth for both the in-game dialog and the shared Harmony patch guards.*

|  |  |
| --- | --- |
| **Module** | [Client Plugin Entry Point](../../modules/client-plugin.md) |
| **Source** | [`Config.cs`](../../../ClientPlugin/Config.cs) (224 lines) |
| **Kind** | `class Config : IPluginConfig` (which extends `INotifyPropertyChanged`) |
| **Role** | Configuration |

## Purpose

`Config` is the client counterpart to the server's `PerformanceConfig`. Each performance fix is a public `bool` auto-property using the C# `field` contextual keyword for its backing store (`get; set => SetField(ref field, value);`) with an `= true` initializer — client users want maximum performance out of the box. Writing a property raises `PropertyChanged`. The class implements `IPluginConfig` so the Harmony patches in the `Shared` project can gate on it uniformly via `Plugin.Common.Config`.

Two flags are exceptions: `FixWheelTrail` and `FixTextPanel` are **client-side stubs** (`get => false; set { }`) with no `[Checkbox]` attribute, so they do not appear in the client dialog and always read `false`. Their patches now run only on the server ([`MyWheelPatch.cs`](../ServerPlugin/Patches/Wheel/MyWheelPatch.cs.md), [`MyLcdSurfaceComponentPatch.cs`](../ServerPlugin/Patches/TextPanel/MyLcdSurfaceComponentPatch.cs.md)), where the real toggles live in [`PerformanceConfig.cs`](../ServerPlugin/Config/PerformanceConfig.cs.md).

Each public property is annotated with one or two attributes from the settings framework: a `[Separator]` attribute opens a named section header in the dialog, and a `[Checkbox]` attribute (carrying a human-readable `label` and `description` tooltip) tells [`SettingsGenerator.cs`](Settings/SettingsGenerator.cs.md) to emit a label + checkbox row for that property. [`SettingsGenerator.cs`](Settings/SettingsGenerator.cs.md) reads these attributes at startup via reflection and wires the getter/setter closures directly to the live `Config.Current` instance, so checking a box in the dialog writes through to the property immediately and triggers `PropertyChanged`.

The options are grouped into five sections mirroring the server config: *Core*, *World load & networking*, *Simulation*, *Requires server restart*, and *Optional*. `Config.Default` is a fresh instance with all fields at their default values; `Config.Current` is the singleton loaded (or defaulted) by [`ConfigStorage.cs`](Settings/ConfigStorage.cs.md) at startup and persisted back when the dialog closes.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Default` | `static Config` | Canonical defaults instance; used as fallback when no saved config exists. |
| `Current` | `static Config` | Live singleton loaded by [`ConfigStorage.cs`](Settings/ConfigStorage.cs.md) at startup; shared with all Harmony patches via `IPluginConfig`. |
| `Title` | `string` | Dialog caption (`"Performance"`), read by [`SettingsGenerator.cs`](Settings/SettingsGenerator.cs.md). |
| `Enabled` | `bool` property | Master switch; gates all other fixes when `false`. |
| `FixGridMerge` … `FixProjection` | `bool` properties | Individual per-fix toggles, default `true`; each decorated with `[Separator]` and/or `[Checkbox]` and backed by the C# `field` keyword. |
| `FixWheelTrail`, `FixTextPanel` | `bool` properties | Client-side stubs (`get => false; set { }`, no `[Checkbox]`) — the real toggles are server-only. |
| `SetField<T>` | `private method` | Equality-guarded `ref field` updater that raises `PropertyChanged` via `[CallerMemberName]`. |
| `PropertyChanged` | `event` | Standard `INotifyPropertyChanged` event; consumed by the settings framework when it needs to refresh controls. |

## References

- [`Plugin.cs`](Plugin.cs.md) — exposes `Config.Current` as `IPluginConfig` to the patch infrastructure
- [`ConfigStorage.cs`](Settings/ConfigStorage.cs.md) — loads and saves `Config.Current` as XML
- [`SettingsGenerator.cs`](Settings/SettingsGenerator.cs.md) — reflects over `Config` properties to build the dialog
- [client-settings](../../modules/client-settings.md) — the settings framework that processes `[Separator]` and `[Checkbox]` attributes

---

*[Handbook](../../TOC.md) · [Module: Client Plugin Entry Point](../../modules/client-plugin.md) · [Index](../../Index.md)*
