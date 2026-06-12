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
each fix and the related Keen support tickets.

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
