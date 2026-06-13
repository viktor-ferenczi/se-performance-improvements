# Performance Improvements — Developer Handbook

This is the developer handbook for the **Performance Improvements for Space
Engineers** plugin: a guided, top-down tour of how the code is organised and what
each part does. It complements the two reader-facing documents:

- **[README](../README.md)** — what the plugin is, its features and how to install it.
- **[Performance Fixes — Technical Details](PerformanceFixes.md)** — the *rationale*
  for every individual fix (the symptom, the mechanism and the Keen support ticket).
  Each patch documented here links back to its section there.

For a flat, searchable list of every documented file, see the **[Index](Index.md)**.

## What this plugin is

The plugin is a **paired client and server plugin** that uses
[Harmony](https://github.com/pardeike/Harmony) to patch the running Space Engineers
game and dedicated server, making them run faster. Every fix is an independent,
**individually toggleable** patch — you tune the plugin to your world and hardware.

The two plugins ([`ClientPlugin/Plugin.cs`](files/ClientPlugin/Plugin.cs.md) and
[`ServerPlugin/Plugin.cs`](files/ServerPlugin/Plugin.cs.md)) are thin entry points.
Almost all behaviour lives in **`Shared/`**, which both reference, so a single patch
codebase runs on both sides.

## Architecture at a glance

```
ClientPlugin/         Pulsar IPlugin entry point + in-game settings dialog
ServerPlugin/         Magnetar/Quasar entry point + XML server config (+ server-only patches)
Shared/
  Plugin/             Common bootstrap shared by both plugins
  Config/             IPluginConfig contract (each side has its own config object)
  Logging/            Environment-agnostic logger
  Tools/              Caches, RW-locks, IL/transpiler helpers, publicizer support
  Patches/            The performance patches, one folder per game subsystem
Tests/                Unit tests for the shared tooling
```

The startup path is the same on both sides: the entry-point `Plugin` constructs a
config object and calls the shared [`Common`](files/Shared/Plugin/Common.cs.md)
bootstrap, which uses [`PatchHelpers`](files/Shared/Patches/PatchHelpers.cs.md) to
discover every patch class, **verify** the targeted game IL still matches expectations
(via [`EnsureCode`](files/Shared/Tools/EnsureCode.cs.md)), and apply the patches with
Harmony. Each patch reads its on/off flag from the config object before doing work,
and shares infrastructure from [`Shared/Tools`](modules/tools.md) (caches, locks,
transpiler helpers).

## Modules

### Entry points

| Module | What it covers |
| --- | --- |
| [Client Plugin Entry Point](modules/client-plugin.md) | The Pulsar `IPlugin` that boots the plugin in the game client and owns the client config (the single source of truth for the toggles). |
| [Client Settings UI Framework](modules/client-settings.md) | A declarative framework that turns the client config's attributes into the in-game settings dialog (typed elements, layouts, binding, persistence). |
| [Server Plugin Entry Point](modules/server-plugin.md) | The dedicated-server entry point and its XML-serialized performance configuration. |

### Shared infrastructure

| Module | What it covers |
| --- | --- |
| [Shared Plugin Core](modules/shared-plugin-core.md) | The `Common` bootstrap and the `ICommonPlugin` / `IPluginConfig` contracts shared by both plugins. |
| [Logging](modules/logging.md) | An environment-agnostic logger so the same patch code logs on client and server alike. |
| [Shared Tools & Data Structures](modules/tools.md) | The toolbox the patches build on: time-bounded caches, RW-locked collections, Harmony IL/transpiler helpers, IL verification, hashing, object pools, publicizer support and Wine detection. |
| [Patch Infrastructure](modules/patch-infrastructure.md) | `PatchHelpers`: discovers, verifies and applies every Harmony patch in a controlled order. The engine every patch module depends on. |

### Performance patches

Each module below groups patches by the game subsystem they touch. The "why" for
each lives in [PerformanceFixes.md](PerformanceFixes.md).

| Module | What it optimizes |
| --- | --- |
| [Grid Merge & Paste Patches](modules/merge-and-paste.md) | Suppresses redundant conveyor/world updates during grid merge, paste and group changes (~60–70% faster on heavy grids). |
| [Conveyor System Patches](modules/conveyor.md) | Caches conveyor-network reachability and pathfinding for large conveyor systems; avoids redundant `IsWorking` recomputation. |
| [Physics Patches](modules/physics.md) | Fixed Havok thread count, faster `RigidBody` getter, better-complexity cluster reordering. |
| [Safe Zone Patches](modules/safe-zone.md) | Caches `IsSafe` / `IsActionAllowed`; allocation-free `IsOutside`. |
| [Keen Overhead Removal](modules/keen-overhead-removal.md) | Removes constant background overhead: `GC.Collect` pauses, EOS P2P `UpdateStats` core load, Mod API call-statistics rewriter. |
| [Memory Allocation Patches](modules/memory-allocation.md) | Cuts GC pressure: cached `MyDefinitionId.ToString`, less frequent block-limit sync, allocation-free voxel material lookups. |
| [World Loading Patches](modules/world-loading.md) | Caches compiled mods and in-game scripts on disk; rate-limits blueprint log flooding. |
| [Simulation & Block Patches](modules/simulation-and-blocks.md) | Assorted per-block/per-subsystem fixes: turret targeting, access-right caching, wind-turbine atmosphere caching, projected-block disabling, server-side footprint/wheel-trail removal. |

### Testing

| Module | What it covers |
| --- | --- |
| [Tests](modules/tests.md) | Unit tests for the shared tooling. |

## How to navigate

The handbook uses **progressive disclosure**:

1. Start here (the TOC) to find the right **module**.
2. A module page introduces the subsystem, lists its files with one-line summaries,
   and explains how they fit together.
3. Each **file page** gives the purpose, key members, patch targets (for patches) and
   cross-references to related files.

To find a specific file directly, use the [Index](Index.md).

## How this handbook is maintained

The handbook is generated by the `structured-documentation` skill. All working data
(the file manifest with SHA256 hashes, the module map, the link map and the generator
scripts) lives under [`Docs/data/`](data/) and can be regenerated cheaply:

```bash
python3 Docs/data/scripts/build_manifest.py       # re-hash sources, assign modules/tiers
python3 Docs/data/scripts/generate_scaffolds.py   # create scaffolds for new/changed files
python3 Docs/data/scripts/resolve_wikilinks.py    # resolve wiki-links to relative links
python3 Docs/data/scripts/build_index.py          # rebuild Index.md
python3 Docs/data/scripts/check_links.py          # verify every relative link resolves
```

The `data/` subdirectory contains only working files — it can be deleted or
git-ignored without affecting the handbook itself.
