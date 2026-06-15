# `ClientPlugin/Settings/Elements/Separator.cs`

*`[Separator]` attribute and `IElement` implementation that inserts an orange section-caption label followed by a full-width horizontal rule between groups of settings.*

|  |  |
| --- | --- |
| **Module** | [Client Settings UI Framework](../../../../modules/client-settings.md) |
| **Source** | [`Separator.cs`](../../../../../ClientPlugin/Settings/Elements/Separator.cs) (46 lines) |
| **Kind** | `Internal class : Attribute, IElement` |
| **Role** | UI element — section separator |

## Purpose

`SeparatorAttribute` is a structural/decorative element that groups the settings dialog into named sections. Each section in [`Config.cs`](../../Config.cs.md) opens with a property annotated `[Separator("Section Name")]` followed by a `[Checkbox]` on the same property (or the next property). The separator produces two controls: an orange `MyGuiControlLabel` with the caption text, and a borderless `MyGuiControlLabel` with `FillFactor=1f` that [`Simple.cs`](../Layouts/Simple.cs.md) stretches to fill the remaining row width, creating a horizontal rule effect.

`SupportedTypes` lists `typeof(object)`, making the separator compatible with any property type.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Caption` | `string` | Text shown in orange on the left; may be null for an unlabelled divider. |
| `GetControls(...)` | method | Returns `[orange caption label, fill-width line label]`. |
| `SupportedTypes` | property | `[typeof(object)]` — matches any property type. |

## References

- [`Element.cs`](Element.cs.md) — `IElement` contract
- [`Control.cs`](Control.cs.md) — wrapper type returned; the line uses `FillFactor=1f`
- [`Simple.cs`](../Layouts/Simple.cs.md) — stretches the fill-factor control to produce the horizontal line effect
- [`Config.cs`](../../Config.cs.md) — uses `[Separator]` to open each settings section

---

*[Handbook](../../../../TOC.md) · [Module: Client Settings UI Framework](../../../../modules/client-settings.md) · [Index](../../../../Index.md)*
