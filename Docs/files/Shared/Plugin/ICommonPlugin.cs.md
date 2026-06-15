# `Shared/Plugin/ICommonPlugin.cs`

*Contract that both client and server plugin classes must implement so [`Common.cs`](Common.cs.md) can accept either without a circular assembly reference.*

|  |  |
| --- | --- |
| **Module** | [Shared Plugin Core](../../../modules/shared-plugin-core.md) |
| **Source** | [`ICommonPlugin.cs`](../../../../Shared/Plugin/ICommonPlugin.cs) (11 lines) |
| **Kind** | Interface |
| **Role** | Plugin abstraction contract |

## Purpose

`ICommonPlugin` is the minimal surface the `Shared` assembly needs from whichever host plugin (client or server) is active. By depending only on this interface, the `Shared` project compiles independently of `ClientPlugin` and `ServerPlugin`.

[`Common.cs`](Common.cs.md).`SetPlugin` accepts an `ICommonPlugin` and extracts the three members it needs: a logger, a config, and the current tick counter. All Harmony patch classes then read these through the static properties on [`Common.cs`](Common.cs.md) rather than referencing the concrete plugin class directly.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Log` | Property | The active [`IPluginLogger.cs`](../Logging/IPluginLogger.cs.md) for this environment. |
| `Config` | Property | The active [`IPluginConfig.cs`](../Config/IPluginConfig.cs.md) containing all feature toggles. |
| `Tick` | Property | Monotonically increasing update counter, used by patches for time-based rate limiting. |

## References

- [`Common.cs`](Common.cs.md) — holds the single `ICommonPlugin` reference after `SetPlugin` is called
- [`IPluginLogger.cs`](../Logging/IPluginLogger.cs.md) — returned by `Log`
- [`IPluginConfig.cs`](../Config/IPluginConfig.cs.md) — returned by `Config`
- [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) — server-side implementation of this interface

---

*[Handbook](../../../TOC.md) · [Module: Shared Plugin Core](../../../modules/shared-plugin-core.md) · [Index](../../../Index.md)*
