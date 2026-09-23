# `Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs`

*Skips the per-run refresh of `IsAccessibleForProgrammableBlock` on every terminal block when the owner, the blocks and their ownership have not changed since the last run.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyGridTerminalSystemPatch.cs`](../../../../../Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs) (162 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyGridTerminalSystem.UpdateGridBlocksOwnership` runs before every programmable block execution and sets `IsAccessibleForProgrammableBlock` on every terminal block of the grid group from the owner's access rights (`HasPlayerAccessWithNobodyCheck`). With a script running every tick on a large grid that walk is most of the main thread's time (42% on a 33000 block grid). See the *Skipping redundant updates of PB access to blocks* section in `Docs/PerformanceFixes.md`.

The Prefix keeps one entry per terminal system in a `ConditionalWeakTable` (keyed by the instance itself, so a hash collision can never skip a needed walk) with the owner the flags were last computed for, the generation they were computed at and an expiry. A call with the same owner and generation before the expiry is skipped; otherwise the original runs and the Postfix records it. The generation is bumped by Postfixes on `MyGridTerminalSystem.Add` / `Remove` and on `MyCubeGrid.NotifyBlockOwnershipChange` / `ChangeGridOwnership`, which covers everything but faction relation and admin setting changes; those are covered by the two second expiry. Gated by `Config.FixTerminal`; the hit rate is published as `Terminal.PbAccess`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Applied` | Private class | Owner, generation and expiry of the last walk of one terminal system. |
| `Entries` | `ConditionalWeakTable<MyGridTerminalSystem, Applied>` | The per terminal system entries; die with the terminal system. |
| `generation` | `long` | Bumped by the invalidation Postfixes. |
| `UpdateGridBlocksOwnershipPrefix` / `Postfix` | Prefix / Postfix | Skip the walk on a valid entry; record the walk otherwise. |
| `AddPostfix`, `RemovePostfix`, `NotifyBlockOwnershipChangePostfix`, `ChangeGridOwnershipPostfix` | Postfixes | Invalidate every entry. |
| `CaptureStatistics(StatisticsSnapshot)` | Static method | Reports the hit rate as `Terminal.PbAccess`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyGridTerminalSystem.UpdateGridBlocksOwnership` | Prefix | Skips the walk when nothing it depends on changed. |
| `MyGridTerminalSystem.UpdateGridBlocksOwnership` | Postfix | Records the owner and generation of a walk that ran. |
| `MyGridTerminalSystem.Add` / `Remove` | Postfix | Invalidate (blocks joined or left a terminal system). |
| `MyCubeGrid.NotifyBlockOwnershipChange` / `ChangeGridOwnership` | Postfix | Invalidate (ownership or share mode changed). |

## References

- [`CacheStat.cs`](../../Tools/CacheStat.cs.md) — hit rate counters published through [`Statistics.cs`](../../Stats/Statistics.cs.md)
- [`MyCubeGridPatchForConveyor.cs`](../Conveyor/MyCubeGridPatchForConveyor.cs.md) — hooks the same ownership events for the conveyor caches
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
