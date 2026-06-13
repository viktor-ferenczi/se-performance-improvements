# `Shared/Tools/Hashing.cs`

*Static utility providing FNV-1a string hashing, IL-body hashing for Harmony `MethodInfo`/`ConstructorInfo`, and a combining hash accumulator.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`Hashing.cs`](../../../../Shared/Tools/Hashing.cs) (103 lines) |
| **Kind** | Static utility class `Hashing` |
| **Role** | Hashing utility |

## Purpose

`Hashing` supplies the hash functions used by the IL-verification pipeline. `HashBody()` on a `MethodInfo` or `ConstructorInfo` retrieves the current IL via `PatchProcessor.GetCurrentInstructions` and passes it through `HashInstructions()` and `CombineHashCodes()` to produce a stable 32-bit fingerprint of the method's compiled body.

This fingerprint is the value stored in `[EnsureCode("hexhash")]` attributes. When [`EnsureCode.cs`](EnsureCode.cs.md)`.Verify()` runs at startup it calls `HashBody()` on each patched method and compares the result against the stored value, catching game updates that silently change the IL that a Harmony patch depends on.

Two overloads of `HashInstructions` exist: one for Harmony `CodeInstruction` lists (used by transpilers at runtime) and one for Mono.Cecil `Instruction` collections (used by preloader patches that operate on raw assembly IL before loading).

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Hash(string)` | Extension method | FNV-1a hash of a string; used as the per-string component when hashing IL operands. |
| `HashBody(MethodInfo)` | Extension method | Produces a combined 32-bit hash of all IL instructions in the method body. |
| `HashBody(ConstructorInfo)` | Extension method | Same for constructor bodies. |
| `HashInstructions(IEnumerable<CodeInstruction>)` | Extension method | Yields per-instruction hash components (opcode + typed operand + labels) for Harmony IL. |
| `HashInstructions(IEnumerable<Instruction>)` | Extension method | Same for Mono.Cecil `Instruction` sequences (preloader path). |
| `CombineHashCodes(IEnumerable<int>)` | Extension method | Folds a sequence of hash ints into a single 32-bit value using a two-lane shift-xor mixer. |

## References

- [`EnsureCode.cs`](EnsureCode.cs.md) — primary consumer of `HashBody()`.
- [`PreloaderHelpers.cs`](PreloaderHelpers.cs.md) — uses the Mono.Cecil overloads of `HashInstructions` and `CombineHashCodes` for preload-time IL verification.
- [`TranspilerHelpers.cs`](TranspilerHelpers.cs.md) — uses `Hash(string)` and `CombineHashCodes` when recording/verifying transpiler IL in DEBUG mode.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
