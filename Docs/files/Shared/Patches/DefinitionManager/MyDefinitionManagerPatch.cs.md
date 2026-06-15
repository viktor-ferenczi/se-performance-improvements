# `Shared/Patches/DefinitionManager/MyDefinitionManagerPatch.cs`

*Eliminates redundant double-lookup and log flooding in `MyDefinitionManager.GetBlueprintDefinition` via a transpiler that replaces the ContainsKey+indexer pattern with a single `GetValueOrDefault` call.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`MyDefinitionManagerPatch.cs`](../../../../../Shared/Patches/DefinitionManager/MyDefinitionManagerPatch.cs) (68 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyDefinitionManager.GetBlueprintDefinition` was implemented as a double-lookup: first `ContainsKey`, then a second dictionary access to retrieve the value. When the key is absent the method returns `null`, but it also implicitly produced a log message via the negative branch — at rates of up to 11,000 "No blueprint with Id" entries per minute when mods such as Isy's Inventory Manager PB script were active. This excessive logging not only burned CPU time but risked filling disk space over long server uptimes (see the "Rate limited excessive logging" section in `Docs/PerformanceFixes.md`).

The transpiler rewrites the method body to a single `GetValueOrDefault` call on the internal `m_blueprintsById` dictionary, eliminating both the redundant lookup and the log flooding in one step. On Linux/Mono, where `Dictionary<,>.GetValueOrDefault` is not available as a BCL extension, the patch falls back to a locally provided `Workarounds.GetValueOrDefault` helper.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `GetBlueprintDefinitionTranspiler` | Transpiler | Replaces the entire post-argument IL of `GetBlueprintDefinition` with a single `GetValueOrDefault` call; gated on `Config.Enabled && Config.FixMemory`. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyDefinitionManager.GetBlueprintDefinition` | Transpiler | Replaces double lookup (ContainsKey + indexer) with a single `GetValueOrDefault`, eliminating log flooding and the redundant dictionary probe. |

## References

- [world-loading](../../../../modules/world-loading.md)
- [`MyScriptCompilerPatch.cs`](../ScriptCompiler/MyScriptCompilerPatch.cs.md) — sibling world-loading patch in the same module

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
