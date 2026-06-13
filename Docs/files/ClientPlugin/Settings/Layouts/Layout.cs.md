# `ClientPlugin/Settings/Layouts/Layout.cs`

*Abstract base class that every layout strategy must implement: it declares the screen size, the control-creation step, and the positioning step.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Layout.cs`](../../../../../ClientPlugin/Settings/Layouts/Layout.cs) (37 lines) |
| **Kind** | `Internal abstract class` |
| **Role** | Layout strategy base |

## Purpose

`Layout` defines the contract between [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) and the concrete layout strategies ([`None.cs`](None.cs.md), [`Simple.cs`](Simple.cs.md)). A layout receives a `Func<List<List<Control>>>` delegate at construction time; calling it returns the current list of control rows built by [`SettingsGenerator`](../SettingsGenerator.cs.md)`.CreateConfigControls`.

The two abstract members correspond to two distinct phases of screen setup. `RecreateControls` is called first: the layout creates any container controls it needs (e.g. a scroll panel) and returns a flat list of `MyGuiControlBase` objects that should be parented to the screen itself. `LayoutControls` is called after `RecreateControls` and is responsible only for positioning; it must not create new controls. [`SettingsGenerator.cs`](../SettingsGenerator.cs.md)'s `OnRecreateControls` callback calls both in sequence, and `RefreshLayout` calls only `LayoutControls` when the screen needs to re-position without rebuilding controls.

`SettingsPanelSize` lets each layout advertise its preferred screen dimensions so [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) can call `Dialog.UpdateSize` accordingly.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `SettingsPanelSize` | `abstract Vector2` property | Preferred screen size reported to [`SettingsGenerator.cs`](../SettingsGenerator.cs.md). |
| `GetControls` | `protected Func<List<List<Control>>>` | Delegate to retrieve the current control rows from the generator. |
| `RecreateControls()` | `abstract method` | Creates layout-specific container controls; returns those to be parented to the screen. |
| `LayoutControls()` | `abstract method` | Positions all existing controls; must not create new ones. |

## References

- [`None.cs`](None.cs.md) — trivial layout implementation (no containers, zero position)
- [`Simple.cs`](Simple.cs.md) — scrollable single-column layout implementation
- [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) — creates and switches layouts; calls both abstract methods
- [`Control.cs`](../Elements/Control.cs.md) — the per-element wrapper iterated by concrete layouts

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
