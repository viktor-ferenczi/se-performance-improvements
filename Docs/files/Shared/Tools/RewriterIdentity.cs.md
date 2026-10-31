# `Shared/Tools/RewriterIdentity.cs`

*Content ID of a rewriter plugin that survives a rebuild from unchanged source, and the redirect from older builds' random assembly names to the loaded build.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`RewriterIdentity.cs`](../../../../Shared/Tools/RewriterIdentity.cs) |
| **Kind** | Internal static class `RewriterIdentity` |
| **Role** | Compilation cache-key component |

## Purpose

Pulsar and Magnetar compile plugins non-deterministically under a random assembly name, so a rebuild from the same source gets a new name, MVID and timestamp. The name also changes the layout of the string heap, which moves the string offsets in the metadata tables, so hashing the file does not work. Quasar's cluster preparation always rebuilds, because it works in a new folder every time, hub-pinned plugins included. Keying the compilation cache on the MVID recompiled every mod on each deployment.

`ContentId` hashes what the code depends on and leaves out the name: the section holding the method bodies, field data and resources without the metadata, the blob and user string heaps, and the metadata tables with strings in place of their offsets. It leaves out the assembly and module rows except for the version. A file it cannot read falls back to the MVID, which only costs recompilations.

Reusing a compiled mod also needs the reference to resolve: linux-compat emits calls to its shims, so a compiled mod references the random name of the build it was compiled against. `RedirectOldBuilds` handles that with an `AssemblyResolve` handler, mapping `Name_xxxxxxxx.xxx` to the loaded rewriter of the same `Name`. The content ID has already guaranteed that the code is the same.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `ContentId(Assembly)` / `ContentId(byte[])` | Static methods | The first 16 bytes of the SHA1 described above, as a `Guid`. |
| `RedirectOldBuilds(rewriters)` | Static method | Installs the `AssemblyResolve` handler for the rewriters with a random name. |
| `PluginNameOf(assemblyName)` | Static method | The name without the random suffix, null if it has none. |

## References

- [`ModRewriterVersions.cs`](ModRewriterVersions.cs.md) and [`LegacyModRewriters.cs`](LegacyModRewriters.cs.md) — the callers.
- [`MyScriptCompilerPatch.cs`](../Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) — loads the compiled mods, which is when the redirect fires.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
