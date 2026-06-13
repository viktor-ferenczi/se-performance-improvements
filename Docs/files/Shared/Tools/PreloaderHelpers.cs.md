# `Shared/Tools/PreloaderHelpers.cs`

*Static helper class for Mono.Cecil-based preloader patches: IL index search, hash verification, and debug IL recording over `Collection<Instruction>`.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`PreloaderHelpers.cs`](../../../../Shared/Tools/PreloaderHelpers.cs) (282 lines) |
| **Kind** | Static utility class `PreloaderHelpers` |
| **Role** | Harmony / IL patching helper (preloader path) |

## Purpose

Some patches in the plugin must run as *preloader* patches — they rewrite game assemblies using Mono.Cecil before those assemblies are loaded into the CLR, rather than using Harmony at runtime. `PreloaderHelpers` provides the Mono.Cecil equivalents of the helpers in [`TranspilerHelpers.cs`](TranspilerHelpers.cs.md).

`FindFirstIndex`, `FindLastIndex`, and `FindAllIndex` search a `Collection<Instruction>` (Cecil's IL sequence type) by predicate, enabling preloader patch methods to locate specific opcodes or operands. `VerifyCodeHash` checks that the method body hash matches an expected value and throws a descriptive exception if not, preventing a broken preloader patch from silently modifying wrong code after a game update. `Hash()` computes the same FNV-derived fingerprint as [`Hashing.cs`](Hashing.cs.md).

In `DEBUG` builds `RecordOriginalCode`, `RecordPatchedCode`, and `RecordCustomCode` dump formatted IL text alongside the source file of the calling preloader patch (`.original.il` / `.patched.il`). This makes it easy to inspect exactly what the patch did during development, and the hash printed at the top of each file can be copy-pasted into a `VerifyCodeHash` call.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `FindFirstIndex(il, predicate)` | Extension method | Returns the index of the first instruction satisfying `predicate`, or `-1` if none (via `DefaultIfEmpty(-1).First()`, kept .NET Framework 4.8-compatible). |
| `FindLastIndex(il, predicate)` | Extension method | Returns the index of the last matching instruction, or `-1` if none. |
| `FindAllIndex(il, predicate)` | Extension method | Returns a list of all matching instruction indices. |
| `Hash(il)` | Extension method | Computes a hex hash string of the Cecil instruction sequence via [`Hashing.cs`](Hashing.cs.md). |
| `VerifyCodeHash(il, method, expected)` | Extension method | Throws if the current hash does not match `expected`, identifying the patched method in the error message. |
| `RecordOriginalCode` / `RecordPatchedCode` / `RecordCustomCode` | Extension methods *(DEBUG only)* | Write formatted IL text to `.il` files next to the calling source file for manual inspection. |

## References

- [`TranspilerHelpers.cs`](TranspilerHelpers.cs.md) — runtime (Harmony `CodeInstruction`) counterpart with the same helper patterns.
- [`Hashing.cs`](Hashing.cs.md) — supplies `HashInstructions` and `CombineHashCodes` called by `Hash()`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
