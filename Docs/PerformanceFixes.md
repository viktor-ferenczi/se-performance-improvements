# Performance Fixes — Technical Details

This document describes each performance fix the plugin applies, what it does and
why. Every fix has a matching toggle in the plugin configuration (the in-game
dialog on the client, Quasar on the server). It is safe to change most of these
options during the game; the ones that say **needs restart** only take effect
after restarting the game or the server.

For the focused overview and installation instructions see the [README](../README.md).
For how the code implementing these fixes is organized — module by module, file by
file — see the [Developer Handbook](TOC.md).

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

## Process memory statistics read every frame

`MyGeneralStats.Update` runs once per frame and reads the process' private memory
size through `MyVRage.Platform.System.ProcessPrivateMemory`, which only feeds the
statistics log lines and the replication statistics. On Windows that is a single
`GetProcessMemoryInfo` call. On Linux the compatibility layer answers it with
`Process.PrivateMemorySize64`, which creates a `Process` object and parses
`/proc/<pid>/stat` and `status` on every call: with the other fixes in place it was
the largest remaining item on the main thread in three test worlds, about 0.5 ms
per frame, on the client and the dedicated server alike.

The fix refreshes the value at most once per second and serves it from a cache in
between. Nothing reads it more precisely than that: the statistics line is logged
once a minute and the replication statistics are sampled per second.

Measured in the "Conveyor Test Heavy" test world on a headless Linux client, main
thread frame time at idle went from 2.0 ms to 1.8 ms with all other fixes on.

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

### Linear target group refresh

A separate option from the allocation fix above. A grid with turrets refreshes its
list of target groups
once per frame in `MyGridTargeting.RefreshGridConnections`: every top-most entity in
the turrets' range, grouped by physical connection. The game pops entities off the
query result and, for every grid, removes each physically connected grid from that
list with `List.Remove`, a linear search, so N grids in range cost N x N comparisons
per turret grid per frame. On a busy server with hundreds of grids around a few
turret grids that is real main thread time.

The fix does the same grouping with a set for the "not yet grouped" test, walking
the query result in the same order and asking the game for the same groups, so the
result is identical; it was checked group by group against the game's own algorithm
over thousands of refreshes with a probe. Measured in the "Many Lifters Slowness"
test world (600 grids in range of the turret grids), the refresh went from 2.3% of
the main thread's time to under 0.1%.

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

## Separating grids stuck inside each other

A large ship rammed into a station inside a safe zone without damage can end up with
part of its hull inside the station: in the Octahedron world blocks of a Rhea got into
the lobby behind the glass pyramid and could not get back out through the windows. No
block breaks, so nothing lets the two grids come apart, and Havok solves the deep
penetration again on every physics step for as long as it lasts. Large collisions
elsewhere can do the same for a while.

The plugin hooks the grids' contact point callback
(`MyGridPhysics.RigidBody_ContactPointCallbackImpl`), which reports how deep each
contact is. Static grids get no callback, so every contact with a station comes from the
ship's side. A pair of grids with contact points deeper than 0.5 m on 60 updates, with no
gap longer than 30 updates, is stuck. The grids that may move are then teleported away
from the center of their contact points, towards their own center, 0.5 m per update
(30 m/s), until they have had no contact deeper than 5 cm for 5 updates. Then their
velocity is zeroed and the physics takes over again. Static grids, and grids attached to
a static grid by a connector or landing gear, never move. Of two ships the lighter one
moves more, by the inverse of their masses. Grids in the same physical group (rotors,
pistons, connectors) are left alone. The server moves the grids, the clients see it
through the normal position replication. It gives up after 300 m.

Option `SeparateStuckGrids`, on by default in both plugins. The client plugin does it only
where the game runs as the server: offline games and the multiplayer games the client
hosts. A client joined to a remote server leaves it to that server. The repro rig is in
`Tools/StuckGrids`: `rig.py` on a Magnetar server, `client_test.py` in an offline client.

Measured on a Linux Magnetar server (polaris, alone on the host) with the Octahedron
Alpha Station in its no-damage safe zone and an IMDC Rhea (40 grids) placed 55 m deep
into the top of the station's glass pyramid. 90 seconds from the placement, three runs
each, interleaved:

| | update time | simulation speed | deep contact |
| --- | --- | --- | --- |
| without the fix | 22.7 - 23.5 ms | 0.72 - 0.74 | all 90 s |
| with the fix | 4.5 - 4.8 ms | 1.00 | 5 - 6 s, then moved out by 36.5 - 37.5 m |

With nothing stuck the update time was the same with and without the fix (5.2 - 5.5 ms
with the Rhea and its subgrids touching each other).

## Skipping redundant updates of PB access to blocks

Before every run of a programmable block the game calls
`MyGridTerminalSystem.UpdateGridBlocksOwnership` with the block's owner, which sets
`IsAccessibleForProgrammableBlock` on every terminal block of the grid group from the
owner's access rights. With a script running every tick on a large grid that walk is
most of the main thread's simulation time: 42% on the 33000 block grid of the "Grid
Size Scalability Test" world.

The flags depend on the owner asked for, on the blocks of the terminal system, on
block ownership and share modes, on faction relations and on the admin settings. The
first three are tracked exactly: an entry per terminal system remembers the owner the
flags were last computed for and a generation counter bumped when a block joins or
leaves a terminal system (`MyGridTerminalSystem.Add` / `Remove`) or ownership changes
(`MyCubeGrid.NotifyBlockOwnershipChange` / `ChangeGridOwnership`). A call with the
same owner and the same generation is skipped. Faction relation and admin setting
changes are not hooked, so an entry is also dropped after two seconds, which is the
longest a programmable block can keep access it should have lost, or lack access it
should have gained, for those two reasons. Two programmable blocks with different
owners on one grid still get a fresh walk each, as before.

An earlier version of this fix inhibited the calls per owner for four seconds
regardless of what happened in between, which was wrong when programmable blocks
with different owners alternated, and was compiled out; this one was verified with a
probe recomputing the flags after every skipped call (no wrong flag over thousands
of runs). Measured on that world, headless Linux client: main thread frame time
3.5 ms to 1.9 ms at 1920 programmable block runs per second, the walk gone from the
profile, hit rate 99.97%.

[Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/24389-performance-frequent-update-of-pb-access-rights-to-blocks)

## Per-thread PB instruction counter and run time limit

`FixScriptCounter`, off by default, needs a restart. It is the groundwork for running
programmable block scripts off the main thread, and on its own it changes nothing a player
sees.

The game counts the work of a running script in `VRage.Library.Compiler.IlInjector`: the
calls it injects into every block of a script, and the allocation tracking of the
MemorySafe collections, update plain static fields (instructions, call depth, stack limit,
time baseline, memory, a dead flag and one shared counter handle). Two scripts running at the
same time on two threads would reset and corrupt each other's counters. With the option on,
a Cecil prepatch marks those fields `[ThreadStatic]` and gives each thread its own counter
handle. Script IL does not change, so compiled script caches stay valid, and on the main
thread the counter behaves exactly as before. The Preloader reads the option from the config
file itself, since the patch must be in place before the game loads; with the option off it
leaves `VRage.Library` alone.

`ScriptRunner.Execute` runs a script on the calling thread with the game's own checks and
limits, so a runaway script on a worker thread is stopped by its own counter. Off the main
thread it keeps the game's "PB invoked from parallel thread" report from firing, which throws
on a server where no mod script was ever compiled.

`ScriptRunLimitMs` (0.1 to 1000 ms in steps of 0.1, default 100) is the hard limit. It replaces the game's 3 s
wall clock limit per run while `FixScriptCounter` is on. A script over it is stopped in the
middle of the run like one over the instruction limit, with a message that names the limit,
and stays stopped until it is recompiled, as in the game.

`ScriptSoftLimitMs` (0.1 to 100 ms, default 5) is the soft limit. It is checked after each run,
against the time of the script's own code, without the game's work before it such as the
ownership walk. A script over it in 3 runs in a row is stopped the same way, with its own
message; a run under it starts the count over. Both limits are wall clock time, so a short
stall of the whole process (a garbage collection, another busy program) counts against every
run in it. In a test with a 5 ms hard limit, 30 scripts that normally run for 1.4 ms were all
stopped by one stall of about 100 ms. The soft limit lets a script survive such a stall and
still stops one that is slow every time.

## PB time budget per frame

`FixScriptScheduler`, off by default, needs `FixScriptCounter`. The game runs every script
whose update frequency is due, in the after simulation update of its block, with no limit on
their total time. With the option on, those runs go to a queue instead, and once every entity
has had its update the queue runs on the main thread until the frame's PB time reaches
`ScriptFrameBudgetMs` (0.1 to 100 ms, default 5). The runs left over wait for the next frame. A
PB due again while it waits still runs once, with the update types of all its due runs, the
way the game merges them too.

The queue runs the PBs passed over the most times first, in random order among equal waits,
so with N PBs due and room for K runs a frame, no PB waits much longer than N/K frames. A run
starts only if the time used so far plus the longest of that PB's last 10 run times fits the
budget. A PB with no run times yet reserves nothing, and the first run of a frame always
starts, so the queue moves even when other work used the budget up. A run can therefore end
past the budget, by at most one run limit. Runs that come from timers, buttons, IGC
callbacks, other scripts (`TryRun`), mods and players are not delayed, since `TryRun` returns
the result of the run, but their time counts against the budget.

The run limit is not lowered when the budget can't keep up: a script stopped by the limit
loses its state and stays stopped until a player recompiles it. Scripts run less often
instead. When a PB waited more than 10 frames in a row, a warning in the log says, once a
minute, how many frames were over, how many runs were due and ran per frame, the longest
wait, and the PB time per frame.

`ScriptThreadCountMode` (Auto or Manual) and `ScriptThreadCount` set how many threads run
PB scripts, the main thread included; Auto takes the automatic physics thread count. Only
scripts with proxies (`FixScriptProxies`, below) leave the main thread. With the proxies on
and more than one thread, the queue first runs the other scripts on the main thread, then
the scripts with proxies in a parallel phase on all the threads, each thread with a budget
of its own. The main thread does nothing else meanwhile. Two PBs of one logical grid group
never run at the same time: a run writes the PB access flags of its whole group, and the
policy of the members a script calls assumes that nothing else changes the group during
the phase. The writes the proxies held back are made on the main thread after the phase,
PB by PB in EntityId order, so the result does not depend on which thread ran what. Then
each PB gets its DetailedInfo, as after a run on the main thread. A held back write that
fails is counted in the once a minute warning. The warning also says how many threads ran
and which share of the runs was parallel.

## Proxies for PB scripts

`FixScriptProxies`, off by default, prepares scripts for running off the main thread. A script
reaches the game only through the roots it is given: `GridTerminalSystem`, `Me`, `Runtime` and
`IGC`. With the option on, a script that may run off the main thread gets proxy objects behind
those roots instead of the game objects. Everything the script gets from a proxy is a proxy
too, so every game call it makes goes through one.

Whether a script may run off the main thread is decided once per script instance, when the
PB creates it, by scanning the IL of the script assembly. Every PB loads its own copy of that
assembly, so the result is kept in memory by a hash of the script's source: the scan runs once
per distinct script while the game runs, not once per PB or recompile. Each game member it calls is looked
up in `ScriptPolicyTable.cs`, which says for every member of the game's script whitelist (about
900, apart from plain data types) what calling it on a worker thread would do:

- **pass**: it only reads, or writes only caches of objects of the script's own grid group,
  like a block's world matrix, which no other thread touches while the script runs. The
  proxy calls the game.
- **queue**: a write without a result, like a setter or `WriteText`. Generic methods can't
  be queued. Off the main thread the proxy records the call, with copies of its text and
  list arguments, and returns; the call is made on the main thread later. A script that sets
  a property and reads it back in the same run gets the value it set. Other effects of its
  writes show up a frame later. `WriteText` returns true while queued: the game returns false
  only while a player edits the text.
- **main**: anything else, for example creating store offers or starting a warhead
  countdown.

A policy can name the mechanism it relies on, like `pass@Terminal`, and there is a fourth
policy, **standin**, where the proxy calls a replacement written for worker threads; see the
next section.

A script that calls any main member, or a game member missing from the table, stays on the
main thread and keeps the game objects. So does a script whose IL can't be read. Every member
was classified from the decompiled game code. A static member is pass only if it uses its
block argument through the block's interfaces, since a proxy fails a type test or cast to a
game class. A unit test checks the table against a snapshot of the whitelist,
`Tests/Shared/Patches/ScriptWhitelist.txt`, so after a game update a refreshed snapshot shows
the new members (`Tools/PbProfile/whitelist.py` says how).

On the main thread the proxies only forward calls, so with the option on and the scripts
still on the main thread they behave as before, at the cost of one extra call per game call.
The option applies to scripts created after it changes: after a recompile or a world load.

With the option on, `MyCubeBlockDefinition.DisplayNameText` takes a lock on the definition,
because it builds the name in a buffer shared by all blocks of that type, and scripts read it
through `CustomName` and `DefinitionDisplayNameText`.

## Mechanisms that let more PB scripts run on a worker thread

About 60 members of the script whitelist are not safe to call on a worker thread as they are,
mostly because the game keeps scratch state for them in shared fields. Ten mechanisms make
them safe, each with its own option. A script runs off the main thread only if every
mechanism its calls rely on is on; otherwise it runs on the main thread, with its proxies.
Turning an option off moves the scripts needing it back to the main thread from the next
frame. The options take effect only with the per-thread counter, the frame budget, the proxies
and more than one PB thread. In the client settings and in Quasar they sit, with the
decision log below, in the "PB on worker thread" group after the programmable block options.
The two that are off by default carry a higher risk, and their descriptions say why.

The patches behind them are applied at startup whenever the per-thread counter is on, whatever
the options say, and act only during the parallel phase, so outside it the game runs as
before. Applied later, a patched game method could still run unpatched where a caller compiled
earlier inlined it.

| Option | Default | Members | How |
| --- | --- | --- | --- |
| `ScriptOffMainSprites` | on | `DrawFrame`, `MySpriteDrawFrame.Dispose` | Nothing to do: submitting a frame only writes fields of that surface, which the main thread sends and renders later. |
| `ScriptOffMainAccess` | on | `CanAccess` | Reads. For a grid, the stand-in adds the owner fallback the game uses only on the main thread. |
| `ScriptOffMainTerminal` | on | `GetProperty`, `GetActionWithName`, `GetValue`, `SetValue`, `ApplyAction` and the rest of the terminal property and action API | The lookup scratch lists are per thread, and the control lists of a block type are made under a lock. Combobox limits are serialized. Values set and actions applied stay queued. `WriteValue` takes the formatter lock. |
| `ScriptOffMainDetailedInfo` | on | `DetailedInfo`, the farm plot's detailed info | A dirty text is refreshed under one lock, because the game's number formatting shares one `NumberFormatInfo`. Pistons, flight movement blocks, path recorders, jump drives and projectors write shared state while refreshing, so off the main thread they return their last text and are refreshed on the main thread after the phase. |
| `ScriptOffMainIgc` | on | `RegisterBroadcastListener`, `AcceptMessage`, `SendBroadcastMessage`, `SendUnicastMessage`, `IsEndpointReachable` | The IGC session component's collections are locked in the phase. The antenna walk behind reachability is serialized; its result is cached per PB and frame. |
| `ScriptOffMainGravity` | on | Artificial and total gravity of ship controllers and parachutes | Before the phase the main thread brings the world matrix and box of every gravity generator up to date. With a gravity provider from a mod in the world, these scripts stay on the main thread. |
| `ScriptOffMainSensing` | off | Turret, turret controller and sensor targets, camera `Raycast`, `GetNearestPlayer`, `IsBeingHacked`, `CanControl` | Reads of entities the main thread updated, ray casts under the Havok world's own lock as the game's parallel ray casts do, and locks around the antenna walk and the planet voxel test. Off by default because it puts physics queries on worker threads. |
| `ScriptOffMainTransfers` | off | `TransferItemTo`, `TransferItemFrom`, `CanTransferItemTo`, `IsConnectedTo` | Reachability is checked on the worker, as the game does in its own background tasks, so the script gets the game's result. The move is made on the main thread after the phase, by item id. Off by default because the script sees the old inventory contents until its next run: a loop moving items until an inventory is empty would not end. |
| `ScriptOffMainTryRun` | on | `TryRun` | The other PB runs right away on the same thread if it is in the same grid group and its script may run off the main thread too. Otherwise `TryRun` returns false, as it does for a busy PB. |
| `ScriptOffMainInventory` | on | `HasInventory`, `GetInventory`, `GetInventoryCount`, `Get/SetUseConveyorSystem`, `NextItemId` | While the proxies are on, these static helpers unwrap the proxy they get. `SetUseConveyorSystem` is made on the main thread after the phase. |

Store offers and orders, warhead countdowns, path recorder beacons and autopilot waypoint
matrices stay on the main thread.

`ScriptLogThreadDecisions`, off by default, logs for each PB where its script runs and why:
the members that keep it on the main thread, or the options it needs that are off. It logs a
PB when it is first seen and when the decision changes, so it can stay on on a live server.

Over a corpus of 219 compiling scripts (76 popular Workshop scripts and 143 from popular
blueprints), 29% could leave the main thread without these mechanisms, 75% can with the
defaults and 99% with all of them on.

With the per-thread counter on, the `AppendDecimal` and `AppendFormatedDecimal` helpers the
game gives scripts get a `NumberFormatInfo` per thread. They set the decimal digits in one
shared instance right before formatting with it, so two scripts formatting at the same time on
two threads could get each other's digits.

## Less frequent update of block access rights

Caches the result of `MyCubeBlock.GetUserRelationToOwner` and
`MyTerminalBlock.HasPlayerAccessReason`. Off by default on the server; opt in
deliberately.

## Configurable Havok physics thread count

**Needs restart.** `MyPhysics.LoadData` sizes the Havok job thread pool from
`MyVRage.Platform.System.OptimalHavokThreadCount`, and the game's implementation of
that property is a hard `null`, so the parameterless `HkJobThreadPool()` runs and
Havok sizes the pool from the machine on its own terms — measured on a 16 logical
processor host, that is 7 worker threads. The property is the game's own extension
point for this answer and `MyPhysics.LoadData` is the only place it is read, so a
postfix on it is the whole fix; the job queue follows, since its size is derived from
the pool's.

Two options control it:

- **Auto** (the default) uses one worker per physical CPU core, minus one, at most 6
  (see below), and keeps the thread count option updated with the number this machine
  gets, so the configuration always shows the count the game will actually be given.
- **Game** leaves the sizing to the game: the property keeps answering what it
  answers without the plugin. This is the way back to the stock behaviour for an
  admin who would rather tune the pool by measuring than take the default.
- **Manual** uses exactly the configured number, between 2 and 64.

The minimum is 2 rather than 1, because a pool of one is *not* single threaded
physics. The Havok worlds are still created and initialized for multithreading
(`HkWorld.InitMultithreading`) and still stepped through the multithreaded path
(`StepSimulation` with `multithreaded: true`, or `InitMtStep` / `ProcessAllJobs` /
`FinishMtStep` on the job queue), so a single worker pays all of the job dispatch and
waiting with nothing to overlap it with. Physics without threading is a different
switch entirely — `MyFakes.ENABLE_HAVOK_MULTITHREADING`, which the game exposes to
admins through `MyPhysics.SetScheduling` and which also decides how the worlds
themselves are created. Havok's own way to run the multithreaded code path on the
calling thread alone would be a pool of zero, but `HkJobThreadPool` hands the count
straight to the native library, so nothing on the managed side can establish what the
shipped build does with 0 or 1 without guessing.

Turning the physics fix off leaves the game's own sizing alone.

The upper end is capped by Havok itself, not by the plugin. Measured on a 16 logical
processor host, with a Star System world loaded and the worker threads counted by name
(`HkThread_1..N`):

| Setting | Havok worker threads |
| --- | --- |
| Physics fix off (the game's own sizing) | 7 |
| Auto (asked for 16) | 11 |
| Manual 24 | 11 |
| Manual 3 | 3 |

So the shipped library has an internal maximum of its own — 11 workers plus the calling
thread — and a number above that is neither an error nor an improvement, it simply stops
making a difference. The resolved count is logged at the DEBUG log level when the world
loads, which is the way to see what a setting actually asked for.

### One worker per physical core

Auto asks for one worker per *physical* core, minus one so the main thread keeps a
core of its own, capped at 6 — on an 8 core / 16 thread host, 6 workers. Physical rather than
logical on both platforms: the pool's workers spin between jobs, and a spinning worker
shares the execution units of its hyper-threading sibling, so counting siblings as
cores inflates the pool without adding throughput.

The physical count comes from the operating system. On Linux it is the number of
distinct hyper-threading sibling groups in
`/sys/devices/system/cpu/cpu*/topology/thread_siblings_list`, with the "physical id"
plus "core id" pairs in `/proc/cpuinfo` as a fallback for kernels and containers which
do not expose the topology in sysfs. On Windows it is the number of
`RelationProcessorCore` records returned by `GetLogicalProcessorInformationEx` — the
`Ex` function rather than `GetLogicalProcessorInformation`, because the latter only
describes the first processor group and so stops at 64 logical processors. When
neither can be read the logical count stands in for it.

Auto never asks for more than 6 workers, on the client and on the server. Several
servers often run on one host, as cluster nodes or as separate Magnetar instances
started by Quasar or any other automation, and each pool's workers spin between jobs,
so pools sized to the whole machine fight over the same cores. The cap limits that
contention without per-host tuning. A physics-heavy world
that has a host to itself can still ask for more with **Manual**.

The Linux side is where an oversized pool was measured to hurt, because there the game
runs the Windows build of Havok through the native wrappers and the workers wait on
emulated Win32 events and semaphores (ntsync).

How far down the count should go is load dependent, and a heavily jointed scene wants
fewer workers than this. Measured in the "Many Lifters Slowness" test world, a lattice of 602 small grids resting on each other with
1206 landing gears and 116 turrets, headless Linux client on an 8 core / 16 thread
host, simulation speed at idle with the lattice awake:

| Havok workers | Sim speed | Frame time |
| --- | --- | --- |
| game's own sizing (7) | 0.57 | 30 ms |
| 11 (asking for 16) | 0.55 | 30 ms |
| 8 | 0.55 | 30 ms |
| 6 | 0.60 | 28 ms |
| 4 | 0.62 | 27 ms |
| 3 | 0.65 | 26 ms |
| 2 | 0.72 | 23 ms |

The other direction was checked too: 300 independent 21 block grids pasted at once
and falling onto a planet, where a big pool could in principle solve the islands in
parallel, ran at the same simulation speed with 2 workers as with 11 (median frame 2.6
against 4.6 ms, 90th percentile 14 against 16 ms).

So on this particular world a smaller pool than Auto's is better still, and a server
whose load looks like that lattice should set **Manual** and measure. Auto does not
count sibling threads as cores and reserves a core for the main thread, but it is not
necessarily optimal for every scene. Manual asks for exactly what it says, and Game
leaves the sizing to Havok.
Whether the Windows build of the game shows the same trend was not measured, so the
Windows default is unchanged.

Until this was reworked the same thing was done by a transpiler on `MyPhysics.LoadData`
which wrote an `int` into a `Nullable<int>` local — invalid IL, which is why it had to be
disabled on .NET Core, where it corrupted memory during world load. So on any current
(.NET) client or server the thread count fix did nothing at all until now.

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

### Cached master assembler lookup

Part of the same option. An assembler in cooperative mode looks for a master on
every production tick with `MyAssembler.GetMasterAssembler`: a breadth-first walk
over the whole conveyor network under the global pathfinding lock, filtered to the
reachable assemblers with a friendly owner, shuffled, then the first one with a
non-empty queue wins. The parallel item transfer computations take the same lock,
so on a production base the main thread mostly waits in this method. In the
"Conveyor Test Heavy" test world (855 cooperative assemblers, about 5000 conveyor
blocks on one merged grid) it was 40% of the main thread's simulation time at
idle.

The walk depends only on the conveyor network and on block ownership, so its
result, the list of reachable assemblers, is cached per assembler for five
seconds. The random choice and the checks on the candidates (not disassembling,
not a slave, non-empty queue) still run on every call. The entries are discarded
at once by every event which invalidates the reachability cache above and also
when a conveyor network is flagged for recomputation, which covers conveyor lines
losing or regaining power and sorter changes. Faction relation changes are only
picked up when an entry expires, up to five seconds later.

Measured on that test world with the plugin's other fixes on, main thread frame
time at idle went from about 2.9 ms to 2.4 ms, and the lock contention seen in the
profiler dropped by two thirds. Cache hit rate 80 to 90%.

## Toolbar block group actions

While the player sits in a cockpit, the game refreshes every item of its toolbar
on every frame. An item which is a block group (`MyToolbarItemTerminalGroup`)
rebuilds the group's block list and collects the terminal actions valid for the
group, which walks every block of the group and every component of every block.
For a group of hundreds of blocks (all refineries of a base, all lifters of a
carrier) that is milliseconds per frame, paid only while the seat is occupied,
which is exactly when the player notices. The result changes only when a block is
added to or removed from the group.

The fix caches the collected actions per toolbar item for one second, keyed by a
fingerprint of the group's block list, so a change to the group is picked up on
the next frame regardless of the cache age. The enabled state, icons and value
text of the item still refresh on every frame from the live blocks. An action
which appears or disappears without the membership changing (a rare kind of
action, enabled by block state) can lag by up to a second. Client only, since a
dedicated server never updates toolbars.

Measured in the "Conveyor Test Heavy" test world in a flight seat whose toolbar
holds four groups of several hundred blocks: main thread frame time while seated
went from about 5 ms to 2.4 ms, the same as standing next to the seat. Cache hit
rate 98%.

## Eliminated excessive logging

`MyDefinitionManager.GetBlueprintDefinition` looked its dictionary up twice, and the
miss branch logged a "No blueprint with Id" message. For example it caused 11000 of
those messages logged every minute while players were running Isy's Inventory Manager
PB script. In addition to the extra CPU load it risked running out of disk space if
left unchecked. The transpiler rewrites the body to a single `GetValueOrDefault` call,
which removes both the second lookup and the logging. It runs with the memory
allocation fixes (`FixMemory`); there is no separate option for it.

## Disabled functional blocks in projected grids

Projected functional blocks are updated, which is a waste of time. Also due to bugs
some of them can even function, for example projected welders can weld in creative
mode if they are enabled in the blueprint.

To fix this, functional blocks are disabled on preview grids with no physics, which
are the projected grids. It happens only once when the functional block is added to
the scene in order to avoid a constant CPU overhead. Hidden physics-less grids
spawned by mods, like the camera grid of Scope Framework, are not preview grids, so
their blocks are left alone.

This fix has the visual side-effect of all functional blocks showing up as disabled
in the projection, so the players don't know in advance whether they will be enabled
once welded. The fix does not affect the welded state, only the visual feedback.
This applies only if the plugin is installed on the client side.

## Faster image loading with a newer ImageSharp

The game decodes PNG images with SixLabors.ImageSharp 1.0.0-beta0006 from 2019.
Every planet height map and material map (six 2048x2048 faces per planet, 16 bit
gray and RGBA), the terrain blend textures and every non-DDS mod texture go through
it, all via `MyImage.Load` in `VRage.Render`. The plugin ships ImageSharp 2.1.13
next to itself and decodes those images with it instead.

The newer library cannot simply be referenced: the game's copy is loaded early by
dotnet-compat and linux-compat, and on .NET (Core) a process holds one assembly per
simple name. The plugin therefore rewrites its copy with Cecil to a different
assembly name (`SixLabors.ImageSharp.Performance`, cached under the plugin's cache
folder per library version) and drives it through a small reflection layer, so the
plugin's own code never references either ImageSharp at compile time. A prefix on
the one `MyImage.Load` overload that the other two funnel into does the decoding;
if the library is missing or a decode throws, the game's own decoder runs.

The decoded pixels are byte-identical to the game's decoder: the pixel format
selection mirrors `MyImage.Load`, and every planet map shipped with the game was
checked by hashing the decoded arrays from the game's decoder, from this fix and
from an independent PNG reader. Height maps feed voxel generation, so anything less
would change terrain and desync a patched client from an unpatched server.

Measured on the Earth planet test world (79 planet map files, decoded on several
threads during world load), the planet map phase went from about 1.2 s to about
0.95 s and the per-file decode time dropped by roughly a third. The gain is modest
because the game's decoder, once dotnet-compat has fixed its stream handling for
.NET, is already reasonable at this; the fix mostly removes the remaining decoder
overhead and brings a maintained PNG decoder into the game.


## Asteroid voxel preloading on game start

**Needs restart.** `MySandboxGame.PerformPreloading` runs while the game starts, before
any world is loaded. Besides the vanilla sounds it walks every `VoxelMapStorage`
definition which can take part in procedural asteroid generation and calls
`MyStorageBase.LoadFromFile` on each, purely to warm the LRU cache inside
`MyStorageBase`. That happens whether or not the session will ever contain an
asteroid, and it is paid by the dedicated server as well as by the client.

The fix skips those loads. It is a lazy versus eager change and nothing else:
`LoadFromFile` *is* the cache lookup (a 512 entry LRU cache keyed by the file path
plus the material modifiers), so a caller which needs one of these storages later
loads it then and gets the same shared object the preload would have put there. The
only cost is that the load happens on first use instead of at startup, and the game
already has a weaker form of the same idea — the preload calls `ResetDataCache()` on
what it has just loaded when the platform reports `IsMemoryLimited`.

Two patches implement it. A prefix and a finalizer on `PerformPreloading` mark the
window in which the preload runs, and a prefix on `LoadFromFile` drops the loads
asked for inside that window, returning `null` — a value the preload already handles,
since `LoadFromFile` returns `null` for a definition whose file is missing. Only the
preload's own call shape is dropped (`cache: true, logInfo: false`, which no other
caller in the game uses), so a voxel load which happens to run on another thread
while the window is open is left alone. Everything a session actually needs still
loads normally.

Both patches must be in place before the game starts preloading, which happens
before `IPlugin.Init` on the client, so they are applied from the plugin's
`Preloader` through a hook on `MyInitializer.InvokeBeforeRun` — the same early
bootstrap the dedicated server already used. The number of skipped loads is logged
at the DEBUG log level when the window closes.

Measured on a headless Linux client (isolated Pulsar instance, this plugin and the
Remote API loaded), 99 storages skipped, RSS 30 seconds after reaching the main menu
and 60 seconds after a Star System world with procedural asteroids became active:

| | fix off | fix on |
| --- | --- | --- |
| Main menu | 3046 MiB | 2020 MiB |
| In the world | 6545 MiB | 5517 MiB |

About a gigabyte either way — the game never reclaims the preloaded storages, so the
saving survives into the session. Both runs loaded the world with no exceptions at
simulation speed 1.0, and both logged exactly one vanilla asteroid shape loaded from
`Content/VoxelMaps` while playing — with the fix on, that is the lazy path doing its
job.

The fix is ported from the `--bare-bones` mode of the Remote API plugin, where it was
first measured.


## XML deserialization

The game reads worlds, blueprints, the world checkpoint and all definition files
(including those of mods) with the pregenerated XML serializers in the
`*.XmlSerializers` assemblies. Polymorphic elements, which means every cube block,
inventory, entity component and definition, are read one by one: `MyXmlSerializerBase`
calls `XmlSerializer.Deserialize` for each of them. Every such call creates a new
generated reader, and the runtime's `XmlSerializationReader.Init` calls the reader's
`InitIDs`, which adds every element and attribute name the whole assembly knows to the
XML reader's name table. That is 4941 names for VRage.Game and 2065 for
SpaceEngineers.ObjectBuilders, repeated for every block.

`InitIDs` only stores the strings `NameTable.Add` returns, and for the same name table
those are always the same atomized strings. All nested readers of one document share the
document's name table, so the fix runs `InitIDs` once per name table and generated reader
class, captures the result, and copies it into every later reader instead. The patch is a
transpiler on the runtime's `XmlSerializationReader.Init`, which reroutes its `InitIDs`
call; patching the generated `InitIDs` methods themselves would work too, but Harmony
takes about two seconds of every game start to rewrite their huge bodies. The captured
values are held in a `ConditionalWeakTable` keyed by the name table, so they go away with
the document.

The long `if (type == typeof(...)) else if ...` chains in the same generated code are not
a problem: they only handle values declared as `object`, which world files do not use.
They never ran while loading or saving the test worlds below.

Measured on a headless Linux client, same build with the fix off and on. Worlds loaded
from XML only (without their `.sbsB5`, which is how the game loads a world the first time
if it has none), the game's own `Loading duration`, first and second load in a session:

| World | fix off | fix on |
| --- | --- | --- |
| Conveyor Test Heavy (26 MB, 20810 blocks) | 12.2 s / 9.3 s | 5.9 s / 3.0 s |
| Many Lifters Slowness (52 MB, 699 grids) | 12.0 s / 11.6 s | 3.8 s / 3.4 s |

The CPU time of the XML parsing itself went from 11.3 s to 0.9 s and from 11.5 s to 0.7 s.
Normal loads, where the sector comes from the `.sbsB5` and only the checkpoint and the
definitions are XML, gain less: the median of four loads went from 5.6 s to 5.2 s and from
4.3 s to 4.1 s. The `.sbsB5` the game writes straight from the parsed XML sector was
byte-identical with the fix off and on for both worlds.
