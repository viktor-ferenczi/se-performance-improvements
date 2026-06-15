# `Shared/Tools/TranspilerHelpers.cs`

*Static helper class for Harmony transpiler patches: IL search, hash verification, debug IL recording, and deep-clone utilities for `CodeInstruction` sequences.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`TranspilerHelpers.cs`](../../../../Shared/Tools/TranspilerHelpers.cs) (268 lines) |
| **Kind** | Static utility class `TranspilerHelpers` + exception class `CodeInstructionNotFound` |
| **Role** | Harmony / IL patching helper |

## Purpose

Harmony transpiler methods receive a `List<CodeInstruction>` and must return a modified list. `TranspilerHelpers` provides the extension methods that all transpiler patches in the plugin use to find, verify, and mutate that instruction list without writing boilerplate LINQ in every patch file.

Search helpers (`FindAllIndex`, `GetField`, `FindPropertyGetter`, `FindPropertySetter`, `GetLabel`) locate specific IL patterns. Mutation helpers (`RemoveFieldInitialization`) surgically remove instruction ranges. `VerifyCodeHash` ties into [`Hashing.cs`](Hashing.cs.md) to assert the method body has not changed since the patch was written, matching the runtime verification approach of [`EnsureCode.cs`](EnsureCode.cs.md) but from within the transpiler itself.

In `DEBUG` builds `RecordOriginalCode` and `RecordPatchedCode` dump the before/after IL to `.il` files next to the calling source file, making it easy to inspect exactly what changed. `DeepClone` ensures copied instructions carry independent label and exception-block lists.

`CodeInstructionNotFound` is a dedicated exception thrown when a search fails, making it clear the target IL pattern was not found (usually a game-update signal).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `FindAllIndex(il, predicate)` | Extension method | Returns indices of all `CodeInstruction`s satisfying the predicate. |
| `GetField(il, predicate)` | Extension method | Finds the `FieldInfo` operand of the first `ldfld`/`stfld` matching the predicate; throws `CodeInstructionNotFound` if absent. |
| `FindPropertyGetter` / `FindPropertySetter` | Extension methods | Locate `call get_Name` / `call set_Name` instructions by property name. |
| `GetLabel(il, predicate)` | Extension method | Extracts a `Label` operand matching an opcode predicate. |
| `RemoveFieldInitialization(il, name)` | Extension method | Removes the `ldarg.0` + `newobj` + `stfld` triple that initialises a named field. |
| `Hash(il)` | Extension method | Computes a hex hash string of the instruction list via [`Hashing.cs`](Hashing.cs.md). |
| `VerifyCodeHash(il, method, expected)` | Extension method | Throws if the current hash differs from `expected`. |
| `RecordOriginalCode` / `RecordPatchedCode` / `RecordCustomCode` | Extension methods *(DEBUG only)* | Write formatted IL to `.il` files beside the calling source file. |
| `DeepClone(CodeInstruction)` / `DeepClone(IEnumerable<CodeInstruction>)` | Extension methods | Produce independent copies including label and exception-block lists. |
| `CodeInstructionNotFound` | Exception class | Thrown by search helpers when no matching instruction is found. |

## References

- [`Hashing.cs`](Hashing.cs.md) — supplies `HashInstructions` and `CombineHashCodes` used by `Hash()` and `VerifyCodeHash()`.
- [`EnsureCode.cs`](EnsureCode.cs.md) — attribute-based IL verification counterpart (used on prefix/postfix methods rather than inside transpilers).
- [`PreloaderHelpers.cs`](PreloaderHelpers.cs.md) — Mono.Cecil counterpart for preloader patches.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
