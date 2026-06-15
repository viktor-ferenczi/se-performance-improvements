# World Loading Patches

Speeds up world load by caching compiled mods and in-game scripts, and rate-limits the blueprint-not-found log flooding from `GetBlueprintDefinition`.

World load time is dominated by two independent costs: Roslyn compilation of all mod and PB script assemblies, and log flooding from a dictionary double-lookup in `GetBlueprintDefinition`. This module addresses both.

`MyScriptCompilerPatch` is the headline fix: it intercepts the async compilation pipeline and stores each compiled assembly as a `.cache` file keyed by a SHA-1 hash of the runtime version, game build version, compiler symbols, and source text. On subsequent loads the compiled binary is read back and loaded with `Assembly.Load(byte[])`, bypassing Roslyn entirely. `MyDefinitionManagerPatch` eliminates the double `m_blueprintsById` lookup (ContainsKey + indexer) that caused up to 11,000 "No blueprint with Id" log lines per minute, replacing it with a single `GetValueOrDefault` call.

## Files

| File | Summary |
| --- | --- |
| [`MyDefinitionManagerPatch.cs`](../files/Shared/Patches/DefinitionManager/MyDefinitionManagerPatch.cs.md) | Replaces double dictionary lookup in `GetBlueprintDefinition` with `GetValueOrDefault`, eliminating log flooding. |
| [`MyScriptCompilerPatch.cs`](../files/Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) | Caches compiled mod and PB script assemblies to disk, skipping Roslyn on subsequent world loads. |

## How it fits together

The two patches are independent and operate at different points in the world-load sequence.

`MyDefinitionManagerPatch` fires during ordinary gameplay whenever `GetBlueprintDefinition` is called (not just on load), so its benefit is continuous. It touches no shared state beyond the config flag.

`MyScriptCompilerPatch` is exercised only during world load when `MyScriptCompiler.Compile` is awaited. The patch intercepts the state-machine `MoveNext` via a dynamic `TargetMethod()` to work around Harmony's lack of direct async-method support. `RecallFromCache` is called before compilation begins and `StoreIntoCache` is called after — both consult `GetCachePath` which in turn calls `GetScriptsHash`. The hash computation is the only coupling point between the two helpers; they share no mutable state.

Cache directories (`CompiledMods/`, `CompiledInGameScripts/`) live under `Common.CacheDir` and are created at `Configure()` time. The patch respects `CacheMods` and `CacheScripts` config flags independently, so operators can cache one type but not the other.

There is no interaction between the two files in this module; they are co-located here because both affect world-load performance.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
