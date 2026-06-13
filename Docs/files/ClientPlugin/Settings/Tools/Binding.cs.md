# `ClientPlugin/Settings/Tools/Binding.cs`

*Value type representing a keyboard shortcut (key + Ctrl/Alt/Shift modifiers) with press-detection helpers.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Binding.cs`](../../../../../ClientPlugin/Settings/Tools/Binding.cs) (40 lines) |
| **Kind** | `Public struct` |
| **Role** | Data type |

## Purpose

`Binding` is a plain value struct that packages a `MyKeys` key together with three boolean modifier flags (`Ctrl`, `Alt`, `Shift`). It is the property type used when a config option represents a keyboard shortcut and is the value type expected by [`Keybind.cs`](../Elements/Keybind.cs.md).

`ToString` renders the binding as a human-readable string such as `"Ctrl+Shift+F1"`, or `"None"` when no key is assigned. `IsPressed` tests whether the binding is currently held down (continuous), while `HasPressed` tests for the leading edge of a key press (one-shot), both delegating modifier matching to the private `AreModifiersMatch` helper.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Key` | `MyKeys` field | The primary key of the binding; `MyKeys.None` means unbound. |
| `Ctrl` / `Alt` / `Shift` | `bool` fields | Required modifier state. |
| `ToString()` | method | Formats the binding as `"[Ctrl+][Alt+][Shift+]Key"` or `"None"`. |
| `IsPressed(IMyInput)` | method | Returns `true` while the key and all modifiers are held. |
| `HasPressed(IMyInput)` | method | Returns `true` on the first frame the key+modifiers transition to pressed. |

## References

- [`Keybind.cs`](../Elements/Keybind.cs.md) — the `IElement` attribute that uses `Binding` as its property type
- [`Tools.cs`](Tools.cs.md) — sibling utility class in the same namespace

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
