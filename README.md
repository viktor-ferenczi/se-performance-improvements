# Performance Improvements for Space Engineers

Various performance improvements to let the game and the server run smoother.
Each fix can be toggled individually, so you can tune the plugin to your world and hardware.

For support please join the [Pulsar Discord](https://discord.gg/z8ZczP2YZY).

Please consider supporting my work on [Patreon](https://www.patreon.com/semods) or one time via [PayPal](https://www.paypal.com/paypalme/vferenczi/).

*Thank you and enjoy!*

## Features

- Optimized grid operations (less redundant calculations)
- Optimized world loading (compilation cache)
- Improved algorithms (simulation, data structures)
- Less memory allocation (lower GC pressure)

<details>
<summary>Details</summary>

See [Docs/PerformanceFixes.md](Docs/PerformanceFixes.md) for the detailed technical description of
each fix and the related Keen support tickets, and the [Developer Handbook](Docs/TOC.md) for how
the code that implements them is organized.

**Grid operations**
- Faster grid merge and paste (skips redundant conveyor/world updates)
- Less lag on connector lock/unlock and rotor attach/detach (optimized grid group changes)

**World loading**
- Cache compiled mods and in-game (PB) scripts
- Decode planet maps and PNG textures with a newer ImageSharp
- Skip the preloading of the vanilla asteroid voxels on game start
- Parse worlds, blueprints and definitions from XML several times faster
- Eliminate long pauses from explicit `GC.Collect` calls
- Disable Mod API call statistics overhead

**Simulation & CPU**
- Read the process memory statistic once per second instead of every frame
- Cache safe zone checks (`IsSafe`, `IsActionAllowed`, optimized `IsOutside`)
- Cache wind turbine atmosphere checks
- Cache conveyor network reachability lookups and the cooperative assembler master lookup
- Cache the toolbar actions of large block groups while seated
- Physics optimizations (`RigidBody` getter, configurable Havok thread count, faster cluster reordering)
- Reduce frequent memory allocations (`MyDefinitionId.ToString`, turret targeting, voxel material lookups)
- Linear instead of quadratic refresh of turret target groups
- Less frequent sync of block counts and block access rights; PB access to blocks refreshed only when something changed
- Optional per-thread PB instruction counter (groundwork for running scripts off the main thread) and configurable hard and soft PB run time limits
- Optional PB time budget per frame: scheduled PB runs over it wait for later frames, the longest waiting first
- Disable server-side character footprints and wheel trail tracking
- Push apart grids stuck inside each other after a collision, like a ship rammed into a station in a no-damage safe zone (server, offline games and hosted multiplayer games)
- Push grids stuck inside a planet or an asteroid out of it (server, offline games and hosted multiplayer games)
- Disable functional blocks in projected grids
- Eliminate the blueprint-not-found log flooding from `GetBlueprintDefinition`

</details>

## Documentation

| Document | What it covers |
| --- | --- |
| [Developer Handbook (TOC)](Docs/TOC.md) | Top-down tour of the codebase: architecture, modules and how the pieces fit together. **Start here** to understand or modify the plugin. |
| [Performance Fixes — Technical Details](Docs/PerformanceFixes.md) | The rationale for every individual fix: the symptom, the mechanism and the Keen support ticket. |
| [File Index](Docs/Index.md) | A flat, searchable list of every source file with a one-line summary and a link to its detailed page. |
| [Development](Docs/Development.md) | Building from source, the plugin version and how to bump it, local paths, deployment. |


## Installation

### Client

Have [Pulsar](https://github.com/SpaceGT/Pulsar) installed. Link to the Installer is in the README there.

1. Enable the **Performance** plugin from the **Plugins** dialog.
2. Apply and restart the game.

### Server

<details>
<summary>Quasar</summary>

Have [Quasar](https://github.com/CometWorks/quasar) installed and a server created.

1. Enable the **Performance** plugin in your config profile(s).
2. Restart the server, so it picks up the plugin

The configuration is on Quasar's Web UI.

The server leaves off the conveyor lookup cache, the two fixes that change what players
see (LCD surface visibility, functional blocks in projected grids), and enables the rest; the client enables everything.
</details>

<details>
<summary>Magnetar</summary>

For standalone [Magnetar](https://github.com/CometWorks/magnetar) 
you need to reference the Performance plugin from the `Current` profile.

Edit the profile:
- Linux: `~/.config/Magnetar/Profiles/Current.xml`
- Windows: `%AppData%\Magnetar\Legacy\Profiles\Current.xml`

Directly inside the `<GitHub>` element insert:
```xml
    <GitHubPluginConfig>
      <Id>454B0F55-C727-4DAB-B1A9-5AE6050A0932</Id>
    </GitHubPluginConfig>
```

Configuration file is `Performance.cfg`, created in the `SpaceEngineersDedicated` folder.
**FIXME:** Include a default config here, because only the ones with non-default values are saved.

The server leaves off the conveyor lookup cache, the two fixes that change what players
see (LCD surface visibility, functional blocks in projected grids), and enables the rest.
</details>

## Community & support

- [SE Mods Discord](https://discord.gg/PYPFPGf3Ca) — FAQ, troubleshooting, support, bug reports, discussion.
- [Pulsar Discord](https://discord.gg/z8ZczP2YZY) — community of plugin users.

## Credits

*In alphabetical order*

### Patreon

#### Admiral level supporters
- BetaMark
- Bishbash777
- Casinost
- Dorimanx
- wafoxxx

#### Captain level supporters
- CaptFacepalm
- DontFollowOrders
- Gabor
- Lazul
- Linux123123
- Lotan
- mkaito
- ransomthetoaster
- Raidfire

### Developers
- SpaceGT: client side Pulsar
- Avaness: client side Plugin Loader (legacy)
- mkaito: testing and design discussions
- zznty: contributed patches

### Testers
- CaveBadgerMan: SG Dimensions servers
- Dorimanx: contributed patches
- mkaito: testing with his heavy offline world
- Robot10: client side
- Multiple server admins for testing and feedback
- zznty

## Building from source

A pair of client and server plugins based on the
[Space Engineers server plugin template](https://github.com/CometWorks/server-plugin-template).
See [Docs/Development.md](Docs/Development.md) for building, versioning and deployment.
