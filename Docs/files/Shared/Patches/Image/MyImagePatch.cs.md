# `Shared/Patches/Image/MyImagePatch.cs`

*Prefix on `MyImage.Load` that routes image decoding to the bundled ImageSharp, falling back to the game's decoder on any failure.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`MyImagePatch.cs`](../../../../../Shared/Patches/Image/MyImagePatch.cs) (84 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

All three `MyImage.Load` overloads funnel into the `Stream` one, so a single prefix covers planet height and material maps, terrain blend textures and non-DDS mod textures. When the fix is enabled the prefix decodes through [`ImageLoader.cs`](ImageLoader.cs.md) and skips the original; when the bundled library is unavailable or a decode throws, it rewinds the stream and lets the original run, so the worst case is the game's own behavior.

`Configure` evaluates `Config.Enabled && Config.UpgradeImageSharp` and asks [`ImageSharpRuntime.cs`](ImageSharpRuntime.cs.md) to load the library; it is re-run on config changes, so the option can be toggled at runtime (the library stays loaded once it has been). Debug builds add a postfix that times and hashes the game's own decoder in the same log format, which is how the fix-off and fix-on runs are compared.

See the "Faster image loading with a newer ImageSharp" section in `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Configure()` / `OnConfigChanged` | Static methods | Gate the patch on `Config.UpgradeImageSharp` and load the bundled library on demand. |
| `LoadPrefix` | Prefix | Decodes through `ImageLoader`; returns true (original runs) when disabled or on failure. |
| `LoadPostfix` | Postfix (debug builds) | Logs the game decoder's result with timing and hash for A/B comparison. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyImage.Load(Stream, bool, bool, string)` | Prefix | Decodes with the bundled ImageSharp and skips the original on success. |
| `MyImage.Load(Stream, bool, bool, string)` | Postfix (debug) | Logs the game decoder's output for comparison. |
## References

- [`ImageLoader.cs`](ImageLoader.cs.md)
- [`ImageSharpRuntime.cs`](ImageSharpRuntime.cs.md)
- [`PatchHelpers.cs`](../PatchHelpers.cs.md)
- [`MyScriptCompilerPatch.cs`](../ScriptCompiler/MyScriptCompilerPatch.cs.md)
- [world-loading](../../../../modules/world-loading.md)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
