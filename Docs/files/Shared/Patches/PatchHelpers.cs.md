# `Shared/Patches/PatchHelpers.cs`

*Central patch engine: runs [`EnsureCode.cs`](../Tools/EnsureCode.cs.md) verification, then applies all Harmony patch classes in the assembly, and provides per-tick update and configuration hooks for every patch module.*

|  |  |
| --- | --- |
| **Module** | [Patch Infrastructure](../../../modules/patch-infrastructure.md) |
| **Source** | [`PatchHelpers.cs`](../../../../Shared/Patches/PatchHelpers.cs) (119 lines) |
| **Kind** | Static utility class |
| **Role** | Plugin entry point (patch lifecycle) |

## Purpose

`PatchHelpers` is the glue between the plugin's startup sequence and every Harmony patch class in the assembly. It is not itself a game patch; it is the engine that bootstraps all other patches.

`HarmonyPatchAll` first runs [`EnsureCode.cs`](../Tools/EnsureCode.cs.md).`Verify()` to scan the targeted game methods for unexpected bytecode changes (e.g. from a game update or a conflicting plugin). If any divergence is found it logs the details and returns `false` — or throws, when the `SE_PLUGIN_THROW_ON_FAILED_METHOD_VERIFICATION` environment variable is set. Only when verification passes does it call `harmony.PatchAll(Assembly.GetExecutingAssembly())`, which picks up every class decorated with `[HarmonyPatch]` in the shared assembly.

`Configure()` is called once after the plugin configuration is loaded (but before patching) to let each patch module read its config and prepare static state. `PatchUpdates()` is called on every simulation tick and drives time-based logic in cache patches (expiry, hit-rate reporting). Both methods enumerate the full set of known patch types explicitly, so the call order is deterministic.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `HarmonyPatchAll(log, harmony, handleExceptions)` | `static bool` | Verifies targeted methods via [`EnsureCode.cs`](../Tools/EnsureCode.cs.md), then calls `harmony.PatchAll`. Returns `false` on failure; throws if `handleExceptions` is `false` or the env var is set. |
| `Configure()` | `static void` | Calls `Configure()` on every patch class that needs one-time post-config initialisation. Called by the plugin before `HarmonyPatchAll`. |
| `PatchUpdates()` | `static void` | Drives per-tick logic (cache expiry, debug reporting) by calling `Update()` on cache-based patch classes. Called every game simulation tick. |


## References

- [`EnsureCode.cs`](../Tools/EnsureCode.cs.md) — bytecode verification used before patching.
- [merge-and-paste](../../../modules/merge-and-paste.md) — one of the modules whose `Configure()` / `Update()` this class drives.
- [keen-overhead-removal](../../../modules/keen-overhead-removal.md) — another module bootstrapped through this class.

---

*[Handbook](../../../TOC.md) · [Module: Patch Infrastructure](../../../modules/patch-infrastructure.md) · [Index](../../../Index.md)*
