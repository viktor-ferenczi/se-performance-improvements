# Performance Fixes — Technical Details

This document describes each performance fix the plugin applies, what it does and
why. Every fix has a matching toggle in the plugin configuration (the in-game
dialog on the client, Quasar on the server). It is safe to change most of these
options during the game; the ones that say **needs restart** only take effect
after restarting the game or the server.

For the focused overview and installation instructions see the [README](../README.md).

## Conveyor updates while merging grids

Disables the `MyConveyorLine.UpdateIsWorking` method while any grid merging
operation is in progress. It considerably reduces the merge time of grids with
long conveyor systems. At the end of `MyCubeGrid.MergeGridInternal` it calls
`GridSystems.ConveyorSystem.FlagForRecomputation()` on the grid to force
recalculating all `IsWorking` values to fix any side-effects of the optimization.

## Updates while pasting grids

Disables updates while pasting grids by setting `MySession.Static.m_updateAllowed`
to `false` while `MyCubeGrid.PasteBlocksServer` is running. It eliminates a lot of
unnecessary computations until the paste is done.

This one and the previous fix combined make grid merge and paste operations
~60-70% faster in heavy test worlds, at least for grids with lots of blocks and
conveyor ports. It adds up on multiplayer servers, especially if NPCs are pasted
automatically.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/22823-performance-unnecessary-updates-during-grid-merge-and-paste-operations)

## EOS P2P UpdateStats

Eliminates 98% of the ~50% constant CPU core load imposed by the
`VRage.EOS.MyP2PQoSAdapter.UpdateStats` method, even during **offline** games.
It is done by replacing 49 out of 50 calls with a `Thread.Sleep(1)`. It limits the
outer loop's frequency to less than 1000/s and spends less CPU power on gathering
statistics.

It makes the game faster only if you have 4 or fewer CPU cores, since this method
is called repeatedly in a loop on its own thread. It still helps to reduce CPU
power consumption and cache misses if you have more than 4 cores.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/22802-performance-constant-50-core-load-by-vrage-eos-myp2pqosadapter-updatestats)

## GC.Collect calls

*Contributed by zznty.*

The game makes explicit calls to `GC.Collect`, which may cause long pauses while
starting or stopping large worlds. It mostly affects large multiplayer servers
where worlds are big, but it can shave off a few hundred milliseconds of world
load (and close) time when loading offline games as well.

There are also calls elsewhere, for example in `MyPlanetTextureMapProvider` and
`MySimpleProfiler.LogPerformanceTestResults`, which may be invoked during
gameplay. The patched calls are logged at the DEBUG log level.

Parallel GC should happen later and free up memory anyway. Consider disabling
this setting if your PC or server does not have at least 8 GB RAM.

## Mod API call statistics overhead

*Contributed by zznty.*

It may be a performance hog if many mods are used. This fix disables the
`VRage.Scripting.Rewriters.PerfCountingRewriter.Rewrite` method, so the API calls
are not rewritten, removing the overhead.

Measured 10% lower simulation CPU load in a heavily modded test world after
loading it with this fix enabled.

## Lag on grid group changes

There is serious lag on connector lock/unlock and rotor head attach/detach due to
grid group changes causing massive main thread workload, which could easily be
deferred to worker threads with minimal consequences.

This fix disables resource updates while grids are being moved between groups and
marks those resources for updating by a worker thread later.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/23278-lag-on-connector-lockunlock-and-rotor-head-attachdetach-due-to-grid-group-changes)

## Caching compiled mods and in-game scripts

Compiling all mods and PB scripts on world load is very time consuming and CPU
intensive. It takes a lot of time to load a world which uses many mods and/or
in-game scripts. It mainly affects large multiplayer servers, but advanced single
player worlds can be affected by slow world loading too.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/23906-performance-cache-compiled-mods-and-in-game-scripts)

## MySafeZone caching and optimizations

### Caching the result of MySafeZone.IsSafe

`MySafeZone.IsSafe` is called very frequently for entities inside safe zones. This
is quite a bit of overhead in multiplayer worlds with many small grids and safe
zones.

The workaround is to cache the result of `MySafeZone.IsSafe` for up to 128
simulation ticks (~2 seconds). A side effect is that grid ownership changes are
reflected in safe zone behavior only up to 2 seconds later (1 second on average).

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/24146-performance-mysafezone-issafe-is-called-frequently-but-not-cached)

### Optimized MySafeZone.IsOutside

`MySafeZone.IsOutside()` is implemented in a convoluted way. Replaced it with an
optimized implementation which does not instantiate any new bounding boxes.

Only the `MySafeZone.IsOutside(BoundingBoxD aabb)` override is replaced, because it
caused issues with many grids around safe zones.

### Caching the result of MySafeZone.IsActionAllowed

Due to the high call counts of busy servers this method benefits from caching. The
result is cached for 2 seconds, therefore the effect of changes in safe zone
configuration or grid safe-zone containment is delayed by up to 2 seconds, which is
acceptable considering the overall performance benefits.

## Reducing frequent memory allocations

Game update 1.202.066 (Automaton) attempted to fix
[the slowness](https://support.keenswh.com/spaceengineers/pc/topic/24210-performance-pre-calculate-or-cache-mydefinitionid-tostring-results),
but introduced a
[deadlock](https://support.keenswh.com/spaceengineers/pc/topic/27997-servers-deadlocked-on-load)
as a result, so the fix to `MyDefinitionId.ToString` has been put back into this
plugin.

## Reducing memory allocations in the turret targeting system

There are large memory allocations in some frequently called routines, causing
quite a bit of GC pressure:

- `MyLargeTurretTargetingSystem.SortTargetRoots`
- `MyLargeTurretTargetingSystem.UpdateVisibilityCacheCounters` (this part was
  disabled due to reported crashes)

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/24145-excessive-memory-allocation-in-mylargeturrettargetingsystem)

## Caching the result of wind turbine atmosphere checks

Since the result of `MyWindTurbine.IsInAtmosphere` does not change often, it can
safely be cached for a few seconds.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/24209-performance-cache-the-result-of-mywindturbine-isinatmosphere)

## Less frequent sync of block counts for limit checking

Suppresses frequent calls to `MyPlayerCollection.SendDirtyBlockLimits`.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/24390-performance-myplayercollection-senddirtyblocklimits-is-called-too-frequently)

## Cache actions allowed by the safe zone

Caches the result of `MySafeZone.IsActionAllowed` and
`MySessionComponentSafeZones.IsActionAllowedForSafezone` for 2 seconds.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/24391-performance-safe-zone-isactionallowed)

## Less frequent update of PB access to blocks

Suppresses frequent calls to `MyGridTerminalSystem.UpdateGridBlocksOwnership`
updating `IsAccessibleForProgrammableBlock` unnecessarily often.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/24389-performance-frequent-update-of-pb-access-rights-to-blocks)

## Less frequent update of block access rights

Caches the result of `MyCubeBlock.GetUserRelationToOwner` and
`MyTerminalBlock.HasPlayerAccessReason`. Off by default on the server; opt in
deliberately.

## Fixed Havok thread count in MyPhysics

Keen introduced `MyVRage.Platform.System.OptimalHavokThreadCount`, but it is set to
`null`. The new logic in `MyPhysics.LoadData` falls back to the call it made before:
`HkJobThreadPool()`.

Inside Havok (C++ code) they apparently changed it to default to a single thread in
this case, so all the physics ends up running on a single thread (main thread).

## Optimized MyClusterTree.ReorderClusters

Replaced an O(N*M) algorithm with one of better time complexity. Improves the load
time of servers with many grids and potentially reduces lag as ships move around.

## Cached MyGridConveyorSystem.Reachable

Caches the result of `Reachable` calls, because they are very numerous for grids
with long conveyor networks (capital ships, production bases). There is a separate
cache per logical grid group.

Cache invalidation conditions:

- block added/removed to/from grid if the block has conveyor ports
- grid split/merge
- grid ownership change
- connector lock/unlock or config change
- grid added/removed to/from logical group

It eliminates most of the lag when players enter/leave cockpits or cryopods. It
also reduces the conveyor overhead while loading large production grids. It may
have a slight impact on simple grids with short conveyor systems due to the
additional overhead of building and using the cache, however this overhead should
be negligible.

## Rate limited excessive logging

Rate limits excessive logging from `MyDefinitionManager.GetBlueprintDefinition`.
For example it caused 11000 of the "No blueprint with Id" messages logged every
minute while players were running Isy's Inventory Manager PB script. In addition to
the extra CPU load it risked running out of disk space if left unchecked.

## Disabled functional blocks in projected grids

Projected functional blocks are updated, which is a waste of time. Also due to bugs
some of them can even function, for example projected welders can weld in creative
mode if they are enabled in the blueprint.

To fix this, functional blocks have to be disabled on grids with no physics. These
should only be the projected functional blocks. It happens only once when the
functional block is added to the scene in order to avoid a constant CPU overhead.

This fix may have side-effects should a plugin provide physics-less subgrids. In
such a case disable this fix and use the Multigrid Projector plugin to fix this
specific case only for the welders in a different way.

This fix has the visual side-effect of all functional blocks showing up as disabled
in the projection, so the players don't know in advance whether they will be enabled
once welded. The fix does not affect the welded state, only the visual feedback.
This applies only if the plugin is installed on the client side.
