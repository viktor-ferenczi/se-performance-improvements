# `Shared/Patches/Conveyor/MyAssemblerPatch.cs`

*Caches the assemblers reachable from a cooperative-mode assembler, so the master lookup on every production tick no longer walks the whole conveyor network.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyAssemblerPatch.cs`](../../../../../Shared/Patches/Conveyor/MyAssemblerPatch.cs) (159 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

An assembler in cooperative mode (`IsSlave`) calls `MyAssembler.GetMasterAssembler` on every production tick to find a master whose queue it can share. The game implements that as `MyGridConveyorSystem.FindReachable` from the assembler's endpoint, filtered to the other assemblers with a friendly owner, shuffled, and scanned for the first one with a non-empty queue. The walk visits every endpoint of the conveyor network under the global pathfinding lock, which the parallel item transfer computations also take. On a production base with hundreds of assemblers and thousands of conveyor blocks the main thread spends most of its simulation time in this one method, mostly waiting for the lock.

The walk depends only on the conveyor network and on block ownership. The Prefix looks the caller up in a [`Cache.cs`](../../Tools/Cache.cs.md) keyed by entity id; on a hit it makes the game's random choice over the cached candidates (shuffle, then the first assembler which is not closed, not disassembling, not a slave and has a queue) and skips the original. On a miss the original runs and the Postfix copies the reachable assemblers out of `MyAssembler.m_conveyorEndpoints`, the static list the game filled. Entries live for five seconds and carry the `MyGridConveyorSystemPatch.Generation` they were filled at, so anything that invalidates the reachability caches (block added or removed, split, merge, connector, ownership) or flags a network for recomputation (conveyor line working state, sorters) discards them at once.

Enabled by `FixConveyor`. See the *Cached master assembler lookup* part of the *Cached MyGridConveyorSystem.Reachable* section of `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Candidates` | Private class | The reachable assemblers of one caller and the conveyor generation they were collected at. |
| `Cache` | `Cache<long, Candidates>` | Keyed by the assembler's entity id; five second lifetime, cleaned up once a minute. |
| `GetMasterAssemblerPrefix` | Prefix | Serves a hit from the cache, or lets the original run and flags the Postfix. |
| `GetMasterAssemblerPostfix` | Postfix | Copies the game's freshly walked list into the cache. |
| `Pick(List<MyAssembler>)` | Static method | The game's own choice: shuffle, then the first candidate which can be a master right now. |
| `Update()` | Static method | Advances the cache clock each tick; clears the cache when `FixConveyor` is turned off. |
| `CaptureStatistics(StatisticsSnapshot)` | Static method | Reports the hit rate as `Conveyor.MasterAssembler`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyAssembler.GetMasterAssembler` | Prefix | Picks the master from the cached reachable assemblers, skipping the network walk on a hit. |
| `MyAssembler.GetMasterAssembler` | Postfix | Caches the reachable assemblers after a miss. |

## References

- [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) — the `Generation` counter the entries are validated against, and the reachability caches invalidated by the same events
- [`MyCubeGridPatchForConveyor.cs`](MyCubeGridPatchForConveyor.cs.md), [`MyCubeBlockPatchForConveyor.cs`](MyCubeBlockPatchForConveyor.cs.md), [`MyShipConnectorPatchForConveyor.cs`](MyShipConnectorPatchForConveyor.cs.md) — the invalidation hooks
- [`Cache.cs`](../../Tools/Cache.cs.md) — the expiring cache primitive
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
