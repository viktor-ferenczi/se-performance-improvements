# Development

This is a pair of client and server plugins built based on the
[Space Engineers server plugin template](https://github.com/CometWorks/server-plugin-template).
For development guidance see [se-dev-skills](https://github.com/viktor-ferenczi/se-dev-skills/).

The [Developer Handbook](TOC.md) is a generated, cross-referenced reference for the whole
`ClientPlugin`, `ServerPlugin` and `Shared` source tree; see
[How this handbook is maintained](TOC.md#how-this-handbook-is-maintained) to regenerate it after
changing the code.

## Plugin version

The version is written in four places, and a version bump changes all of them in one commit:

| File | What | Used by |
|---|---|---|
| `Version.Build.props` | `<Version>` | local builds (`dotnet build`), imported by `Directory.Build.props` |
| `ClientPlugin/Plugin.cs` | `AssemblyVersion` and `AssemblyFileVersion` under `#if !LOCAL_BUILD` | the client plugin built from source by Pulsar |
| `ServerPlugin/Plugin.cs` | the same two attributes | the server plugin built from source by Magnetar |
| `Shared/Plugin/Common.cs` | `PluginVersion` | both plugins; a change clears the debug files of the old version |

Pulsar and Magnetar compile only the `.cs` files, so they never see `Version.Build.props`. A
version missing from the `.cs` files goes unnoticed in a local build but ships the old version
number to the players.

To bump, for example from 1.16.1 to 1.16.2:

```bash
git grep -l -F 1.16.1 -- Version.Build.props ClientPlugin/Plugin.cs ServerPlugin/Plugin.cs Shared/Plugin/Common.cs | xargs sed -i 's/1\.16\.1/1.16.2/g'
git grep -n -F 1.16.2 -- Version.Build.props ClientPlugin/Plugin.cs ServerPlugin/Plugin.cs Shared/Plugin/Common.cs
```

The second command has to list six lines: one in `Version.Build.props` and `Common.cs`, two in
each `Plugin.cs`.

## Folder path overrides

`Directory.Build.props` **is** committed and declares the overridable folder paths:

- `Bin64`: the folder containing `SpaceEngineers.exe`
- `Dedicated64`: the folder containing `SpaceEngineersDedicated.exe`
- `Magnetar`: the Magnetar installation folder holding `Libraries/<launcher>/PluginSdk.dll`
- `Pulsar`: the Pulsar folder to deploy the client plugin into, empty by default
- `MagnetarData`: the Magnetar config folder to deploy the server plugin into, the one
  holding `Local`, `Sources` and `Profiles`, empty by default

`Bin64`, `Dedicated64` and `Magnetar` are auto-detected when left empty, on both Windows and
Linux, so a fresh clone normally builds without any setup. `Pulsar` and `MagnetarData` are
never auto-detected.

`Directory.Build.props.user` is where you override those paths. It is **not committed**
(`*.user` is git-ignored), so each contributor keeps their own local paths. Copy the first
`PropertyGroup` of `Directory.Build.props` into it, wrapped into a top-level `Project`
element, and fill in what you need. `setup.py` writes the file for you with the paths it
auto-detects, and leaves any other overrides in it alone on a re-run.

## Development

Load the working copy through a loader development folder: start Pulsar or Magnetar with
`-sources` and add this folder with the Sources button. The loader then compiles the plugin
from source at startup.

Builds deploy nothing by default. To deploy anyway, set `Pulsar` and/or `MagnetarData` in
`Directory.Build.props.user`, or pass them to a single build with `-p:Pulsar=...` and
`-p:MagnetarData=...`. The client build then lands in
`<Pulsar>/Legacy/Local/Performance/` (`net48`) or `<Pulsar>/Interim/Local/Performance/`
(`net10.0`, which on Linux falls back to `Legacy` if there is no `Interim` folder) as
`plugin.dll`, and the `net10.0` server build lands flat in `<MagnetarData>/Local/` as
`Performance.dll`. Both copy the bundled `SixLabors.ImageSharp.dll` next to the plugin, and
the `net48` client build also copies its `System.*` dependencies.
