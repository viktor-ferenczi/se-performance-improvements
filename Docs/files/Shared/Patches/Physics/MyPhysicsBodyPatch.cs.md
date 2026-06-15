# `Shared/Patches/Physics/MyPhysicsBodyPatch.cs`

*Optimizes the `MyPhysicsBody.RigidBody` property getter by removing a redundant parent-body load sequence.*

|  |  |
| --- | --- |
| **Module** | [Physics Patches](../../../../modules/physics.md) |
| **Source** | [`MyPhysicsBodyPatch.cs`](../../../../../Shared/Patches/Physics/MyPhysicsBodyPatch.cs) (49 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyPhysicsBody.RigidBody` is a getter called many times per simulation tick from physics queries and entity updates. The original implementation loads the parent body reference from a field, tests it for null, and — if not null — loads the field a second time to read the parent's `RigidBody`. This double load is unnecessary; the first load result can be reused via `Dup`.

The transpiler removes the redundant triple-instruction sequence (two loads + a branch target consume) that follows the null branch and replaces it with the stacked value already present from the `Dup` instruction it inserts before the `brtrue.s`. This eliminates one field-access instruction on the hot non-null path and is gated on `Config.FixPhysics`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Configure()` | Static method | Sets `enabled` from `Config.FixPhysics`. |
| `RigidBodyGetterTranspiler` | Harmony Transpiler | Inserts `dup` before the null check and removes the redundant field reload after it, tightening the getter's IL. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyPhysicsBody.RigidBody` (getter) | Transpiler | Eliminates the duplicate parent-body field load on the non-null path. |

## References

- [physics](../../../../modules/physics.md)
- [`TranspilerHelpers.cs`](../../Tools/TranspilerHelpers.cs.md)
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Physics Patches](../../../../modules/physics.md) · [Index](../../../../Index.md)*
