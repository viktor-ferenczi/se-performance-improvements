# `ServerPlugin/Preloader.cs`

*Namespace-less preloader hook the Magnetar dedicated-server loader calls before the game starts; installs the early Harmony bootstrap so the plugin's patches are applied before world-load mod/script compilation.*

|  |  |
| --- | --- |
| **Module** | [Server Plugin Entry Point](../../modules/server-plugin.md) |
| **Source** | [`Preloader.cs`](../../../ServerPlugin/Preloader.cs) (24 lines) |
| **Kind** | Top-level class (no namespace) |
| **Role** | Early bootstrap hook |

## Purpose

On the dedicated server the world — including mod and in-game script compilation — is loaded *before* `IPlugin.Init` runs, which is the normal point where the plugin applies its Harmony patches. For the on-disk compilation cache (see [`MyScriptCompilerPatch.cs`](../Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) and the [world-loading](../../modules/world-loading.md) module) to actually populate, the cache-relevant patches must already be in place when that compilation happens. `Preloader` is the earliest hook the Magnetar loader exposes, and is where that early bootstrap is triggered.

The class must be a top-level type with **no namespace**: Magnetar (like the Pulsar client loader) locates it via `assembly.GetType("Preloader")`, which only resolves an un-namespaced type. The plugin does no Mono.Cecil pre-patching, so it declares neither `TargetDLLs` nor a `Patch` method — only the `Finish()` post-hook. Magnetar still runs `Finish()` because its `HasPatches` check counts post-hooks. `Finish()` runs in the loader's `SetupPlugins` stage, right after the game assembly resolver is set up but before the game starts; the game has not initialised yet, so it hands off to [`Plugin.cs`](Plugin.cs.md).`InstallEarlyBootstrap`, which installs a Harmony postfix on `MyInitializer.InvokeBeforeRun`. That postfix runs the rest of early startup once the game's filesystem and logging are ready — still well before world-load compilation.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Finish()` | `static void` | Loader post-hook entry point. Calls [`Plugin.cs`](Plugin.cs.md).`InstallEarlyBootstrap` to install the early Harmony bootstrap. |

## References

- [`Plugin.cs`](Plugin.cs.md) — `InstallEarlyBootstrap` and `EarlyStartup`, the two-phase patching this hook kicks off.
- [`PatchHelpers.cs`](../Shared/Patches/PatchHelpers.cs.md) — applies the uncategorized patches early, and the deferred "Late" category from `Init`.
- [`MyScriptCompilerPatch.cs`](../Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) — the compilation-cache patch whose early application this timing exists to guarantee.
- [world-loading](../../modules/world-loading.md) — the module that benefits from the cache being populated before world load.

---

*[Handbook](../../TOC.md) · [Module: Server Plugin Entry Point](../../modules/server-plugin.md) · [Index](../../Index.md)*
