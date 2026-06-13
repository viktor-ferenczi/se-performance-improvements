# `ServerPlugin/Patches/Wheel/MyWheelPatch.cs`

*Suppresses wheel-trail generation on dedicated servers by short-circuiting `MyWheel.CheckTrail` when `Sync.IsDedicated` is true. Lives in the server plugin only.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyWheelPatch.cs`](../../../../../ServerPlugin/Patches/Wheel/MyWheelPatch.cs) (29 lines) |
| **Kind** | Static Harmony patch class (compiled into the server plugin only) |
| **Role** | Performance patch |

## Purpose

`MyWheel.CheckTrail` runs each update for every wheel block to generate visual tyre-track decals on terrain voxels. On a dedicated server these decals are never rendered, so the work is entirely wasted.

This patch used to live in `Shared/` behind an `#if TORCH || DEDICATED` compile guard; it now lives in the **server plugin** ([server-plugin](../../../../modules/server-plugin.md)) instead, so it is only ever compiled into the server build. It still guards itself at runtime: the Prefix returns `true` (run original) when `!Sync.IsDedicated`, otherwise returns `!Config.FixWheelTrail` — i.e. it skips the original (`false`) only on a dedicated server with the fix enabled. The check is a single branch with no shared state.

The matching client-side flag `Config.FixWheelTrail` is now a hard-coded `false` stub with no checkbox (see [`Config.cs`](../../../ClientPlugin/Config.cs.md)), because the trail tracking only ever runs on the server.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Config` | Static property | Shortcut to `Common.Config` ([`IPluginConfig.cs`](../../../Shared/Config/IPluginConfig.cs.md)). |
| `CheckTrailPrefix` | Harmony Prefix | Returns `false` (skip original) when running as a dedicated server with `FixWheelTrail` enabled; `true` otherwise. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyWheel.CheckTrail` | Prefix | Skips wheel-trail generation entirely on dedicated servers. Verified against game IL via [`EnsureCode.cs`](../../../Shared/Tools/EnsureCode.cs.md) hash `b3d278e9`. |

## References

- [`MyLcdSurfaceComponentPatch.cs`](../TextPanel/MyLcdSurfaceComponentPatch.cs.md) — the sibling server-only no-op prefix, relocated the same way
- [`MyCharacterPatch.cs`](../../../Shared/Patches/Character/MyCharacterPatch.cs.md) — analogous server-side footprint skip for characters
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
