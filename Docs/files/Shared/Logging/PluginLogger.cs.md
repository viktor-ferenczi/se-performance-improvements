# `Shared/Logging/PluginLogger.cs`

*Concrete [`IPluginLogger.cs`](IPluginLogger.cs.md) implementation that writes to the game's `MyLog` via [`LogFormatter.cs`](LogFormatter.cs.md).*

|  |  |
| --- | --- |
| **Module** | [Logging](../../../modules/logging.md) |
| **Source** | [`PluginLogger.cs`](../../../../Shared/Logging/PluginLogger.cs) (110 lines) |
| **Kind** | Class extending `LogFormatter`, implementing `IPluginLogger` |
| **Role** | Logger implementation |

## Purpose

`PluginLogger` bridges the [`IPluginLogger.cs`](IPluginLogger.cs.md) abstraction to the Space Engineers game log (`MyLog.Default`). It inherits [`LogFormatter.cs`](LogFormatter.cs.md) for allocation-efficient message formatting and delegates every log call to `MyLog.Default.Log` with the appropriate `MyLogSeverity`.

Because the Keen engine has no `Trace` severity, `Trace` calls are mapped to `MyLogSeverity.Debug`. All six severity levels are always enabled as long as `MyLog.Default.LogEnabled` is true; there is no compile-time or runtime level filter beyond that. Every method is marked `AggressiveInlining` to minimise overhead on hot patch code paths.

The class is always compiled — the former `#if !TORCH` guard was removed when Torch support was dropped on this branch (the server plugin now targets Magnetar only), so the Keen `MyLog` is always the logging backend.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `PluginLogger(string pluginName)` | Constructor | Passes `"{pluginName}: "` as the prefix to [`LogFormatter.cs`](LogFormatter.cs.md). |
| `IsTraceEnabled` … `IsCriticalEnabled` | Properties | All delegate to `MyLog.Default.LogEnabled`. |
| `Trace(Exception, string, object[])` | Method | Maps Trace to `MyLogSeverity.Debug` (Keen has no Trace level). |
| `Debug` / `Info` / `Warning` / `Error` / `Critical` | Methods (×2 each) | Forward to `MyLog.Default.Log` with the matching severity after calling `Format`. |

## References

- [`IPluginLogger.cs`](IPluginLogger.cs.md) — interface this class implements
- [`LogFormatter.cs`](LogFormatter.cs.md) — base class providing the thread-local `Format` method
- [`Common.cs`](../Plugin/Common.cs.md) — stores the logger instance created by [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md)
- [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) — constructs `new PluginLogger(Name)` and stores it as `Logger`

---

*[Handbook](../../../TOC.md) · [Module: Logging](../../../modules/logging.md) · [Index](../../../Index.md)*
