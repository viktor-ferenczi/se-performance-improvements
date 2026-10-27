# `Shared/Tools/ScriptCacheKey.cs`

*Builds the cache key of a compiled script assembly as one SHA-1 over an ordered sequence of strings.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`ScriptCacheKey.cs`](../../../../Shared/Tools/ScriptCacheKey.cs) (40 lines) |
| **Kind** | Static class `ScriptCacheKey` |
| **Role** | Misc utility |

## Purpose

[`MyScriptCompilerPatch`](../Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) names its `.cache` files after this key. Each part is written length-prefixed (`BinaryWriter.Write(string)`, UTF-8) after the `Format` tag, and the whole buffer is hashed once, so duplicate parts don't cancel out, order counts and `"ab","c"` differs from `"a","bc"`. A null part hashes like an empty one. It has no game dependency, so `Tests` compiles it directly.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Format` | Constant | Key format tag, the first hashed part. Bump it when the key layout or the cache file format changes, so old cache files never match. |
| `Compute(parts)` | Static method | Returns the lowercase hex SHA-1 of the format tag and the parts. |

## References

- [`MyScriptCompilerPatch.cs`](../Patches/ScriptCompiler/MyScriptCompilerPatch.cs.md) — the only caller

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
