# `ClientPlugin/Settings/Elements/Button.cs`

*`[Button]` attribute and `IElement` implementation that exposes a `void`-returning `Action` method on `Config` as a clickable button in the settings dialog.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Button.cs`](../../../../../ClientPlugin/Settings/Elements/Button.cs) (36 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — button |

## Purpose

`ButtonAttribute` handles the case where a `Config` *method* (not a property) should be exposed as a clickable button. [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) processes methods as well as properties; when it finds a method decorated with `[Button]`, the getter delegate returns the method's `Delegate` and the setter is `null`. `GetControls` invokes `(Action)propertyGetter()` inside the `ButtonClicked` handler to execute the action.

The control row consists of an empty label (fixed at `LabelMinWidth` to align with property rows) followed by the `MyGuiControlButton`, matching the visual layout of checkbox and textbox rows.

`SupportedTypes` contains only `typeof(Delegate)`, so the generator will throw if `[Button]` is placed on a property instead of a method.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Label` | `string` | Display text for the button; defaults to the PascalCase-split method name. |
| `Description` | `string` | Tooltip text. |
| `GetControls(...)` | method | Returns `[empty label, MyGuiControlButton]`; button click invokes the action delegate. |
| `SupportedTypes` | property | `[typeof(Delegate)]` — valid only on methods. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned by `GetControls`
- [`Tools.cs`](../Tools/Tools.cs.md) — `GetLabelOrDefault` for auto-labelling
- [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) — discovers method attributes and supplies the `Delegate` getter

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
