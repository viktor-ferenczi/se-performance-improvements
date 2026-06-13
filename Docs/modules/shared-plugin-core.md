# Shared Plugin Core

Code shared by both the client and server plugins: the `Common` bootstrap that applies patches and wires up logging/config, plus the `ICommonPlugin` and `IPluginConfig` contracts.

The shared plugin core is the glue layer that lets the `Shared` project operate without any knowledge of the concrete client or server plugin. It defines two contracts ([`ICommonPlugin.cs`](../files/Shared/Plugin/ICommonPlugin.cs.md) and [`IPluginConfig.cs`](../files/Shared/Config/IPluginConfig.cs.md)) and one bootstrap class ([`Common.cs`](../files/Shared/Plugin/Common.cs.md)) that holds the live instances of those contracts after either host plugin calls `Common.SetPlugin`.

Every Harmony patch class in the `Shared` project reads `Common.Config`, `Common.Logger`, and `Common.Plugin.Tick` through the static properties on [`Common.cs`](../files/Shared/Plugin/Common.cs.md). This means the entire patch library is host-agnostic: the same assembly runs unchanged on both the Pulsar client and the Magnetar / Quasar dedicated server.

## Files

| File | Summary |
| --- | --- |
| [`IPluginConfig.cs`](../files/Shared/Config/IPluginConfig.cs.md) | Shared configuration contract: one boolean toggle per performance fix, plus `INotifyPropertyChanged` |
| [`Common.cs`](../files/Shared/Plugin/Common.cs.md) | Static bootstrap that wires logger/config/directories and triggers Harmony patch registration |
| [`ICommonPlugin.cs`](../files/Shared/Plugin/ICommonPlugin.cs.md) | Minimal interface the host plugin must implement to hand its logger, config, and tick counter to `Common` |

## How it fits together

The dependency direction is strictly one-way: the server (`ServerPlugin`) and client (`ClientPlugin`) assemblies both depend on `Shared`; `Shared` depends on neither.

`Common.SetPlugin` is the seam. The caller (e.g. [`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md)) passes `this` (an [`ICommonPlugin.cs`](../files/Shared/Plugin/ICommonPlugin.cs.md)) together with the game version string and the platform storage directory. `Common` extracts `Log` and `Config` from the plugin reference and stores them as static properties. It then derives the three well-known filesystem paths, runs cache and debug-dump cleanup, and finally calls `PatchHelpers.Configure()`.

`IPluginConfig` is the only configuration object ever seen by patch code. Both [`PerformanceConfig.cs`](../files/ServerPlugin/Config/PerformanceConfig.cs.md) (server) and the client's `ClientPlugin.Config` implement it. Patches gate their behaviour with `Common.Config.Enabled && Common.Config.FixXxx`, which is the standard two-level guard used throughout the patch classes.

`ICommonPlugin` is a thin envelope: it adds nothing beyond grouping `Log`, `Config`, and `Tick` into one object so `Common.SetPlugin` can accept a single argument. After `SetPlugin` returns, `Common.Plugin` holds the reference should any code need the raw tick counter.

Interactions with other modules: [logging](logging.md) supplies the [`IPluginLogger.cs`](../files/Shared/Logging/IPluginLogger.cs.md) implementation; [server-plugin](server-plugin.md) (and the client equivalent) supply the [`IPluginConfig.cs`](../files/Shared/Config/IPluginConfig.cs.md) implementation and call `SetPlugin`; [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) is the downstream consumer that registers all Harmony patches after `Configure()` and `HarmonyPatchAll`.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
