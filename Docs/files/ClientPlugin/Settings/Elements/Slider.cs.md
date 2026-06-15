# `ClientPlugin/Settings/Elements/Slider.cs`

*`[Slider]` attribute and `IElement` implementation that renders a `float` or `int` config property as a label, range slider, and live value label, with optional manual numeric entry.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Slider.cs`](../../../../../ClientPlugin/Settings/Elements/Slider.cs) (115 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — slider |

## Purpose

`SliderAttribute` handles `float` and `int` properties. `GetControls` builds a three-control row: a label, a `MyGuiControlSlider` (fill-factor 1, so it takes all available horizontal space), and a read-only value label updated on every `ValueChanged` event.

The slider supports two modes controlled by the `SliderType` enum: `Float` (default) and `Integer`. In `Integer` mode the value is rounded to `int` before being passed to `propertySetter`. In `Float` mode, decimal places on the value label are computed from the step size. A `MinimumStepOverride` is set so the slider snaps to the configured `Step`.

Clicking the slider (via `SliderSetValueManual`) opens a `MyGuiScreenDialogAmount` that lets the user type an exact numeric value, providing accessibility beyond drag interaction. The dialog is set `CanHideOthers = true` via reflection (the property is `protected` in the game's type).

`SupportedTypes` is `[typeof(float), typeof(int)]`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Min` / `Max` / `Step` | `float` | Range and step size for the slider. |
| `Type` | `SliderType` | `Integer` or `Float`; controls rounding and label formatting. |
| `Label` / `Description` | `string` | Display label and tooltip. |
| `GetControls(...)` | method | Returns `[label, MyGuiControlSlider, value label]`; wires `ValueChanged` and `SliderSetValueManual`. |
| `ValueUpdate(MyGuiControlSlider)` | `local method` | Reads slider value, calls `propertySetter`, updates value label. |
| `SpecifyValue(MyGuiControlSlider)` | `local method` | Opens `MyGuiScreenDialogAmount` for keyboard entry; sets slider value on confirm. |
| `SupportedTypes` | property | `[typeof(float), typeof(int)]`. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned
- [`Tools.cs`](../Tools/Tools.cs.md) — `GetLabelOrDefault` for auto-labelling

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
