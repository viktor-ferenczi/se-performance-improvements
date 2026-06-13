# `ClientPlugin/Settings/Elements/Checkbox.cs`

*`[Checkbox]` attribute and `IElement` implementation that renders a `bool` config property as a label + checkbox row with immediate write-through to `Config.Current`.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Checkbox.cs`](../../../../../ClientPlugin/Settings/Elements/Checkbox.cs) (35 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — checkbox |

## Purpose

`CheckboxAttribute` is the most-used element in this plugin: every performance-fix toggle in [`Config.cs`](../../Config.cs.md) is decorated with `[Checkbox]`. `GetControls` reads the current property value via `propertyGetter()` to set the initial checked state, and wires `IsCheckedChanged` to call `propertySetter(x.IsChecked)` so any change propagates immediately to `Config.Current` without any additional plumbing. The label is resolved via [Tools.GetLabelOrDefault](../Tools/Tools.cs.md).

The control row is `[MyGuiControlLabel (minWidth=LabelMinWidth), MyGuiControlCheckbox (toolTip=Description)]`, matching the two-column convention used by all other element types.

`SupportedTypes` is `[typeof(bool)]`, so the generator will reject `[Checkbox]` on non-boolean properties at startup.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Label` | `string` | Optional display label; auto-generated from the property name if null. |
| `Description` | `string` | Tooltip shown on the checkbox. |
| `GetControls(...)` | method | Returns `[label control, MyGuiControlCheckbox]` wired to getter/setter. |
| `SupportedTypes` | property | `[typeof(bool)]`. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned
- [`Tools.cs`](../Tools/Tools.cs.md) — `GetLabelOrDefault` for auto-labelling
- [`Config.cs`](../../Config.cs.md) — all fix-toggle properties use this attribute

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
