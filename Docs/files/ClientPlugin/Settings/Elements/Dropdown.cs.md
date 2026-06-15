# `ClientPlugin/Settings/Elements/Dropdown.cs`

*`[Dropdown]` attribute and `IElement` implementation that renders an `enum`-typed config property as a label + combo-box, automatically populating items from enum member names.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Dropdown.cs`](../../../../../ClientPlugin/Settings/Elements/Dropdown.cs) (70 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — dropdown |

## Purpose

`DropdownAttribute` handles any `enum`-typed property. `GetControls` reflects the enum type at runtime, populates a `MyGuiControlCombobox` with one item per enum member (names de-camel-cased by the private `UnCamelCase` regex), and selects the item matching the current value. When the user selects a new item, `ItemSelected` parses the enum name back to the enum value and calls `propertySetter`.

`SupportedTypes` contains `typeof(Enum)`, so the generator accepts any enum property; the concrete enum type is discovered dynamically from the value returned by `propertyGetter()`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `VisibleRows` | `int` | Maximum visible rows in the expanded dropdown (default `20`). |
| `Label` / `Description` | `string` | Display label and tooltip. |
| `UnCamelCase(string)` | `private static method` | Inserts spaces before each uppercase letter boundary for readable enum member display. |
| `GetControls(...)` | method | Populates `MyGuiControlCombobox` from enum names; wires `ItemSelected` to `propertySetter`. |
| `SupportedTypes` | property | `[typeof(Enum)]`. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned
- [`Tools.cs`](../Tools/Tools.cs.md) — `GetLabelOrDefault` for auto-labelling

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
