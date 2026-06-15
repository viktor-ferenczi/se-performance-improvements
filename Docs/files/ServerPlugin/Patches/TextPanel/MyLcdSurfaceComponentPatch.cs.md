# `ServerPlugin/Patches/TextPanel/MyLcdSurfaceComponentPatch.cs`

*Suppresses `MyLcdSurfaceComponent.UpdateVisibility` on dedicated servers, where LCD surface visibility updates have no effect. Lives in the server plugin only.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyLcdSurfaceComponentPatch.cs`](../../../../../ServerPlugin/Patches/TextPanel/MyLcdSurfaceComponentPatch.cs) (29 lines) |
| **Kind** | Static Harmony patch class (compiled into the server plugin only) |
| **Role** | Performance patch |

## Purpose

`MyLcdSurfaceComponent.UpdateVisibility` is called each update for every LCD surface component to refresh its render visibility state. On a dedicated server no rendering takes place, so this work is wasted CPU time.

This patch used to live in `Shared/` behind an `#if TORCH || DEDICATED` compile guard; it now lives in the **server plugin** ([server-plugin](../../../../modules/server-plugin.md)) instead, so it is only ever compiled into the server build. It still guards itself at runtime: the Prefix returns `true` (run original) when `!Sync.IsDedicated`, otherwise returns `!Config.FixTextPanel` — i.e. it skips the original (`false`) only on a dedicated server with the fix enabled. The check is a single inlined branch with no shared state or cache.

The matching client-side flag `Config.FixTextPanel` is now a hard-coded `false` stub with no checkbox (see [`Config.cs`](../../../ClientPlugin/Config.cs.md)), because the visibility update only matters on the server.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Config` | Static property | Shortcut to `Common.Config` ([`IPluginConfig.cs`](../../../Shared/Config/IPluginConfig.cs.md)). |
| `UpdateVisibilityPrefix` | Harmony Prefix | Returns `false` (skip original) when running as a dedicated server with `FixTextPanel` enabled; `true` otherwise. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyLcdSurfaceComponent.UpdateVisibility` | Prefix | Skips the LCD visibility update entirely on dedicated servers. Verified against game IL via [`EnsureCode.cs`](../../../Shared/Tools/EnsureCode.cs.md) hash `83495656`. |

## References

- [`MyWheelPatch.cs`](../Wheel/MyWheelPatch.cs.md) — the sibling server-only no-op prefix, relocated the same way
- [`MyCharacterPatch.cs`](../../../Shared/Patches/Character/MyCharacterPatch.cs.md) — analogous server-side no-op (transpiler) pattern
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
