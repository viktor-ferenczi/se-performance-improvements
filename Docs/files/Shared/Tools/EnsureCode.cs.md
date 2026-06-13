# `Shared/Tools/EnsureCode.cs`

*Custom attribute that verifies the IL hash of a Harmony-patched game method at plugin startup, detecting game updates that would break a patch.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`EnsureCode.cs`](../../../../Shared/Tools/EnsureCode.cs) (107 lines) |
| **Kind** | `[AttributeUsage(Method)]` attribute class `EnsureCode : Attribute` |
| **Role** | IL verification / safety guard |

## Purpose

`[EnsureCode("hexhash")]` is placed on a Harmony prefix, postfix, or transpiler method alongside the usual `[HarmonyPatch]` attribute. The hex string is the expected FNV-like hash of the patched game method's IL body, computed by [`Hashing.cs`](Hashing.cs.md)`.HashBody()`.

At plugin startup the static `Verify()` method scans every method in the calling assembly for `[EnsureCode]` annotations. For each one it resolves the patched method or constructor via Harmony's `AccessTools`, computes the current IL hash, and compares it against the allowed hash(es) (multiple hashes can be pipe-separated to tolerate minor version differences). Any mismatch produces a [`CodeChange.cs`](CodeChange.cs.md) record instead of throwing immediately, so all broken patches are reported together before the plugin decides whether to continue or abort.

This mechanism is critical for a long-lived plugin like Performance Improvements: when Keen releases a game update the startup log immediately names every patch that needs review rather than producing mysterious runtime crashes.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `EnsureCode(allowedHashes)` | Constructor | Takes a pipe-separated list of acceptable hex hashes for the target method body. |
| `Verify()` *(static)* | Method | Entry point called by plugin init; scans the calling assembly and returns all detected [`CodeChange.cs`](CodeChange.cs.md) mismatches. |
| `IsAllowed(hash)` | Method (private) | Returns `true` if `hash` appears in the pipe-delimited `allowedHashes` string. |

## References

- [`CodeChange.cs`](CodeChange.cs.md) — result type yielded for each hash mismatch.
- [`Hashing.cs`](Hashing.cs.md) — provides `HashBody()` used to compute the current IL hash.
- [`TranspilerHelpers.cs`](TranspilerHelpers.cs.md) — companion helpers used inside the transpiler methods guarded by `[EnsureCode]`.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
