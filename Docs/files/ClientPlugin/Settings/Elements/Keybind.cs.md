# `ClientPlugin/Settings/Elements/Keybind.cs`

*`[Keybind]` attribute and `IElement` implementation that renders a [`Binding.cs`](../Tools/Binding.cs.md)-typed config property as a label, key-assignment button, and Ctrl/Alt/Shift checkboxes.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Keybind.cs`](../../../../../ClientPlugin/Settings/Elements/Keybind.cs) (177 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — key binding |

## Purpose

`KeybindAttribute` handles [`Binding.cs`](../Tools/Binding.cs.md)-typed config properties. `GetControls` builds a five-control row: a label, a `MyGuiControlButton` displaying the currently bound key (using the game's own `MyControl` / `AppendBoundButtonNames` machinery), and three `MyGuiControlCheckbox` controls for Ctrl, Alt, and Shift modifiers.

Clicking the button opens the game's private `MyGuiControlAssignKeyMessageBox` dialog (accessed via reflection because it is `private` inside `MyGuiScreenOptionsMouseKeyboard`). When that dialog closes, `StoreControl` reads the newly assigned key from the `MyControl` object, writes it into the [`Binding.cs`](../Tools/Binding.cs.md) struct, and calls `propertySetter`. Right-clicking the button opens a confirmation dialog to clear the binding. Each modifier checkbox's `IsCheckedChanged` similarly reads the current binding, mutates the relevant flag, and writes back.

`SupportedTypes` is `[typeof(Binding)]`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Label` / `Description` | `string` | Display label and tooltip. |
| `GetControls(...)` | method | Returns `[label, key button, Ctrl checkbox, Alt checkbox, Shift checkbox]`. |
| `OnRebindClick` | `private method` | Opens the game's key-assignment dialog via reflection; writes result via `StoreControl`. |
| `OnUnbindClick` | `private method` | Shows a YES/NO confirmation, then clears the key. |
| `StoreControl` | `private method` | Reads the updated key from `MyControl`, rebuilds a `Binding`, calls `propertySetter`, updates button text. |
| `SupportedTypes` | property | `[typeof(Binding)]`. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Binding.cs`](../Tools/Binding.cs.md) — the value type this element operates on
- [`Control.cs`](Control.cs.md) — wrapper type returned
- [`Tools.cs`](../Tools/Tools.cs.md) — `GetLabelOrDefault` for auto-labelling

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
