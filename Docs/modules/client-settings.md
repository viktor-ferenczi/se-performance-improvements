# Client Settings UI Framework

A declarative framework that turns the client `Config` into an in-game settings dialog: typed UI elements, layouts, data binding, change persistence and the settings screen itself.

The `client-settings` module is a self-contained, declarative settings-dialog framework. Its purpose is to turn [`Config.cs`](../files/ClientPlugin/Config.cs.md) — a plain C# class whose properties carry custom `Attribute` annotations — into a fully functional in-game GUI dialog, with no hand-written UI code required in the plugin layer. The framework is general enough to support a variety of input types (checkboxes, sliders, dropdowns, textboxes, colour pickers, key bindings, buttons) and is extensible: adding a new control type means implementing [IElement](../files/ClientPlugin/Settings/Elements/Element.cs.md) and nothing else.

The framework is organised into four layers: **tools** (shared utilities), **elements** (attribute+element pairs, one per control type), **layouts** (strategies for positioning a set of controls on screen), and **the screen/generator coordinator** that wires them all together.

## Files

| File | Summary |
| --- | --- |
| [`ConfigStorage.cs`](../files/ClientPlugin/Settings/ConfigStorage.cs.md) | XML serialisation/deserialisation of `Config` to/from `Performance.cfg` in user storage. |
| [`Button.cs`](../files/ClientPlugin/Settings/Elements/Button.cs.md) | `[Button]` element — maps a `Config` method to a clickable button. |
| [`Checkbox.cs`](../files/ClientPlugin/Settings/Elements/Checkbox.cs.md) | `[Checkbox]` element — maps a `bool` property to a label + checkbox row. |
| [`Color.cs`](../files/ClientPlugin/Settings/Elements/Color.cs.md) | `[Color]` element — maps a `Color` property to a label, colour preview, and hex textbox. |
| [`Control.cs`](../files/ClientPlugin/Settings/Elements/Control.cs.md) | Layout-hint wrapper bundling a `MyGuiControlBase` with sizing and alignment metadata. |
| [`Dropdown.cs`](../files/ClientPlugin/Settings/Elements/Dropdown.cs.md) | `[Dropdown]` element — maps an `enum` property to a label + combo-box. |
| [`Element.cs`](../files/ClientPlugin/Settings/Elements/Element.cs.md) | `IElement` interface — contract every attribute element must implement. |
| [`Keybind.cs`](../files/ClientPlugin/Settings/Elements/Keybind.cs.md) | `[Keybind]` element — maps a `Binding` property to a label, key-assignment button, and modifier checkboxes. |
| [`Separator.cs`](../files/ClientPlugin/Settings/Elements/Separator.cs.md) | `[Separator]` element — inserts an orange section caption and a horizontal rule. |
| [`Slider.cs`](../files/ClientPlugin/Settings/Elements/Slider.cs.md) | `[Slider]` element — maps a `float`/`int` property to a label, range slider, and value label. |
| [`Textbox.cs`](../files/ClientPlugin/Settings/Elements/Textbox.cs.md) | `[Textbox]` element — maps a `string` property to a label + free-text input. |
| [`Layout.cs`](../files/ClientPlugin/Settings/Layouts/Layout.cs.md) | Abstract base defining the two-phase layout contract (`RecreateControls` + `LayoutControls`). |
| [`None.cs`](../files/ClientPlugin/Settings/Layouts/None.cs.md) | No-op placeholder layout used before the dialog is opened. |
| [`Simple.cs`](../files/ClientPlugin/Settings/Layouts/Simple.cs.md) | Scrollable single-column layout with fill-factor horizontal distribution. |
| [`SettingsGenerator.cs`](../files/ClientPlugin/Settings/SettingsGenerator.cs.md) | Reflects `Config` attributes at startup and drives the layout/screen lifecycle. |
| [`SettingsScreen.cs`](../files/ClientPlugin/Settings/SettingsScreen.cs.md) | `MyGuiScreenBase` subclass that hosts controls and persists config on close. |
| [`Binding.cs`](../files/ClientPlugin/Settings/Tools/Binding.cs.md) | Value struct for a keyboard shortcut (key + Ctrl/Alt/Shift) with press-detection helpers. |
| [`Tools.cs`](../files/ClientPlugin/Settings/Tools/Tools.cs.md) | Shared utilities: PascalCase-to-label conversion and hex colour parsing/formatting. |

## How it fits together

**Startup — attribute extraction.** When [`Plugin.cs`](../files/ClientPlugin/Plugin.cs.md) calls `new SettingsGenerator()`, the generator immediately calls `ExtractAttributes`, which walks every property and method on [`Config.cs`](../files/ClientPlugin/Config.cs.md) via reflection. For each member that carries an [IElement](../files/ClientPlugin/Settings/Elements/Element.cs.md) attribute, it records an `AttributeInfo` containing the attribute (which is also the element factory), the property name, and a getter/setter `Delegate` pair bound to `Config.Current`. The type of each property is validated against `IElement.SupportedTypes`; a mismatch throws immediately at startup.

**Dialog construction.** The generator creates a [`SettingsScreen.cs`](../files/ClientPlugin/Settings/SettingsScreen.cs.md) (a `MyGuiScreenBase` subclass) with an `OnRecreateControls` callback. At this point the active layout is [`None.cs`](../files/ClientPlugin/Settings/Layouts/None.cs.md) — a no-op placeholder. When [`Plugin.cs`](../files/ClientPlugin/Plugin.cs.md) is about to push the screen onto the GUI stack, it calls `SetLayout<Simple>()`, which replaces the active layout with [`Simple.cs`](../files/ClientPlugin/Settings/Layouts/Simple.cs.md) and updates the screen's size.

**Control materialisation.** When the screen's `LoadContent` fires (i.e., the dialog is first shown), it calls `RecreateControls`, which invokes `OnRecreateControls` on the generator. The generator calls `CreateConfigControls`: for each `AttributeInfo`, it calls `info.ElementType.GetControls(name, getter, setter)`. Each [IElement](../files/ClientPlugin/Settings/Elements/Element.cs.md) implementation creates its specific `MyGuiControlBase` objects (e.g. a label + checkbox), wires the live getter/setter closures as event handlers, and wraps them in [`Control.cs`](../files/ClientPlugin/Settings/Elements/Control.cs.md) objects carrying sizing metadata. The result is a `List<List<Control>>` — one inner list per settings row.

**Layout and positioning.** `OnRecreateControls` then calls `ActiveLayout.RecreateControls()` (which creates any container controls, e.g. [`Simple.cs`](../files/ClientPlugin/Settings/Layouts/Simple.cs.md)'s scroll panel, and returns what the screen should host) and `ActiveLayout.LayoutControls()` (which computes the final position of every control using the [`Control.cs`](../files/ClientPlugin/Settings/Elements/Control.cs.md) sizing hints). Only then does [`SettingsScreen.cs`](../files/ClientPlugin/Settings/SettingsScreen.cs.md) add the returned controls to its `Controls` collection.

**Persistence.** All writes go through the getter/setter closures directly to `Config.Current` properties, which raise `PropertyChanged`. When the user closes the dialog, `SettingsScreen.OnRemoved` calls [`ConfigStorage.cs`](../files/ClientPlugin/Settings/ConfigStorage.cs.md)`.Save(Config.Current)`, serialising the current state to `Performance.cfg`.

**Cross-module interaction.** This module has no knowledge of the Harmony patches. It interacts with [client-plugin](client-plugin.md) only through [`Config.cs`](../files/ClientPlugin/Config.cs.md) (the reflected type) and through the `OpenConfigDialog` call in [`Plugin.cs`](../files/ClientPlugin/Plugin.cs.md). The patches themselves gate on `Config.Current` properties via the `IPluginConfig` interface, which is defined in the `Shared` project.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
