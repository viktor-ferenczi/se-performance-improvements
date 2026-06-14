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
- Less memory allocation (lower GC pressule)

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
- Eliminate long pauses from explicit `GC.Collect` calls
- Disable Mod API call statistics overhead

**Simulation & CPU**
- Eliminate the constant EOS P2P `UpdateStats` core load
- Cache safe zone checks (`IsSafe`, `IsActionAllowed`, optimized `IsOutside`)
- Cache wind turbine atmosphere checks
- Cache conveyor network reachability lookups
- Physics optimizations (`RigidBody` getter, Havok thread count, faster cluster reordering)
- Reduce frequent memory allocations (`MyDefinitionId.ToString`, turret targeting, voxel material lookups)
- Less frequent sync of block counts, block access rights and PB access to blocks
- Disable server-side character footprints and wheel trail tracking
- Disable functional blocks in projected grids
- Rate limit log flooding from `GetBlueprintDefinition`

</details>

## Documentation

| Document | What it covers |
| --- | --- |
| [Developer Handbook (TOC)](Docs/TOC.md) | Top-down tour of the codebase: architecture, modules and how the pieces fit together. **Start here** to understand or modify the plugin. |
| [Performance Fixes — Technical Details](Docs/PerformanceFixes.md) | The rationale for every individual fix: the symptom, the mechanism and the Keen support ticket. |
| [File Index](Docs/Index.md) | A flat, searchable list of every source file with a one-line summary and a link to its detailed page. |

The handbook is a generated, cross-referenced reference for the whole `ClientPlugin`,
`ServerPlugin` and `Shared` source tree; see [How this handbook is maintained](Docs/TOC.md#how-this-handbook-is-maintained)
to regenerate it after changing the code.

## Installation

### Client

Have [Pulsar](https://github.com/SpaceGT/Pulsar) installed. Link to the Installer is in the README there.

1. Enable the **Performance Improvements** plugin from the **Plugins** dialog.
2. Apply and restart the game.

### Server

<details>
<summary>Quasar</summary>

Have [Quasar](https://github.com/viktor-ferenczi/Quasar) installed and a server created.

1. Enable the **Performance Improvements** plugin in your config profile(s).
2. Restart the server, so it picks up the plugin

The configuration is on Quasar's Web UI.

The client enables all newly added performance fixes by default.
</details>

<details>
<summary>Magnetar</summary>

For standalone [Magnetar](https://github.com/viktor-ferenczi/Magnetar/) 
you need to reference the Performance Improvements plugin from the `Current` profile.

Edit the profile:
- Linux: `~/.config/Magnetar/Profiles/Current.xml`
- Windows: `%AppData%\Magnetar\Logacy\Profiles\Current.xml`

Directly inside the `<GitHub>` element insert:
```xml
    <GitHubPluginConfig>
      <Id>viktor-ferenczi/se-performance-improvements</Id>
    </GitHubPluginConfig>
```

Configuration file is `Performance.cfg`, created in the `SpaceEngineersDedicated` folder.
**FIXME:** Include a default config here, because only the ones with non-default values are saved.

The server uses conservative defaults and does not enable any recently added performance fixes.
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

This is a pair of client and server plugins built based on the
[Space Engineers server plugin template](https://github.com/viktor-ferenczi/se-server-plugin-template).
For development guidance see [se-dev-skills](https://github.com/viktor-ferenczi/se-dev-skills/).

### Folder path overrides

`Directory.Build.props.template` is a template for `Directory.Build.props`. The latter is a
local config file you can use to override the reference folder paths (`Bin64` for the Space
Engineers client, `Pulsar` and `Magnetar` for the two plugin loaders, and `Dedicated64` for
the Dedicated Server). It is **not committed** to the repository, so each contributor keeps
their own local paths.

`setup.py` copies `Directory.Build.props.template` to `Directory.Build.props` if the latter
does not exist yet, then fills in the auto-detected paths. Because the override is not
committed, anyone else who clones the repo and runs `setup.py` gets their own
`Directory.Build.props` with paths properly auto-detected for their machine. Leaving a path
empty in `Directory.Build.props` falls back to the platform-specific auto-detection further
down in the same file (Windows and Linux), so the build works on both operating systems.
