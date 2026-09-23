# `Shared/Tools/LegacyModRewriters.cs`

*Legacy fallback for [`ModRewriterVersions.cs`](ModRewriterVersions.cs.md): recognizes the pre-`Rewrite`-hook dotnet-compat and linux-compat builds loaded by old Pulsar and Magnetar releases by their compiler-hook marker types, so their MVIDs still feed the compilation cache keys.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`LegacyModRewriters.cs`](../../../../Shared/Tools/LegacyModRewriters.cs) (76 lines) |
| **Kind** | Internal static class `LegacyModRewriters` |
| **Role** | Compatibility fallback (scheduled for removal) |

## Purpose

Old Pulsar and Magnetar releases predate the `Rewrite` hook and load the compat plugins by their legacy IDs (`se-dotnet-compat`, `se-linux-compat`), pinned at builds that rewrite mod scripts on their own: dotnet-compat patches `MyScriptCompiler.CreateCompilation` and exposes the `CompilerHookExtensions.RewriterFactories` list, and linux-compat appends its `PathSubstitutionRewriter` to that list from its `RewriterRegistration` type. Neither declares a `Rewrite` method on its main type, so the modern detection in [`ModRewriterVersions.cs`](ModRewriterVersions.cs.md) does not see them, yet the cached mod assemblies still depend on their exact builds (they even reference shim types inside the linux-compat assembly).

`Find` therefore recognizes those builds by the presence of the hook types, probing each loaded non-dynamic assembly for the four marker type names (the `ClientPlugin.Rewriter.*` and `ServerPlugin.Rewriter.*` variants of both). The types exist as soon as the plugin assemblies are loaded, so this works from the preloader hook and independently of plugin `Init` order. Each match yields the same `(Mvid, Description)` shape as the modern scan, with a `legacy <plugin>` prefix in the description, so the MVIDs are hashed identically. `Assembly.GetType` is wrapped because it can throw instead of returning null when a type's dependencies fail to resolve.

The whole file is marked LEGACY FALLBACK: it, its entry in `Shared.projitems` and its single call site in `ModRewriterVersions.FindModRewriters` are to be removed once the loaders pinning those compat builds (dotnet-compat `90b08f2` client / `0f01870` server, linux-compat `0d7204b`) are no longer supported.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `MarkerTypes` | `static readonly (string TypeName, string PluginName)[]` | Full names of the four hook marker types with the compat plugin each identifies. |
| `Find()` | Static method | One `(Mvid, Description)` per loaded assembly containing a marker type; empty when none are loaded. |
| `GetTypeOrNull(Assembly, string)` | Private static method | `Assembly.GetType(name, throwOnError: false)` that also swallows the exceptions unresolved dependencies can raise. |

## References

- [`ModRewriterVersions.cs`](ModRewriterVersions.cs.md) — the only caller; falls back to `Find()` when no plugin declares `Rewrite`.
- [`MyScriptCompilerPatch.cs`](../Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) — the compilation cache whose keys the detected MVIDs protect.
- [world-loading](../../../modules/world-loading.md) — the module owning that cache.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
