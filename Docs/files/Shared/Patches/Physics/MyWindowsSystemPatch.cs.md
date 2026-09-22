# `Shared/Patches/Physics/MyWindowsSystemPatch.cs`

*Postfix answering `OptimalHavokThreadCount`, which decides how many worker threads the Havok physics job pool gets.*

|  |  |
| --- | --- |
| **Module** | [Physics Patches](../../../../modules/physics.md) |
| **Source** | [`MyWindowsSystemPatch.cs`](../../../../../Shared/Patches/Physics/MyWindowsSystemPatch.cs) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyPhysics.LoadData` sizes the Havok job thread pool from `MyVRage.Platform.System.OptimalHavokThreadCount`, falling back to the parameterless `HkJobThreadPool()` when that property is `null` — which it always is, since `MyWindowsSystem` answers a hard `=> null`. Havok then sizes the pool from the machine on its own terms (7 worker threads on a 16 logical processor host, measured).

That property is the game's own extension point for this answer, and `MyPhysics.LoadData` is the only place it is read, so a postfix on it is the whole fix. The job queue follows, because `MyPhysics` derives its size from the pool's thread count.

Two configuration options drive it, both under `Config.FixPhysics`: `HavokThreadCountMode` (`Auto` or `Manual`) and `HavokThreadCount`. In `Auto` the count is `HavokThreads.Auto` — one worker per logical processor, capped at 16 — and `Configure()` writes it back into `Config.HavokThreadCount`, so the in-game dialog and the server's web UI always show the count the game is going to get. In `Manual` the configured number is used, clamped to `HavokThreads.Min`..`HavokThreads.Max` (2..64). With the physics fix off the patch answers nothing and the game's own sizing stands.

The minimum is 2 rather than 1 because a pool of one is not single threaded physics: the worlds are still created and stepped through Havok's multithreaded path, so one worker pays the whole job dispatch with nothing to overlap it with. Running physics without threading is `MyFakes.ENABLE_HAVOK_MULTITHREADING`, a different switch which the game exposes to admins through `MyPhysics.SetScheduling`.

The pool is built at world load, so a changed count needs a restart (or a world reload) to take effect.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `threadCount` | `int` | The resolved count; `0` means "leave the game's own sizing alone". |
| `Configure()` / `OnConfigChanged()` | Static methods | Resolve the count from the config, subscribe once, and sync `Config.HavokThreadCount` with the effective number. |
| `OptimalHavokThreadCountGetterPostfix` | Harmony Postfix | Replaces the property's `null` with the resolved count. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyWindowsSystem.OptimalHavokThreadCount` (getter) | Postfix | Answers the Havok thread count the plugin resolved, instead of `null`. |

## References

- [physics](../../../../modules/physics.md)
- [`IPluginConfig.cs`](../../Config/IPluginConfig.cs.md) — `HavokThreadCountMode` and the `HavokThreads` range/automatic value.
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Physics Patches](../../../../modules/physics.md) · [Index](../../../../Index.md)*
