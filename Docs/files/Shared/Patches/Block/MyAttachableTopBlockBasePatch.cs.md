# `Shared/Patches/Block/MyAttachableTopBlockBasePatch.cs`

*Runs the stator reattach mark of a mechanical top on the main thread, so a top initialized on a worker thread can't tear its stator grid's attach-state set (issue #161).*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyAttachableTopBlockBasePatch.cs`](../../../../../Shared/Patches/Block/MyAttachableTopBlockBasePatch.cs) |
| **Kind** | Static Harmony patch class (transpiler), always applied |
| **Role** | Crash fix |

## Purpose

`MyAttachableTopBlockBase.Init` looks up the top's stator by `ParentEntityId` and calls `MarkForReattach` on it. Grids a client streams in, and grids made with `MyEntities.CreateFromObjectBuilderParallel`, run `Init` on a worker thread while the stator's grid may already be in the scene. The call then adds the stator to `MyCubeGrid.BlocksWaitingOnAttachUpdate` from the worker, while the main thread changes the same `HashSet` from the stators' `UpdateOnceBeforeFrame`. A set torn by the race throws `NullReferenceException` in `HashSet.AddIfNotPresent` on the main thread and the game crashes, with `MechanicalConnectionBlockAttachUpdateStatusChanged` on top of the stack.

The transpiler replaces that one call with `MarkForReattach(stator)` of the patch class, which calls the original directly on the main thread and otherwise queues it with `MySandboxGame.Static.Invoke`. The queued call runs before the completion callback that the same `Init` queues afterwards, so the stator is still marked before the new top enters the scene.

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyAttachableTopBlockBase.Init` | Transpiler | Redirects `MyMechanicalConnectionBlockBase.MarkForReattach` to the main thread. |

## Testing

`Tools/AttachRace` (see `Tools/README.md`) makes rotor tops on worker threads against in-scene stators on an isolated Magnetar server and counts the attach-state changes made off the main thread.

---
