# `ClientPlugin/Plugin.cs`

*Pulsar `IPlugin` entry point that initialises Harmony patches, registers the shared plugin common state, and opens the in-game settings dialog on demand.*

|  |  |
| --- | --- |
| **Module** | [Client Plugin Entry Point](../../modules/client-plugin.md) |
| **Source** | [`Plugin.cs`](../../../ClientPlugin/Plugin.cs) (123 lines) |
| **Kind** | `class Plugin : IPlugin, ICommonPlugin` |
| **Role** | Plugin entry point |

## Purpose

`Plugin` is the single class Pulsar instantiates when the *Performance Improvements* plugin is enabled. `Init` is called once at game startup: it sets up the `Common` shared state (game version, user-data path, config reference), then runs `PatchHelpers.HarmonyPatchAll` to apply all Harmony patches declared in the `Shared` project. If patching fails the `failed` flag is set and `Update` becomes a no-op for the rest of the session.

The `Update` method is called every simulation frame. In the current implementation it delegates to `CustomUpdate`, which calls `PatchHelpers.PatchUpdates()` to service any patches that require per-frame work (e.g. cache invalidation timers).

`OpenConfigDialog` is the entry point Pulsar calls when the user clicks the plugin's *Settings* button. It sets the active layout to [`Simple.cs`](Settings/Layouts/Simple.cs.md) (a scrollable single-column panel) and pushes a [`SettingsScreen.cs`](Settings/SettingsScreen.cs.md) onto the game's GUI stack via `MyGuiSandbox.AddScreen`. The [`SettingsGenerator.cs`](Settings/SettingsGenerator.cs.md) instance created in `Init` owns both the dialog and the layout, so the dialog is constructed lazily on first open and reused thereafter.

`Dispose` is called when the game exits (not guaranteed). Harmony patches are intentionally *not* un-patched here to avoid interfering with other loaded plugins.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Instance` | `static Plugin` | Singleton reference; set in `Init`, cleared in `Dispose`. |
| `Name` | `const string` | `"Performance"` — used as the Harmony ID and the logger name. |
| `Tick` | `long` property | Incremented every frame; available to patches that need a coarse frame counter. |
| `Config` | `IPluginConfig` property | Returns `ClientPlugin.Config.Current`; satisfies the `ICommonPlugin` contract. |
| `Log` | `IPluginLogger` property | A `PluginLogger` prefixed with `Name`. |
| `Init` | method | Bootstraps `Common`, applies Harmony patches, creates the [`SettingsGenerator.cs`](Settings/SettingsGenerator.cs.md). |
| `Update` | method | Per-frame pump; calls `PatchHelpers.PatchUpdates()`. |
| `OpenConfigDialog` | method | Sets layout to [`Simple.cs`](Settings/Layouts/Simple.cs.md) and pushes the [`SettingsScreen.cs`](Settings/SettingsScreen.cs.md). |
| `Dispose` | method | Clears `Instance`; does not un-patch Harmony. |

## References

- [`Config.cs`](Config.cs.md) — the config object exposed as `IPluginConfig`
- [`SettingsGenerator.cs`](Settings/SettingsGenerator.cs.md) — owns the dialog and layout; instantiated in `Init`
- [`Simple.cs`](Settings/Layouts/Simple.cs.md) — the layout activated when the dialog opens
- [`SettingsScreen.cs`](Settings/SettingsScreen.cs.md) — the `MyGuiScreenBase` subclass pushed onto the GUI stack

---

*[Handbook](../../TOC.md) · [Module: Client Plugin Entry Point](../../modules/client-plugin.md) · [Index](../../Index.md)*
