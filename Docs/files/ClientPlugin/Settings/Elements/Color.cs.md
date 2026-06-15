# `ClientPlugin/Settings/Elements/Color.cs`

*`[Color]` attribute and `IElement` implementation that renders a `VRageMath.Color` property as a label, colour-preview button, and hex textbox.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Color.cs`](../../../../../ClientPlugin/Settings/Elements/Color.cs) (86 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — colour picker |

## Purpose

`ColorAttribute` handles `VRageMath.Color` config properties. `GetControls` builds a three-control row: a label, a small square `MyGuiControlButton` that previews the current colour via its border, and a `MyGuiControlTextbox` containing the hex representation (RGB or RGBA depending on `HasAlpha`).

The textbox's `TextChanged` handler uses [`Tools.cs`](../Tools/Tools.cs.md) hex-parsing helpers to parse the input. On a valid parse it updates both the preview button's `BorderColor` and the config property (via `propertySetter`) and normalises the text to uppercase. On an invalid parse it turns the textbox border red. This gives immediate visual feedback without requiring an apply button.

`SupportedTypes` is `[typeof(Color)]`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `HasAlpha` | `bool` | If `true`, uses 8-digit RGBA hex; otherwise 6-digit RGB. |
| `Label` / `Description` | `string` | Display label and tooltip. |
| `GetControls(...)` | method | Returns `[label, colour preview button, hex textbox]`; textbox `TextChanged` validates and writes through. |
| `SupportedTypes` | property | `[typeof(Color)]`. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned
- [`Tools.cs`](../Tools/Tools.cs.md) — `ToHexStringRgb/Rgba` and `TryParseColorFromHexRgb/Rgba` extension methods

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
