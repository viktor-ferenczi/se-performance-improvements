# Logging

An environment-agnostic logger abstraction with a shared message formatter, so the same patch code can log on both the client and the dedicated server.

The logging module provides an environment-agnostic, allocation-minimising logging stack for the plugin. It is designed around one rule: patch code in the `Shared` project must not import any host-specific logging framework. All patch code logs through the [`IPluginLogger.cs`](../files/Shared/Logging/IPluginLogger.cs.md) interface, which [`Common.cs`](../files/Shared/Plugin/Common.cs.md) stores and any plugin initialiser populates.

The three-file stack is a classic layering: the interface defines the API, the formatter handles allocation-efficient string building, and the concrete logger bridges to the game's output channel.

## Files

| File | Summary |
| --- | --- |
| [`IPluginLogger.cs`](../files/Shared/Logging/IPluginLogger.cs.md) | Six-level logging contract (Trace → Critical) with per-level enabled guards and optional `Exception` overloads |
| [`LogFormatter.cs`](../files/Shared/Logging/LogFormatter.cs.md) | Thread-local `StringBuilder`-based formatter that prepends a plugin prefix and appends full exception chains |
| [`PluginLogger.cs`](../files/Shared/Logging/PluginLogger.cs.md) | Concrete `IPluginLogger` backed by `MyLog.Default` |

## How it fits together

[`LogFormatter.cs`](../files/Shared/Logging/LogFormatter.cs.md) sits at the bottom of the stack. It owns the only `StringBuilder` allocation (one per thread, via `ThreadLocal<StringBuilder>`) and the only call to `string.Format`. It is a non-abstract base class with a `protected` constructor so it can only be subclassed, not used directly.

[`PluginLogger.cs`](../files/Shared/Logging/PluginLogger.cs.md) extends `LogFormatter` and implements [`IPluginLogger.cs`](../files/Shared/Logging/IPluginLogger.cs.md). Its constructor passes `"{pluginName}: "` as the prefix so every log line is unambiguously attributed. All six severity methods call `Format` to build the message string, then write to `MyLog.Default.Log` with the matching `MyLogSeverity`. The `Trace` level maps to `MyLogSeverity.Debug` because Keen does not define a Trace severity. The class is always compiled — the former `#if !TORCH` guard was removed when Torch support was dropped on this branch.

[`Plugin.cs`](../files/ServerPlugin/Plugin.cs.md) creates `new PluginLogger(Name)` and stores it as the static `Logger` field. [`Common.cs`](../files/Shared/Plugin/Common.cs.md).`SetPlugin` receives it via `plugin.Log` (the [`ICommonPlugin.cs`](../files/Shared/Plugin/ICommonPlugin.cs.md) contract) and stores it as `Common.Logger`. All patch classes then call methods like `Common.Logger.Warning(...)` without knowing which concrete implementation is active.

Routing all patch logging through the [`IPluginLogger.cs`](../files/Shared/Logging/IPluginLogger.cs.md) interface keeps the `Shared` patch code decoupled from the concrete sink: the same code logs identically whether it runs in the game client or the dedicated server, and an alternative `IPluginLogger` could be substituted without touching any patch.

Interactions with other modules: [shared-plugin-core](shared-plugin-core.md) holds and distributes the active `IPluginLogger`; [server-plugin](server-plugin.md) constructs the concrete `PluginLogger` instance.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
