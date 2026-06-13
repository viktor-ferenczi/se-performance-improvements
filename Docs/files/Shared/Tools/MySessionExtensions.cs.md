# `Shared/Tools/MySessionExtensions.cs`

*Extension methods on `MySession` exposing the internal `m_updateAllowed` flag through publicized direct access.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`MySessionExtensions.cs`](../../../../Shared/Tools/MySessionExtensions.cs) (20 lines) |
| **Kind** | Static utility class `MySessionExtensions` |
| **Role** | Misc utility |

## Purpose

The "Updates while pasting grids" fix (see `Docs/PerformanceFixes.md`) works by temporarily setting `MySession.Static.m_updateAllowed = false` while `MyCubeGrid.PasteBlocksServer` is running, preventing the game engine from processing all pending simulation ticks during the paste operation. This eliminates a large amount of redundant computation.

`m_updateAllowed` is a private field; `MySessionExtensions` wraps it via two thin extension methods using the publicized `Sandbox.Game` assembly (see [`GameAssembliesToPublicize.cs`](GameAssembliesToPublicize.cs.md)). The extension-method approach keeps the calling patch code readable without having to use `AccessTools` reflection.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `IsUpdateAllowed(this MySession)` | Extension method | Reads `m_updateAllowed` from the session object. |
| `SetUpdateAllowed(this MySession, bool)` | Extension method | Writes `m_updateAllowed`; set to `false` by the paste-grid patch and restored to `true` when the patch exits. |

## References

- [`GameAssembliesToPublicize.cs`](GameAssembliesToPublicize.cs.md) — declares `Sandbox.Game` as publicized, enabling direct access to `m_updateAllowed`.
- [`IgnoresAccessChecksToAttribute.cs`](IgnoresAccessChecksToAttribute.cs.md) — runtime companion that lets the CLR accept the direct field access.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
