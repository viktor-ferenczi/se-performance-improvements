# `Shared/Tools/CodeChange.cs`

*Data class describing a detected mismatch between the expected and actual IL hash of a patched game method.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`CodeChange.cs`](../../../../Shared/Tools/CodeChange.cs) (22 lines) |
| **Kind** | Class `CodeChange` |
| **Role** | Diagnostics / IL verification |

## Purpose

`CodeChange` is the result type returned by [`EnsureCode.cs`](EnsureCode.cs.md)`.Verify()`. It carries the identity of the patched method or constructor together with the expected hash string (from the `[EnsureCode]` attribute) and the actual hash computed at runtime.

When a game update changes the IL of a patched method, `EnsureCode.Verify()` yields one `CodeChange` per mismatch. The plugin logs these at startup so developers know which patches need updating before they apply them blindly to changed code.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Method` | Field | The patched `MethodInfo`, or `null` when a constructor is patched. |
| `Constructor` | Field | The patched `ConstructorInfo`, or `null` when a regular method is patched. |
| `Expected` | Field | The pipe-separated hex hash(es) declared in the `[EnsureCode]` attribute. |
| `Actual` | Field | The hex hash computed from the current game IL. |
| `ToString()` | Method | Returns a human-readable description including the full method signature and both hashes. |

## References

- [`EnsureCode.cs`](EnsureCode.cs.md) — attribute and verifier that produces `CodeChange` instances.
- [`Hashing.cs`](Hashing.cs.md) — provides the `HashBody()` extension methods used to compute `Actual`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
