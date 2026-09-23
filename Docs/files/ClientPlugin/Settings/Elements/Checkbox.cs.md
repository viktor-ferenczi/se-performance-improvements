# `ClientPlugin/Settings/Elements/Checkbox.cs`

*`[Checkbox]` attribute and `IElement` implementation that renders a `bool` config property as a checkbox, label and description row with immediate write-through to `Config.Current`.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Checkbox.cs`](../../../../../ClientPlugin/Settings/Elements/Checkbox.cs) (48 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — checkbox |

## Purpose

`CheckboxAttribute` is the most-used element in this plugin: every performance-fix toggle in [`Config.cs`](../../Config.cs.md) is decorated with `[Checkbox]`. `GetControls` reads the current property value via `propertyGetter()` to set the initial checked state, and wires `IsCheckedChanged` to call `propertySetter(x.IsChecked)` so any change propagates immediately to `Config.Current` without any additional plumbing. The label is resolved via [Tools.GetLabelOrDefault](../Tools/Tools.cs.md).

The control row is `[MyGuiControlCheckbox (toolTip=Description), MyGuiControlLabel (the label), MyGuiControlLabel (the description)]`. Unlike the other element types the description is not only a tooltip here: it is a column of its own, so it has to be wrapped to fit. The width it gets is worked out per row — `RowRight - RowLeft` less the checkbox and the label, the label being the wider of its own text and `LabelMinWidth` — so a row whose label runs past the minimum simply leaves its description less room instead of pushing it off the edge. Both the description and the tooltip are wrapped by [Tools.Wrap](../Tools/Tools.cs.md); the row grows in height to fit, since [`Simple.cs`](../Layouts/Simple.cs.md) takes the height of a row's tallest control.

`SupportedTypes` is `[typeof(bool)]`, so the generator will reject `[Checkbox]` on non-boolean properties at startup.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Label` | `string` | Optional display label; auto-generated from the property name if null. |
| `Description` | `string` | Shown as the row's third column and as the checkbox's tooltip, wrapped to fit each. |
| `GetControls(...)` | method | Returns `[MyGuiControlCheckbox, label, wrapped description]` wired to getter/setter. |
| `SupportedTypes` | property | `[typeof(bool)]`. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned
- [`Tools.cs`](../Tools/Tools.cs.md) — `GetLabelOrDefault` for auto-labelling
- [`Config.cs`](../../Config.cs.md) — all fix-toggle properties use this attribute

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
