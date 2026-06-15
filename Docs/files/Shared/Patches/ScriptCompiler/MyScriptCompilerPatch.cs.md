# `Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs`

*Caches compiled mod and in-game script assemblies to disk, short-circuiting the Roslyn compilation step on subsequent world loads.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`MyScriptCompilerPatch.cs`](../../../../../Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs) (295 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

Compiling all mods and PB scripts on world load is very time-consuming and CPU-intensive, particularly on large multiplayer servers running many mods. This patch eliminates that cost on repeated loads by persisting compiled assemblies to disk and loading them from the cache on subsequent runs.

A Transpiler is injected into the `MoveNext` method of the compiler's internal async state machine (`MyScriptCompiler.Compile` enumerator). It inserts two calls around the compilation step: `RecallFromCache` is tried first — if it returns a cached `Assembly`, the transpiler's injected branch jumps past the Roslyn compilation entirely; `StoreIntoCache` is called after a fresh compilation to save the resulting `MemoryStream` as a `.cache` file.

Cache keys are 20-byte SHA-1 hashes computed over the .NET runtime version, the game's build version (from `Common.GameVersion`), the compiler's conditional compilation symbols, and the concatenated source text of every `Script` in the batch. The XOR-accumulation strategy means the hash is order-independent across scripts within a single compilation unit, while the runtime/build/symbol prefixes ensure stale entries are never loaded after a game update or SDK change. Separate directories (`CompiledMods/` and `CompiledInGameScripts/`) are used for the two `MyApiTarget` types. The patch respects the `CacheMods` and `CacheScripts` config flags independently.

When loading from cache the patch uses `Assembly.Load(byte[])` rather than `Assembly.LoadFrom` to avoid preventing world re-loads due to locked files. I/O errors are retried up to 200 times (5 ms sleep each) to handle the race condition where multiple threads try to load the same script simultaneously.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Configure()` | Static method | Creates the cache directories and validates reflection targets at startup. |
| `TargetMethod()` | Static method | Locates the `MoveNext` of the `<Compile>` state machine at runtime, since the type is compiler-generated and cannot be named directly. |
| `CompileTranspiler` | Transpiler | Injects `RecallFromCache` (branch-to-exit on hit) and `StoreIntoCache` (after compilation) into the async state machine. |
| `RecallFromCache` | Static method | Materialises the script list, computes the cache path, loads bytes and returns an `Assembly`; returns `null` on miss. |
| `StoreIntoCache` | Static method | Writes the `MemoryStream` of a freshly compiled assembly to the cache file, then rewinds the stream. |
| `GetCachePath` | Static method | Checks config flags and constructs the `.cache` file path from the computed hash. |
| `GetScriptsHash` | Static method | Computes the XOR-accumulated SHA-1 cache key from runtime version, game version, symbols, and script source texts. |
| `InGameScriptCacheDir` / `ModScriptCacheDir` | Static properties | Lazily-initialised paths inside `Common.CacheDir` for each target type. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyScriptCompiler.Compile` (state-machine `MoveNext`) | Transpiler | Bypasses Roslyn compilation when a cached assembly exists; persists newly compiled assemblies to disk. |

## References

- [world-loading](../../../../modules/world-loading.md)
- [`Hashing.cs`](../../Tools/Hashing.cs.md) — SHA-1 used for cache-key generation
- [`PreloaderHelpers.cs`](../../Tools/PreloaderHelpers.cs.md) — provides `Common.CacheDir` and `Common.GameVersion`

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
