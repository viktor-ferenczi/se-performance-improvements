# `ClientPlugin/Settings/SettingsGenerator.cs`

*Reflects over `Config` properties and methods at startup to build an `AttributeInfo` list, then drives the active `Layout` to materialise GUI controls and wire them to the live config.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../modules/client-settings.md) |
| **Source** | [`SettingsGenerator.cs`](../../../../ClientPlugin/Settings/SettingsGenerator.cs) (149 lines) |
| **Kind** | `Internal class` |
| **Role** | Settings dialog builder / coordinator |

## Purpose

`SettingsGenerator` is the central coordinator of the settings framework. During construction it calls `ExtractAttributes`, which iterates every property and method on [`Config.cs`](../Config.cs.md) via reflection. For each member that carries an [IElement](Elements/Element.cs.md) attribute, it records an `AttributeInfo` containing the `IElement` instance, the property name, and a getter/setter delegate pair that reads from and writes to `Config.Current`. Methods are handled similarly, but the getter returns the `Delegate` itself (used by [`Button.cs`](Elements/Button.cs.md)).

The generator also creates the [`SettingsScreen.cs`](SettingsScreen.cs.md) (an `MyGuiScreenBase` subclass) and the initial [`None.cs`](Layouts/None.cs.md) layout. When the screen needs to populate its controls (triggered by `LoadContent`/`RecreateControls`), it calls the `OnRecreateControls` callback injected at construction. That callback calls `CreateConfigControls`, which asks each `AttributeInfo`'s `IElement` for its `List<Control>`, then delegates positioning to the active [`Layout.cs`](Layouts/Layout.cs.md).

The active layout is swapped by `SetLayout<T>`. [`Plugin.cs`](../Plugin.cs.md) calls `SetLayout<Simple>()` just before pushing the screen onto the GUI stack, so the scrollable layout is only active when the dialog is actually visible.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Dialog` | `SettingsScreen` property | The `MyGuiScreenBase` instance; created once in the constructor. |
| `ActiveLayout` | `Layout` property | The currently selected layout strategy; defaults to [`None.cs`](Layouts/None.cs.md). |
| `attributes` | `List<AttributeInfo>` field | Ordered list of reflected config members with their element types and accessor delegates. |
| `controls` | `List<List<Control>>` field | Flat list of control groups, rebuilt by `CreateConfigControls` on each `RecreateControls` call. |
| `ExtractAttributes()` | `static method` | Reflects properties and methods of `Config`; validates element/type compatibility; returns `AttributeInfo` list. |
| `CreateConfigControls()` | `private method` | Asks each `AttributeInfo.ElementType.GetControls(...)` for its `Control` rows and collects them. |
| `SetLayout<T>()` | method | Instantiates a new layout of type `T` and updates the dialog's size. |
| `OnRecreateControls()` | `private method` | Callback passed to [`SettingsScreen.cs`](SettingsScreen.cs.md); rebuilds controls then asks the layout to position them. |

## References

- [`Config.cs`](../Config.cs.md) — reflected to extract attribute metadata
- [`Element.cs`](Elements/Element.cs.md) — `IElement` contract implemented by every attribute class
- [`Control.cs`](Elements/Control.cs.md) — the layout-agnostic wrapper each element returns
- [`Layout.cs`](Layouts/Layout.cs.md) — abstract base consumed by `SetLayout<T>` and `OnRecreateControls`
- [`None.cs`](Layouts/None.cs.md) — default layout used before the dialog is opened
- [`Simple.cs`](Layouts/Simple.cs.md) — layout activated by [`Plugin.cs`](../Plugin.cs.md) on dialog open
- [`SettingsScreen.cs`](SettingsScreen.cs.md) — the GUI screen created and owned by this generator

---

*[Handbook](../../../TOC.md) · [Module: Client Settings UI Framework](../../../modules/client-settings.md) · [Index](../../../Index.md)*
