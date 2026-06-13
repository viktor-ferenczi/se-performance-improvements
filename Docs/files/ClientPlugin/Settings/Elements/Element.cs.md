# `ClientPlugin/Settings/Elements/Element.cs`

*`IElement` interface — the contract every settings-dialog attribute must implement to supply typed GUI controls to `SettingsGenerator`.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Element.cs`](../../../../../ClientPlugin/Settings/Elements/Element.cs) (10 lines) |
| **Kind** | `Internal interface` |
| **Role** | Element contract |

## Purpose

`IElement` is the single interface that connects the reflection-based [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) to the concrete attribute classes ([`Checkbox.cs`](Checkbox.cs.md), [`Slider.cs`](Slider.cs.md), [`Dropdown.cs`](Dropdown.cs.md), etc.). [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) casts each `Attribute` found on a `Config` property to `IElement`; if the cast succeeds, the attribute is a recognised settings element.

`SupportedTypes` tells the generator which CLR property types the element can handle. The generator validates this at startup and throws if the decorated property's type is not in the list, providing early detection of mismatched attribute/property pairs.

`GetControls` is called during each `RecreateControls` cycle to materialise the actual `MyGuiControlBase` objects. It receives the property name (for auto-labelling), a getter delegate that reads the current value from `Config.Current`, and a setter delegate that writes back. The element returns a `List<Control>` — one entry per GUI widget in the row (label + widget, or multiple widgets).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `SupportedTypes` | `List<Type>` property | CLR types this element can handle; validated by [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) at startup. |
| `GetControls(string, Func<object>, Action<object>)` | method | Creates and returns the `List<Control>` for this setting, wired to the supplied getter/setter. |

## References

- [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) — casts attributes to `IElement` and calls both members
- [`Control.cs`](Control.cs.md) — the wrapper type returned by `GetControls`
- [`Checkbox.cs`](Checkbox.cs.md), [`Button.cs`](Button.cs.md), [`Color.cs`](Color.cs.md), [`Dropdown.cs`](Dropdown.cs.md), [`Keybind.cs`](Keybind.cs.md), [`Separator.cs`](Separator.cs.md), [`Slider.cs`](Slider.cs.md), [`Textbox.cs`](Textbox.cs.md) — concrete implementations

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
