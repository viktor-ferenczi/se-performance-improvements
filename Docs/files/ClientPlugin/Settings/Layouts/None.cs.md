# `ClientPlugin/Settings/Layouts/None.cs`

*A no-op [`Layout.cs`](Layout.cs.md) that places all controls at the origin with no containers — used as the initial placeholder layout before the dialog is opened.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`None.cs`](../../../../../ClientPlugin/Settings/Layouts/None.cs) (31 lines) |
| **Kind** | `Internal class : Layout` |
| **Role** | Null layout |

## Purpose

`None` is the default layout created by [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) during construction, before [`Plugin.cs`](../../Plugin.cs.md) has had a chance to set a real layout via `SetLayout<Simple>()`. Its `RecreateControls` simply flattens all `Control` rows into a single list of raw `MyGuiControlBase` objects (without any container), and its `LayoutControls` sets every control's position to `Vector2.Zero`. The result is intentionally invisible (all controls stacked at the origin), which is acceptable because `None` is replaced by [`Simple.cs`](Simple.cs.md) before the dialog is ever pushed onto the GUI stack.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `SettingsPanelSize` | override | Returns `(0.5, 0.5)`. |
| `RecreateControls()` | override | Returns a flat list of all `GuiControl` instances across all rows; no containers created. |
| `LayoutControls()` | override | Sets every control's `Position` to `Vector2.Zero`. |

## References

- [`Layout.cs`](Layout.cs.md) — abstract base
- [`Simple.cs`](Simple.cs.md) — the real layout that replaces `None` when the dialog opens
- [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) — creates `None` as the initial `ActiveLayout`

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
