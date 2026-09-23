# `Shared/Patches/Image/ImageSharpRuntime.cs`

*Loads a renamed copy of the shipped ImageSharp 2.1.13 beside the game's own 2019 beta and decodes images through reflection with the same pixel format selection as `MyImage.Load`.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`ImageSharpRuntime.cs`](../../../../../Shared/Patches/Image/ImageSharpRuntime.cs) (305 lines) |
| **Kind** | Static utility class |
| **Role** | Library loader and decoder |

## Purpose

The game's `SixLabors.ImageSharp` (1.0.0-beta0006) is loaded early by dotnet-compat and linux-compat, and on .NET (Core) a process holds one assembly per simple name, so the newer library cannot be referenced or loaded as it is. `TryLoad` finds the shipped `SixLabors.ImageSharp.dll` next to the plugin (Pulsar and Magnetar restore it from NuGet, the build copies it), rewrites it once per library version with Cecil to the assembly name `SixLabors.ImageSharp.Performance` without a strong name, caches the result under `Common.CacheDir/ImageSharp` together with the library's `System.*` dependencies, and loads it with `Assembly.LoadFrom`. `Bind` then resolves the few members used by reflection: `Image.Identify`, `Image.Load<TPixel>` for `L8`, `L16` and `Rgba32`, the PNG metadata and `Image<TPixel>.CopyPixelDataTo(Span<byte>)`.

`Load` is a line-by-line mirror of the game's `MyImage.Load` decision tree: a grayscale PNG is loaded as one channel even when the caller did not ask for it; one-channel images are 8 or 16 bit gray, everything else RGBA; unsupported combinations return null. The pixels are copied straight into the array a real `MyImage<TData>` instance carries (the type is publicized, since its constructor and setters are private), so callers that type-check for `MyImage<uint>` and friends keep working. The reflective calls happen a handful of times per image; the decoding itself runs at full speed inside the library.

This class deliberately has no ImageSharp type in any signature, so nothing that calls it pulls the library in before `TryLoad` has decided whether it can be used.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `TryLoad()` | Static method | Finds, renames, caches, loads and binds the library; false with a logged reason when unusable. |
| `Load(Stream, bool, bool)` | Static method | Decodes with the same pixel format selection as `MyImage.Load`, returning a real `MyImage<TData>`. |
| `RenameIntoCache` | Static method | Cecil rewrite of the assembly name and strong name, cached per library version. |
| `Bind` | Static method | Resolves the reflected members once; fails early if the library's API moved. |
| `Version` / `IsLoaded` | Properties | The loaded library version and whether it is available. |

## References

- [`ImageLoader.cs`](ImageLoader.cs.md)
- [`MyImagePatch.cs`](MyImagePatch.cs.md)
- [`Common.cs`](../../Plugin/Common.cs.md)
- [world-loading](../../../../modules/world-loading.md)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
