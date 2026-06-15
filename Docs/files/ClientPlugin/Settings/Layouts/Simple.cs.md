# `ClientPlugin/Settings/Layouts/Simple.cs`

*[`Layout.cs`](Layout.cs.md) that arranges controls in a vertically scrollable single-column list, distributing horizontal space according to each `Control`'s fill-factor.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Simple.cs`](../../../../../ClientPlugin/Settings/Layouts/Simple.cs) (108 lines) |
| **Kind** | `Internal class : Layout` |
| **Role** | Scrollable settings layout |

## Purpose

`Simple` is the layout activated by [`Plugin.cs`](../../Plugin.cs.md) when the settings dialog opens. `RecreateControls` creates a `MyGuiControlParent` (the inner canvas) wrapped in a `MyGuiControlScrollablePanel` with a vertical scrollbar. All `Control` GUI objects from every row are added as children of the parent, and only the scroll panel is returned to [`SettingsScreen.cs`](../SettingsScreen.cs.md) to be added to the screen's own `Controls` collection.

`LayoutControls` implements a two-pass row/column algorithm. The vertical pass accumulates the tallest control in each row to compute cumulative `rowY` offsets and the total canvas height, which is applied back to the parent so the scroll panel knows its full extents. The horizontal pass distributes the available width: controls with `FixedWidth` get exactly that many units; controls with a `FillFactor` share the remaining space proportionally; controls with neither get at least `MinWidth`. `RightMargin` is added as spacing between columns. After positioning, `scrollPanel.RefreshInternals()` recalculates scroll bounds.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `SettingsPanelSize` | override | Returns `(0.5, 0.7)`. |
| `parent` | `MyGuiControlParent` field | Inner canvas whose height is set dynamically to total content height. |
| `scrollPanel` | `MyGuiControlScrollablePanel` field | Wraps `parent`; the only control parented to the screen. |
| `RecreateControls()` | override | Creates the parent + scroll panel; adds all controls as children of `parent`; returns `[scrollPanel]`. |
| `LayoutControls()` | override | Two-pass width/height distribution algorithm; calls `scrollPanel.RefreshInternals()`. |
| `ElementPadding` | `private const float` | `0.01f` — vertical gap between rows and horizontal inset from the panel edge. |

## References

- [`Layout.cs`](Layout.cs.md) — abstract base
- [`None.cs`](None.cs.md) — the placeholder layout replaced by `Simple` at dialog open
- [`Control.cs`](../Elements/Control.cs.md) — the per-element wrapper whose `FixedWidth`, `FillFactor`, `MinWidth`, `RightMargin`, `Offset`, and `OriginAlign` drive the layout algorithm
- [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) — activates `Simple` via `SetLayout<Simple>()`
- [`SettingsScreen.cs`](../SettingsScreen.cs.md) — receives `[scrollPanel]` from `RecreateControls` and adds it to the screen

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
