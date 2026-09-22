# `ClientPlugin/Preloader.cs`

*Namespace-less preloader hook Pulsar calls before the game starts; installs the early Harmony bootstrap so the `"Early"` patch category is applied before the game's own startup preloading.*

|  |  |
| --- | --- |
| **Module** | [Client Plugin Entry Point](../../modules/client-plugin.md) |
| **Source** | [`Preloader.cs`](../../../ClientPlugin/Preloader.cs) (20 lines) |
| **Kind** | Top-level class (no namespace) |
| **Role** | Early bootstrap hook |

## Purpose

`MySandboxGame` starts preloading the vanilla sounds and asteroid voxel storages from its own startup path, which runs *before* `IPlugin.Init` — the normal point where the plugin applies its Harmony patches. The patches which skip that preloading (see [`MySandboxGamePatchForVoxelPreload.cs`](../Shared/Patches/Voxel/MySandboxGamePatchForVoxelPreload.cs.md)) are therefore applied through this earlier hook instead, mirroring what the dedicated server already does in [`ServerPlugin/Preloader.cs`](../ServerPlugin/Preloader.cs.md).

The class must be a top-level type with **no namespace**: Pulsar locates it via `assembly.GetType("Preloader")`, which only resolves an un-namespaced type. The plugin does no Mono.Cecil pre-patching, so it declares neither `TargetDLLs` nor a `Patch` method — only the `Finish()` post-hook, which runs after Pulsar's game assembly resolver is in place but before the game's `Main`. It hands off to [`Plugin.cs`](Plugin.cs.md).`InstallEarlyBootstrap`, which installs a Harmony postfix on `MyInitializer.InvokeBeforeRun`; that postfix applies the `"Early"` category once the game's filesystem, logging and config are ready.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Finish()` | `static void` | Pulsar post-hook entry point. Calls [`Plugin.cs`](Plugin.cs.md).`InstallEarlyBootstrap`. |

## References

- [`Plugin.cs`](Plugin.cs.md) — `InstallEarlyBootstrap` and `OnGameInitialized`, the early phase this hook kicks off.
- [`ServerPlugin/Preloader.cs`](../ServerPlugin/Preloader.cs.md) — the dedicated-server counterpart.
- [`PatchHelpers.cs`](../Shared/Patches/PatchHelpers.cs.md) — applies the `"Early"` category, then the uncategorized and `"Late"` phases.
- [`MySandboxGamePatchForVoxelPreload.cs`](../Shared/Patches/Voxel/MySandboxGamePatchForVoxelPreload.cs.md) — the patch whose timing this hook exists to guarantee.

---

*[Handbook](../../TOC.md) · [Module: Client Plugin Entry Point](../../modules/client-plugin.md) · [Index](../../Index.md)*
