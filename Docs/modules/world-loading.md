# World Loading Patches

Mod loading and programmable blocks share a bounded compilation queue. The cache can reuse successful assemblies across sessions. Other patches reduce blueprint log traffic and speed up image decoding and XML reads.

## Files

| File | Summary |
| --- | --- |
| [`MyDefinitionManagerPatch.cs`](../files/Shared/Patches/DefinitionManager/MyDefinitionManagerPatch.cs.md) | Replaces double dictionary lookup in `GetBlueprintDefinition` with `GetValueOrDefault`, eliminating log flooding. |
| [`MyScriptCompilerPatch.cs`](../files/Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) | Caches compiled mod and PB script assemblies to disk, skipping Roslyn on subsequent world loads. |
| [`ImageLoader.cs`](../files/Shared/Patches/Image/ImageLoader.cs.md) | Decodes an image through the bundled ImageSharp and, in debug builds, logs size, time and a hash of the pixels for both decoders. |
| [`ImageSharpRuntime.cs`](../files/Shared/Patches/Image/ImageSharpRuntime.cs.md) | Renames the shipped ImageSharp 2.1.13 with Cecil, loads it beside the game's copy and decodes through reflection with the same pixel format selection as `MyImage.Load`. |
| [`MyImagePatch.cs`](../files/Shared/Patches/Image/MyImagePatch.cs.md) | Prefix on `MyImage.Load` that routes decoding to the bundled library, falling back to the game's decoder on any failure. |
| [`XmlSerializationReaderPatch.cs`](../files/Shared/Patches/Serialization/XmlSerializationReaderPatch.cs.md) | Transpiler on `XmlSerializationReader.Init` that reuses the generated readers' name IDs per name table. |
| [`CompilationWorkers.cs`](../files/Shared/Patches/ScriptCompiler/CompilationWorkers.cs.md) | Queues mod and programmable-block compilations on the same set of workers. |
| [`EditorCompilation.cs`](../files/Shared/Patches/ScriptCompiler/EditorCompilation.cs.md) | Checks programmable-block editor code through the shared worker queue. |
| [`ModCompilation.cs`](../files/Shared/Patches/ScriptCompiler/ModCompilation.cs.md) | Batches mod compilation at world startup and registers completed assemblies in discovery order. |
| [`ProgrammableBlockCompilation.cs`](../files/Shared/Patches/ScriptCompiler/ProgrammableBlockCompilation.cs.md) | Submits live programmable-block compiles without waiting on the simulation thread. |

## How it fits together

The platform compiler queues both mod and PB requests, including cache lookups, with a default limit of two workers. During world startup the mod loader waits for the discovered scripts and registers them in order before the factories read their assemblies. During play, PB edits and editor checks return to the game thread for activation or diagnostics. A later edit or a closed block invalidates an outstanding result.

The queue runs when the plugin is enabled and `CompileOnWorkerThreads` is on. With the flag off, mods, live PBs and editor checks use the original compilation path. The two cache switches independently control disk reads and writes. A compile that fails has no assembly to store.

`CompilationCacheRoot` chooses the root for both compilation caches. At startup the plugin creates `Cache/CompiledMods` and `Cache/CompiledInGameScripts` under that root. Relative roots start in the instance’s `Storage` folder.

The image loading fix operates on a different part of world startup. `MyImagePatch` prefixes the one `MyImage.Load` overload every image load funnels into and hands the stream to `ImageLoader`, which decodes through `ImageSharpRuntime`. That class is the only place that knows about the newer ImageSharp: it renames the shipped library with Cecil (so it can be loaded beside the game's own copy on .NET, which allows one assembly per simple name), caches the renamed file under `Common.CacheDir/ImageSharp`, and binds the handful of members it needs by reflection. The pixel format selection is a mirror of `MyImage.Load`, which is what keeps the decoded arrays byte-identical to the game's; planet height maps are terrain, so this is a correctness requirement, not a nicety.

`XmlSerializationReaderPatch` hooks the runtime's `XmlSerializationReader.Init` rather than game code, and it is applied in the `"Early"` category so XML the game reads during its own startup already goes through it. Its cache lives in the name tables' lifetime: a `ConditionalWeakTable` entry disappears with the XML document that owned the name table.

`MyDefinitionManagerPatch` also runs during ordinary gameplay when game code looks up a blueprint.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
