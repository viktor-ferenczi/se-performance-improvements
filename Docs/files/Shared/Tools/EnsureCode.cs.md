# `Shared/Tools/EnsureCode.cs`

*Custom attribute that verifies the IL hash of a Harmony-patched game method at plugin startup, detecting game updates that would break a patch.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`EnsureCode.cs`](../../../../Shared/Tools/EnsureCode.cs) (135 lines) |
| **Kind** | `[AttributeUsage(Method)]` attribute class `EnsureCode : Attribute` |
| **Role** | IL verification / safety guard |

## Purpose

`[EnsureCode("hexhash")]` is placed on a Harmony prefix, postfix, or transpiler method alongside the usual `[HarmonyPatch]` attribute. The hex string is the expected FNV-like hash of the patched game method's IL body, computed by [`Hashing.cs`](Hashing.cs.md)`.HashBody()`.

At plugin startup the static `Verify()` method scans every method in the calling assembly for `[EnsureCode]` annotations. For each one it resolves the patched method or constructor via Harmony's `AccessTools`, computes the current IL hash, and compares it against the allowed hash(es) (multiple hashes can be pipe-separated to tolerate minor version differences). Any mismatch produces a [`CodeChange.cs`](CodeChange.cs.md) record instead of throwing immediately, so all broken patches are reported together before the plugin decides whether to continue or abort.

On the dedicated server the patches are applied in two phases, so verification is scoped to match. `VerifyUncategorized()` checks only the patch classes *without* a Harmony category (mirroring `PatchAllUncategorized`, the early phase); `VerifyCategory(category)` checks only those in the given category (the deferred `"Late"` phase). Each phase resolves only the game methods it is about to patch, so a patch whose target assembly is not loaded yet — handled in a later phase — does not cause a spurious failure here. These overloads resolve the patch assembly directly (`typeof(EnsureCode).Assembly`, which is the plugin assembly because the `Shared` project is compiled into it), whereas the parameterless `Verify()` finds the calling assembly via the call stack.

This mechanism is critical for a long-lived plugin like Performance Improvements: when Keen releases a game update the startup log immediately names every patch that needs review rather than producing mysterious runtime crashes.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `EnsureCode(allowedHashes)` | Constructor | Takes a pipe-separated list of acceptable hex hashes for the target method body. |
| `Verify()` *(static)* | Method | Entry point for single-phase (client) startup; scans the calling assembly and returns all detected [`CodeChange.cs`](CodeChange.cs.md) mismatches. |
| `VerifyUncategorized()` *(static)* | Method | Verifies only the uncategorized patch classes (the server's early phase); mirrors `Harmony.PatchAllUncategorized`. |
| `VerifyCategory(category)` *(static)* | Method | Verifies only the patch classes in the given Harmony category (the server's deferred `"Late"` phase). |
| `IsAllowed(hash)` | Method (private) | Returns `true` if `hash` appears in the pipe-delimited `allowedHashes` string. |

## References

- [`CodeChange.cs`](CodeChange.cs.md) — result type yielded for each hash mismatch.
- [`PatchHelpers.cs`](../Patches/PatchHelpers.cs.md) — calls `Verify` / `VerifyUncategorized` / `VerifyCategory` before each apply phase.
- [`Hashing.cs`](Hashing.cs.md) — provides `HashBody()` used to compute the current IL hash.
- [`TranspilerHelpers.cs`](TranspilerHelpers.cs.md) — companion helpers used inside the transpiler methods guarded by `[EnsureCode]`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
