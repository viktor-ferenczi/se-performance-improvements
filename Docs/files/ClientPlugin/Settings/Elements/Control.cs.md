# `ClientPlugin/Settings/Elements/Control.cs`

*Immutable wrapper around a `MyGuiControlBase` that carries layout hints (`FixedWidth`, `FillFactor`, `MinWidth`, `RightMargin`, `Offset`, `OriginAlign`) consumed by the active [`Layout.cs`](../Layouts/Layout.cs.md).*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Control.cs`](../../../../../ClientPlugin/Settings/Elements/Control.cs) (49 lines) |
| **Kind** | `Internal class` |
| **Role** | Layout-hint wrapper |

## Purpose

`Control` decouples the element attribute classes from the layout algorithm. Every `IElement.GetControls` implementation returns a `List<Control>` rather than bare `MyGuiControlBase` objects. Each `Control` bundles the GUI widget together with sizing hints that [`Simple.cs`](../Layouts/Simple.cs.md) uses in its horizontal distribution pass:

- `FixedWidth` — exact pixel width; overrides all other sizing.
- `FillFactor` — relative weight for proportional width distribution among flexible columns.
- `MinWidth` — floor width when neither `FixedWidth` nor `FillFactor` is set.
- `RightMargin` — gap added to the right of this control before the next column.
- `Offset` — fine-tune nudge applied on top of the computed position.
- `OriginAlign` — the `MyGuiDrawAlignEnum` used when setting the control's `Position`.

The static fields describe the dialog's own geometry, measured on the dialog rather than derived from it (see the `FIXME` in the file). `RowLeft` (`0.04f`) and `RowRight` (`0.94f`) are where a row starts and where it has to end — a little short of the `0.95` at which the scrolled area is clipped by the scrollbar, since a glyph's ink can reach slightly past the width it is measured at. `LabelMinWidth` (`0.3f`) is the shared minimum width of the label column, `LabelGap` (`0.01f`) keeps a long label from touching what follows it, and `DescriptionMinWidth` is what a checkbox row has left for its description once those have taken theirs. `ToolTipWidth` (`0.6f`) is the width tooltips are wrapped to; they are drawn next to the cursor rather than inside the dialog, so they get a width of their own.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `RowLeft` / `RowRight` | `static float` | Where a row starts and where it has to end, inside the clipped scroll area. |
| `LabelMinWidth` | `static float` | Global minimum width for label controls; used by all element `GetControls` implementations. |
| `LabelGap` | `static float` | Gap kept between a label and the control or description after it. |
| `DescriptionMinWidth` | `static float` | What is left of a checkbox row for its description when the label is at its minimum width. |
| `ToolTipWidth` | `static float` | Width tooltips are wrapped to. |
| `GuiControl` | `MyGuiControlBase` | The underlying game GUI widget. |
| `FixedWidth` | `float?` | If set, [`Simple.cs`](../Layouts/Simple.cs.md) forces this exact width. |
| `FillFactor` | `float?` | Proportional share of remaining width; used by [`Simple.cs`](../Layouts/Simple.cs.md)'s distribution algorithm. |
| `MinWidth` | `float` | Minimum allowed width when `FixedWidth` and `FillFactor` are both null. |
| `RightMargin` | `float` | Horizontal gap inserted after this control. |
| `Offset` | `Vector2` | Additional position offset applied after layout. |
| `OriginAlign` | `MyGuiDrawAlignEnum` | Alignment anchor for `Position` assignment. |

## References

- [`Element.cs`](Element.cs.md) — `IElement.GetControls` returns `List<Control>`
- [`Simple.cs`](../Layouts/Simple.cs.md) — consumes all layout-hint fields in `LayoutControls`
- [`None.cs`](../Layouts/None.cs.md) — accesses only `GuiControl` for flat enumeration
- [`SettingsGenerator.cs`](../SettingsGenerator.cs.md) — collects `List<List<Control>>` from each element

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
