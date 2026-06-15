# `Shared/Logging/LogFormatter.cs`

*Thread-safe, allocation-minimising log message formatter that prepends a plugin prefix and appends full exception chains.*

|  |  |
| --- | --- |
| **Module** | [Logging](../../../modules/logging.md) |
| **Source** | [`LogFormatter.cs`](../../../../Shared/Logging/LogFormatter.cs) (88 lines) |
| **Kind** | Class (abstract base) |
| **Role** | Log message formatter |

## Purpose

`LogFormatter` provides the `Format(Exception, string, object[])` method used by [`PluginLogger.cs`](PluginLogger.cs.md) to assemble the final log string before handing it to the game's `MyLog`. It is the only place in the logging stack where `string.Format` is called and where `StringBuilder` is allocated.

To avoid per-call allocation on hot paths, it maintains a `ThreadLocal<StringBuilder>` — one reused instance per thread. The prefix string (e.g. `"Performance: "`) is injected at construction time and prepended to every message. When an `Exception` is supplied, `FormatException` appends the full chain of inner exceptions, up to a hard limit of 100 nesting levels, including exception type, message, `TargetSite`, custom `Data` entries, and the stack trace. A guard message is appended if the depth limit is hit.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `LogFormatter(string prefix)` | Constructor | Stores the plugin-name prefix prepended to every formatted message. |
| `Format(Exception, string, object[])` | Protected method | Produces the final log string: prefix + formatted message + optional exception chain. Uses the thread-local `StringBuilder`. |
| `FormatException(StringBuilder, Exception)` | Private static method | Appends type, message, `TargetSite`, `Data`, and stack trace for each exception in the chain. |
| `threadLocalStringBuilder` | Field | `ThreadLocal<StringBuilder>` ensuring one `StringBuilder` per thread, reused across calls. |

## References

- [`PluginLogger.cs`](PluginLogger.cs.md) — the only subclass; calls `Format` on every log method
- [`IPluginLogger.cs`](IPluginLogger.cs.md) — the interface `PluginLogger` implements using this formatter

---

*[Handbook](../../../TOC.md) · [Module: Logging](../../../modules/logging.md) · [Index](../../../Index.md)*
