# Client Plugin Entry Point

The Pulsar `IPlugin` implementation that boots the plugin inside the Space Engineers game client and owns the client-side configuration object.

The `client-plugin` module is the Pulsar-facing layer of the plugin: the `IPlugin` entry point that boots Harmony and the `Config` class that holds every client-side toggle. Together they bridge the generic `Shared` patch infrastructure and the Space Engineers game client, supplying the live config the patches gate on and the settings dialog the user interacts with.

`Config` is both the data model and the UI specification: each public `bool` property is decorated with `[Separator]` and `[Checkbox]` attributes that the `client-settings` framework reflects over to build the in-game dialog automatically, with no hand-written UI code required in this module.

## Files

| File | Summary |
| --- | --- |
| [`Config.cs`](../files/ClientPlugin/Config.cs.md) | All client-side performance-fix toggles declared as `bool` properties with settings-framework attributes. |
| [`Plugin.cs`](../files/ClientPlugin/Plugin.cs.md) | Pulsar `IPlugin` entry point: bootstraps Harmony patches, the shared common state, and the settings dialog. |

## How it fits together

At startup Pulsar calls `Plugin.Init`. The plugin creates a [`SettingsGenerator.cs`](../files/ClientPlugin/Settings/SettingsGenerator.cs.md) (which immediately reflects over [`Config.cs`](../files/ClientPlugin/Config.cs.md) to build the attribute list), registers the plugin with `Common.SetPlugin` (passing `Config.Current` as the `IPluginConfig`), and runs `PatchHelpers.HarmonyPatchAll` to activate every Harmony patch in the `Shared` project. From that point forward, patches read their enable-flags directly from `Config.Current` properties.

When the user opens the settings dialog, Pulsar calls `Plugin.OpenConfigDialog`. This sets the active layout to [`Simple.cs`](../files/ClientPlugin/Settings/Layouts/Simple.cs.md) and calls `MyGuiSandbox.AddScreen` with the [`SettingsScreen.cs`](../files/ClientPlugin/Settings/SettingsScreen.cs.md) the generator holds. On close, `SettingsScreen.OnRemoved` persists the updated `Config.Current` to disk via [`ConfigStorage.cs`](../files/ClientPlugin/Settings/ConfigStorage.cs.md).

`Config` feeds two consumers: the `Shared` Harmony patches (via the `IPluginConfig` interface) and the [client-settings](client-settings.md) framework (via attribute reflection). Changes flow from the dialog → `Config.Current` property setter → `INotifyPropertyChanged` → back-end patch guard, so toggling a checkbox takes effect on the very next simulation frame without a restart (except for the fixes explicitly marked "needs restart").

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
