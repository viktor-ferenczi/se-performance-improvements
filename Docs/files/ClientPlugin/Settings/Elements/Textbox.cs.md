# `ClientPlugin/Settings/Elements/Textbox.cs`

*`[Textbox]` attribute and `IElement` implementation that renders a `string` config property as a label + free-text input box.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Textbox.cs`](../../../../../ClientPlugin/Settings/Elements/Textbox.cs) (36 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — text input |

## Purpose

`TextboxAttribute` is the simplest text-based element. `GetControls` creates a `MyGuiControlTextbox` pre-populated with the current string value, wires its `TextChanged` event to call `propertySetter(box.Text)` on every keystroke, and attaches the description as a tooltip. The row is `[label (minWidth), textbox (fillFactor=1)]`.

`SupportedTypes` is `[typeof(string)]`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Label` / `Description` | `string` | Display label and tooltip. |
| `GetControls(...)` | method | Returns `[label, MyGuiControlTextbox]`; `TextChanged` writes through to `propertySetter` on every character. |
| `SupportedTypes` | property | `[typeof(string)]`. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned
- [`Tools.cs`](../Tools/Tools.cs.md) — `GetLabelOrDefault` for auto-labelling

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
