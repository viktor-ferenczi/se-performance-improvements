# `Shared/Patches/Toolbar/MyToolbarItemTerminalGroupPatch.cs`

*Caches the terminal actions collected for a block group on a cockpit toolbar, so the per-frame toolbar refresh stops walking every block of the group.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyToolbarItemTerminalGroupPatch.cs`](../../../../../Shared/Patches/Toolbar/MyToolbarItemTerminalGroupPatch.cs) (149 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch (client only) |

## Purpose

While the player sits in a cockpit, `MyToolbarComponent` refreshes every toolbar item on every frame. A block group item (`MyToolbarItemTerminalGroup.Update`) rebuilds the group's block list and calls `GetActionsWithGenericDuplicates` to collect the terminal actions valid for the group: a walk over every block of the group and over every component of every block, plus a `List` and `HashSet` allocation or two. For groups of hundreds or thousands of blocks (a "Refineries" group on a production base) that is milliseconds per frame, and the result only changes when the membership of the group changes.

The Prefix caches the collected `ListReader<ITerminalAction>` and the `genericType` flag per toolbar item for one second in a [`Cache.cs`](../../Tools/Cache.cs.md), keyed by the item's identity and validated against a fingerprint of the block list (its count and the entity ids in order), so a block added to or removed from the group is picked up on the next frame regardless of the cache age. On a hit the original is skipped; on a miss the Postfix stores the fresh result. The rest of the item refresh (the enabled state, icons and the value text) still runs on every frame from the live blocks. The dedicated server never updates toolbars, so the patch is inert there and the `FixToolbar` option is a stub in [`PerformanceConfig.cs`](../../../ServerPlugin/Config/PerformanceConfig.cs.md).

See the *Toolbar block group actions* section of `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Cache` | `Cache<int, Actions>` | Per toolbar item entries (fingerprint, actions, generic flag), one second lifetime, cleaned up every ten seconds. |
| `Fingerprint(ListReader<MyTerminalBlock>)` | Static method | FNV-1a style hash of the block count and entity ids; the cache key check. |
| `GetActionsWithGenericDuplicatesPrefix` | Prefix | Returns the cached actions and skips the original on a hit; passes the key and fingerprint to the Postfix on a miss. |
| `GetActionsWithGenericDuplicatesPostfix` | Postfix | Stores the freshly collected actions. |
| `Update()` | Static method | Advances the cache clock each tick; clears the cache when `FixToolbar` is turned off. |
| `CaptureStatistics(StatisticsSnapshot)` | Static method | Reports the hit rate as `Toolbar.GroupActions`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyToolbarItemTerminalGroup.GetActionsWithGenericDuplicates` | Prefix | Returns the cached action list for an unchanged block group, skipping the walk. |
| `MyToolbarItemTerminalGroup.GetActionsWithGenericDuplicates` | Postfix | Caches the collected actions after a miss. |

## References

- [`Cache.cs`](../../Tools/Cache.cs.md) — the expiring cache primitive
- [`CacheStat.cs`](../../Tools/CacheStat.cs.md) — hit rate counters published through [`Statistics.cs`](../../Stats/Statistics.cs.md)
- [Config](../../../ClientPlugin/Config.cs.md) — the `FixToolbar` toggle
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
