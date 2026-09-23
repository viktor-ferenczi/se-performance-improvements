# `Shared/Tools/ModRewriterVersions.cs`

*Detects the loaded mod-rewriting plugins and hashes their module version IDs, so the exact build of each rewriter contributes to the mod and in-game script compilation cache keys.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`ModRewriterVersions.cs`](../../../../Shared/Tools/ModRewriterVersions.cs) (147 lines) |
| **Kind** | Static class `ModRewriterVersions` |
| **Role** | Compilation cache-key component |

## Purpose

[`MyScriptCompilerPatch.cs`](../Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) caches compiled mod and in-game script assemblies on disk, keyed by a hash of the script sources, the .NET runtime, the game version and the compilation symbols (see [world-loading](../../../modules/world-loading.md)). Some plugins, in practice the dotnet-compat and linux-compat plugins, rewrite mod scripts before compilation, so the compiled assembly depends on their identity too. Without this class, upgrading or recompiling a rewriter would keep serving cached assemblies produced by the old build, or ones referencing an assembly name that no longer loads. `Hash` is XOR-folded into `GetScriptsHash` whenever it is non-null.

Detection uses the loaders' own contract instead of their bookkeeping: Pulsar and Magnetar wire up rewriting by looking for a method named `Rewrite` on a plugin's main type, the one implementing `VRage.Plugins.IPlugin`. `FindModRewriters` scans every loaded non-dynamic assembly for such types, matching the interface by name so the declaring game assembly is not loaded (on the dedicated server this runs from the preloader hook, before the game starts) and enumerating types tolerantly for assemblies with unresolvable references. Each matching assembly contributes its module version ID (MVID) once. The MVID is the right identity because it changes exactly when the build does: Pulsar compiles plugins non-deterministically, minting a fresh MVID per compile, and deterministically built DLLs get a content hash, whereas the assembly version routinely survives recompiles. When no plugin declares `Rewrite`, the scan falls back to [`LegacyModRewriters.cs`](LegacyModRewriters.cs.md) for the compat builds loaded by old Pulsar and Magnetar releases.

`Initialize` fails hard by design. If not a single `IPlugin` implementation is found, the scan ran before the plugin assemblies were loaded or the contract moved, and the exception propagates to the loader, which logs it as an ERROR and reports the plugin as failed. Silently skipping would poison the compilation caches with wrongly keyed entries. It is idempotent because the dedicated server calls it twice: from [`Preloader.cs`](../../ServerPlugin/Preloader.cs.md)`.Finish` (detection only, before the game log exists) and again from [`Common.cs`](../Plugin/Common.cs.md)`.SetPlugin`, which then calls `LogVersions` once a logger is available. The client only has the `SetPlugin` call.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Hash` | `static byte[]` property | SHA1 over the sorted MVIDs of the loaded rewriter assemblies; null when none are loaded, leaving the cache keys unchanged. |
| `Initialize()` | Static method | Idempotent; runs `FindModRewriters`, sorts by MVID, hashes the concatenated 16-byte GUIDs and records the descriptions. Throws if no `IPlugin` implementation is loaded at all. |
| `LogVersions(IPluginLogger)` | Static method | Logs the detected rewriters, or that none are loaded; deferred until a logger exists. |
| `FindModRewriters()` | Private static method | Scans loaded assemblies for concrete `IPlugin` types declaring a `Rewrite` method; one `(Mvid, Description)` per assembly. Falls back to `LegacyModRewriters.Find()` when empty. |
| `GetLoadableTypes(Assembly)` | Private static method | `assembly.GetTypes()` tolerant of `ReflectionTypeLoadException`, yielding only the types that did load. |
| `PluginInterfaceName` / `RewriteMethodName` | `const string` | `"VRage.Plugins.IPlugin"` and `"Rewrite"`, the loader contract matched by name. |

## References

- [`MyScriptCompilerPatch.cs`](../Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) — folds `Hash` into the compilation cache key in `GetScriptsHash`.
- [`LegacyModRewriters.cs`](LegacyModRewriters.cs.md) — the fallback for compat builds predating the `Rewrite` hook.
- [`Preloader.cs`](../../ServerPlugin/Preloader.cs.md) — first call to `Initialize` on the dedicated server, from the loader's post-hook.
- [`Common.cs`](../Plugin/Common.cs.md) — `SetPlugin` calls `Initialize` (client) or re-enters it idempotently (server), then `LogVersions`.
- [`IPluginLogger.cs`](../Logging/IPluginLogger.cs.md) — the logger `LogVersions` writes to.
- [world-loading](../../../modules/world-loading.md) — the compilation cache this key protects.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
