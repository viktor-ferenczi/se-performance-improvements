# `Shared/Patches/Character/MyCharacterPatch.cs`

*Disables server-side character footprint rendering and body-contact audio by short-circuiting the relevant branches in `MyCharacter.RigidBody_ContactPointCallback` on dedicated servers.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyCharacterPatch.cs`](../../../../../Shared/Patches/Character/MyCharacterPatch.cs) (68 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyCharacter.RigidBody_ContactPointCallback` is invoked on physics contact events for every character. On a dedicated server neither footprint decals nor contact sound effects have any effect — no rendering occurs on the server — but the game still executes the code paths that prepare and dispatch them, wasting CPU time.

The transpiler inserts two early-exit branches gated on `Sync.IsDedicated`:

1. **`DisableFootprintRenderingOnServer`** — before the block that reads `otherPhysicsBody` and would trigger footprint rendering, it inserts a `Sync.IsDedicated` check that jumps to the existing `skipRenderingFootprints` label when running as a server.
2. **`DisableBodyContactAudioOnServer`** — before the block that checks `m_canPlayImpact` and would play a contact sound, it inserts a matching `Sync.IsDedicated` check that branches to the existing `skipContactSound` label.

Both helpers target the existing skip labels in the original IL, so no new control-flow structure is introduced.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `RigidBody_ContactPointCallbackTranspiler` | Transpiler | Applies both server-skip helpers to the contact callback IL. |
| `DisableFootprintRenderingOnServer` | Private static method | Inserts `IsDedicated → skipRenderingFootprints` branch before footprint dispatch code. |
| `DisableBodyContactAudioOnServer` | Private static method | Inserts `IsDedicated → skipContactSound` branch before audio dispatch code. |
| `Configure()` | Static method | Gates the patch on `Config.Enabled && Config.FixCharacter`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyCharacter.RigidBody_ContactPointCallback` | Transpiler | Skips footprint rendering and contact-sound dispatch on dedicated servers. |

## References

- [`MyWheelPatch.cs`](../../../ServerPlugin/Patches/Wheel/MyWheelPatch.cs.md) — analogous server-side trail skip for wheels (now in the server plugin)
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
