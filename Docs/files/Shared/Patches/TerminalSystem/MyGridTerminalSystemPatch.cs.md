# `Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs`

*Rate-limits `MyGridTerminalSystem.UpdateGridBlocksOwnership` to suppress redundant PB access-right syncs (file is currently compiled out via `#if BUGGY`).*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyGridTerminalSystemPatch.cs`](../../../../../Shared/Patches/TerminalSystem/MyGridTerminalSystemPatch.cs) (80 lines) |
| **Kind** | Static Harmony patch class (currently disabled — file wrapped in `#if BUGGY`) |
| **Role** | Performance patch |

## Purpose

`MyGridTerminalSystem.UpdateGridBlocksOwnership` is called from `MyProgrammableBlock.RunSandboxedProgramAction` on every PB execution to refresh `IsAccessibleForProgrammableBlock` for every block in the grid. On busy servers with many active PBs this method runs far too often — ownership changes rarely, making the bulk of these calls redundant (see the "Less frequent update of PB access to blocks" section in `Docs/PerformanceFixes.md`).

A Prefix uses a [`UintCache.cs`](../../Tools/UintCache.cs.md) keyed by `GetHashCode() ^ ownerID` as an inhibitor: the first call for a given `(terminal-system, owner)` pair goes through normally and records a cache entry; subsequent calls within the TTL (approximately 4 seconds, 4 × 60 + jitter ticks) are skipped. The stored value is `(uint)ownerID` so that key collisions are detected and allowed through. The patch is governed by `Config.FixTerminal` and cleared when that flag is toggled off.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Inhibitor` | `UintCache<long>` | Rate-limiter keyed by hash of terminal system + owner; TTL ~4 s. |
| `UpdateGridBlocksOwnershipPrefix` | Prefix | Suppresses the call when a valid inhibitor entry exists; stores a new entry on the first allowed call. |
| `Configure()` / `OnConfigChanged` | Static methods | Gate the patch on `Config.FixTerminal` and clear the inhibitor on disable. |
| `Update()` | Static method | Advances inhibitor TTL cleanup each simulation tick. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyGridTerminalSystem.UpdateGridBlocksOwnership` | Prefix | Skips redundant ownership sync calls; allows through at most once per ~4 seconds per (grid terminal system, owner) pair. |

## References

- [`MyCubeBlockPatch.cs`](../Block/MyCubeBlockPatch.cs.md) — related block access-rights caching
- [`MyTerminalBlockPatch.cs`](../Block/MyTerminalBlockPatch.cs.md) — related terminal access-rights caching
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
