# `ClientPlugin/Settings/Tools/Tools.cs`

*Shared utility class providing automatic PascalCase-to-label conversion and hex-colour parsing/formatting helpers used across the settings element classes.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Tools.cs`](../../../../../ClientPlugin/Settings/Tools/Tools.cs) (77 lines) |
| **Kind** | `Public static class` |
| **Role** | Utility library |

## Purpose

`Tools` centralises two groups of helpers consumed by the element attribute classes.

`GetLabelOrDefault` is called by every element's `GetControls` implementation to resolve the display label: if a non-null `label` string is passed (from the attribute constructor), it is returned as-is; otherwise the property name is split on PascalCase word boundaries using a compiled `Regex` and joined with spaces, with words after the first lowercased. This means `[Checkbox]` on a property named `FixGridMerge` automatically yields `"Fix grid merge"` without any explicit label.

The colour helpers (`ToHexStringRgb`, `ToHexStringRgba`, `TryParseColorFromHexRgb`, `TryParseColorFromHexRgba`) are extension methods on `VRageMath.Color` and `string`, used exclusively by [`Color.cs`](../Elements/Color.cs.md) to convert between the game's colour type and the six- or eight-character hex strings shown in the textbox.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `GetLabelOrDefault(string name, string label)` | `static method` | Returns `label` if non-null; otherwise splits `name` on PascalCase boundaries and joins with spaces. |
| `ToHexStringRgb(Color)` | extension method | Formats colour as `"RRGGBB"` uppercase hex. |
| `ToHexStringRgba(Color)` | extension method | Formats colour as `"RRGGBBAA"` uppercase hex. |
| `TryParseColorFromHexRgb(string, out Color)` | extension method | Parses a six-character hex string into a `Color`; returns `false` on failure. |
| `TryParseColorFromHexRgba(string, out Color)` | extension method | Parses an eight-character hex string into a `Color` with alpha; returns `false` on failure. |

## References

- [`Color.cs`](../Elements/Color.cs.md) — consumes the hex colour helpers
- [`Checkbox.cs`](../Elements/Checkbox.cs.md), [`Button.cs`](../Elements/Button.cs.md), [`Dropdown.cs`](../Elements/Dropdown.cs.md), [`Keybind.cs`](../Elements/Keybind.cs.md), [`Separator.cs`](../Elements/Separator.cs.md), [`Slider.cs`](../Elements/Slider.cs.md), [`Textbox.cs`](../Elements/Textbox.cs.md) — all call `GetLabelOrDefault`
- [`Binding.cs`](Binding.cs.md) — sibling type in the same `Tools` namespace

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
