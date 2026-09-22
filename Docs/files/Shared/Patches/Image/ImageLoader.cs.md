# `Shared/Patches/Image/ImageLoader.cs`

*Decodes an image through the bundled ImageSharp and, in debug builds, logs size, time and a hash of the decoded pixels for both this and the game's decoder.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`ImageLoader.cs`](../../../../../Shared/Patches/Image/ImageLoader.cs) (84 lines) |
| **Kind** | Static utility class |
| **Role** | Performance patch helper |

## Purpose

`ImageLoader.Load` is what [`MyImagePatch.cs`](MyImagePatch.cs.md) calls in place of the game's decoder. It times the call into [`ImageSharpRuntime.cs`](ImageSharpRuntime.cs.md) and, when debug logging is on, writes one line per image with the name, size, bits per pixel, time and an FNV-1a hash of the decoded pixel bytes.

The hash is the verification tool for this fix. The same function is implemented by the offline comparison tool in the design notes, and the debug postfix in `MyImagePatch` logs the game's own decoder through `LogGameDecoder` in the same format, so two runs (fix off, fix on) can be compared line by line. Every planet map shipped with the game produced identical hashes on both decoders and on an independent PNG reader.

See the "Faster image loading with a newer ImageSharp" section in `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Load(Stream, bool, bool, string)` | Static method | Decodes through `ImageSharpRuntime`, timing the call and logging it in debug builds. |
| `LogGameDecoder` | Static method | Logs an image the game's own decoder produced in the same format, for A/B comparison. |
| `Hash(IMyImage)` | Static method | FNV-1a 64 bit hash over the raw pixel bytes of an `IMyImage`. |

## References

- [`ImageSharpRuntime.cs`](ImageSharpRuntime.cs.md)
- [`MyImagePatch.cs`](MyImagePatch.cs.md)
- [world-loading](../../../../modules/world-loading.md)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
