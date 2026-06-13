# `ClientPlugin/Settings/SettingsScreen.cs`

*`MyGuiScreenBase` subclass that hosts the plugin settings controls and triggers config persistence when the dialog is closed.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../modules/client-settings.md) |
| **Source** | [`SettingsScreen.cs`](../../../../ClientPlugin/Settings/SettingsScreen.cs) (70 lines) |
| **Kind** | `Internal class : MyGuiScreenBase` |
| **Role** | UI screen host |

## Purpose

`SettingsScreen` is a thin subclass of the game's `MyGuiScreenBase` that delegates all control creation to a `Func<List<MyGuiControlBase>>` callback supplied by [`SettingsGenerator.cs`](SettingsGenerator.cs.md). This keeps the screen itself free of any knowledge about the config or the element types.

`RecreateControls` (called by the base class on `LoadContent` and whenever the screen is re-shown) adds a caption label then iterates the list returned by `GetControls()`, adding each item to the screen's `Controls` collection. The [`Layout.cs`](Layouts/Layout.cs.md) has already positioned the controls by the time this runs, so `RecreateControls` does nothing more than register them with the screen.

`OnRemoved` is called by the game when the dialog is dismissed (escape, close button). It persists the current state of [`Config.cs`](../Config.cs.md) to disk via [`ConfigStorage.cs`](ConfigStorage.cs.md).`Save` before handing off to the base class. `UpdateSize` exists so that [`SettingsGenerator.cs`](SettingsGenerator.cs.md) can resize the screen when switching layouts, and forces the close-button to re-render at the correct position by re-assigning the `CloseButtonEnabled` property.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `FriendlyName` | `string` property | Screen name returned by `GetFriendlyName()`; used by the game's screen manager. |
| `GetControls` | `Func<List<MyGuiControlBase>>` field | Delegate injected by [`SettingsGenerator.cs`](SettingsGenerator.cs.md) to supply the built controls. |
| `UpdateSize(Vector2)` | method | Resizes the screen and forces a close-button refresh. |
| `LoadContent()` | override | Calls `RecreateControls(true)` to populate controls from the delegate. |
| `RecreateControls(bool)` | override | Adds a title caption and all controls returned by `GetControls()`. |
| `OnRemoved()` | override | Saves `Config.Current` via [`ConfigStorage.cs`](ConfigStorage.cs.md) before the screen is unloaded. |

## References

- [`SettingsGenerator.cs`](SettingsGenerator.cs.md) — creates this screen and supplies the `GetControls` delegate
- [`ConfigStorage.cs`](ConfigStorage.cs.md) — called from `OnRemoved` to persist the config
- [`Config.cs`](../Config.cs.md) — the config object saved on close
- [`Layout.cs`](Layouts/Layout.cs.md) — positions controls before `RecreateControls` registers them

---

*[Handbook](../../../TOC.md) · [Module: Client Settings UI Framework](../../../modules/client-settings.md) · [Index](../../../Index.md)*
