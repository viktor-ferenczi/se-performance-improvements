# `Shared/Logging/IPluginLogger.cs`

*Environment-agnostic logging contract used by all patch code in the `Shared` project.*

|  |  |
| --- | --- |
| **Module** | [Logging](../../../modules/logging.md) |
| **Source** | [`IPluginLogger.cs`](../../../../Shared/Logging/IPluginLogger.cs) (27 lines) |
| **Kind** | Interface |
| **Role** | Logger abstraction |

## Purpose

`IPluginLogger` defines a six-level structured logging API (Trace, Debug, Info, Warning, Error, Critical) that all Harmony patch code and shared utilities target. By programming to this interface rather than any concrete logger, the `Shared` project remains independent of the game's `MyLog` and of any host-specific logging framework.

Each level exposes two overloads: one that accepts an optional `Exception` followed by a message and format arguments, and a convenience overload without the exception parameter. The boolean `IsXxxEnabled` guards allow callers to skip expensive string formatting when the level is suppressed.

[`Common.cs`](../Plugin/Common.cs.md) stores the active implementation in `Common.Logger`, which is populated by [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) (or the client plugin) during `Init`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `IsTraceEnabled` … `IsCriticalEnabled` | Properties | Level-enabled guards; callers check these before building expensive format strings. |
| `Trace` / `Debug` / `Info` / `Warning` / `Error` / `Critical` | Methods (×2 each) | Log at the named severity, with or without an attached `Exception`. |

## References

- [`PluginLogger.cs`](PluginLogger.cs.md) — the concrete implementation backed by `MyLog`
- [`LogFormatter.cs`](LogFormatter.cs.md) — base class of `PluginLogger` that handles message formatting
- [`Common.cs`](../Plugin/Common.cs.md) — stores the active `IPluginLogger` in `Common.Logger`
- [`ICommonPlugin.cs`](../Plugin/ICommonPlugin.cs.md) — exposes `IPluginLogger` via `Log`

---

*[Handbook](../../../TOC.md) · [Module: Logging](../../../modules/logging.md) · [Index](../../../Index.md)*
