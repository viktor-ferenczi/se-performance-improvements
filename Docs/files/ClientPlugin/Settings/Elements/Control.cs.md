# `ClientPlugin/Settings/Elements/Control.cs`

*Immutable wrapper around a `MyGuiControlBase` that carries layout hints (`FixedWidth`, `FillFactor`, `MinWidth`, `RightMargin`, `Offset`, `OriginAlign`) consumed by the active [`Layout.cs`](../Layouts/Layout.cs.md).*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Control.cs`](../../../../../ClientPlugin/Settings/Elements/Control.cs) (30 lines) |
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

The static `LabelMinWidth` constant (`0.18f`) is a shared minimum width applied to all label columns across all element types.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `LabelMinWidth` | `static float` | Global minimum width for label controls; used by all element `GetControls` implementations. |
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
