# `Shared/Patches/PatchHelpers.cs`

*Central patch engine: verifies the targeted game methods (via [`EnsureCode.cs`](../Tools/EnsureCode.cs.md)) then applies the Harmony patches — all at once on the client, or in two phases on the dedicated server — logging each applied patch, plus the per-tick update and configuration hooks for every patch module.*

|  |  |
| --- | --- |
| **Module** | [Patch Infrastructure](../../../modules/patch-infrastructure.md) |
| **Source** | [`PatchHelpers.cs`](../../../../Shared/Patches/PatchHelpers.cs) (207 lines) |
| **Kind** | Static utility class |
| **Role** | Plugin entry point (patch lifecycle) |

## Purpose

`PatchHelpers` is the glue between the plugin's startup sequence and every Harmony patch class in the assembly. It is not itself a game patch; it is the engine that applies all the others.

Three public entry points share one private scaffold, `VerifyAndApply`. The client applies everything at once through `HarmonyPatchAll` → `harmony.PatchAll`. The dedicated server splits the work into two phases under the same Harmony id: `HarmonyPatchUncategorized` → `harmony.PatchAllUncategorized` runs early (from the [`Preloader.cs`](../../ServerPlugin/Preloader.cs.md) bootstrap, before world-load compilation) and applies every patch *without* a category; `HarmonyPatchCategory` → `harmony.PatchCategory` then applies the deferred `LateCategory` (`"Late"`) from `Init`, once its by-name target assembly (VRage.EOS) is loaded. See [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) for the two-phase server caller and [`ClientPlugin/Plugin.cs`](../../ClientPlugin/Plugin.cs.md) for the single-phase client one.

`VerifyAndApply` first runs the phase's [`EnsureCode.cs`](../Tools/EnsureCode.cs.md) scan (`Verify`, `VerifyUncategorized`, or `VerifyCategory`) to check the targeted game methods for unexpected bytecode changes (e.g. from a game update or a conflicting plugin). If any divergence is found it logs the details and returns `false` — or throws, when `handleExceptions` is `false` or the `SE_PLUGIN_THROW_ON_FAILED_METHOD_VERIFICATION` environment variable is set. Only then does it apply the patches. It snapshots `harmony.GetPatchedMethods()` before and after applying, so `LogAppliedPatches` can debug-log each game method this phase patched (naming the patch class targeting it) with a running count, then an info line with the total — a test run can be verified against the log. Note the applied set is fixed at **build time**, not by config: the `Fix*` flags gate behavior inside the patch bodies, not whether a patch is applied, so the count is the same regardless of which fixes are enabled.

`Configure()` is called once after the plugin configuration is loaded (but before patching) to let each patch module read its config and prepare static state. `PatchUpdates()` is called on every simulation tick and drives time-based logic in cache patches (expiry, hit-rate reporting). Both methods enumerate the full set of known patch types explicitly, so the call order is deterministic.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `LateCategory` | `const string` | Harmony patch category (`"Late"`) for patches deferred to `Init` because their target type lives in an assembly not loaded at the server's early bootstrap (VRage.EOS). |
| `HarmonyPatchAll(log, harmony, handleExceptions)` | `static bool` | Client / single-phase entry: verifies all targeted methods via [`EnsureCode.cs`](../Tools/EnsureCode.cs.md), then `harmony.PatchAll`. Returns `false` on failure; throws if `handleExceptions` is `false` or the env var is set. |
| `HarmonyPatchUncategorized(log, harmony, handleExceptions)` | `static bool` | Server early phase: verifies and applies every patch *without* a category via `harmony.PatchAllUncategorized`. |
| `HarmonyPatchCategory(log, harmony, category, handleExceptions)` | `static bool` | Server late phase: verifies and applies only the given category via `harmony.PatchCategory`. Called from `Init` with `LateCategory`. |
| `Configure()` | `static void` | Calls `Configure()` on every patch class that needs one-time post-config initialisation. Called once before patching. |
| `PatchUpdates()` | `static void` | Drives per-tick logic (cache expiry, debug reporting) by calling `Update()` on cache-based patch classes. Called every game simulation tick. |


## References

- [`EnsureCode.cs`](../Tools/EnsureCode.cs.md) — bytecode verification; `VerifyUncategorized` / `VerifyCategory` scope it to one server phase.
- [`ServerPlugin/Plugin.cs`](../../ServerPlugin/Plugin.cs.md) — the two-phase server caller (uncategorized early, `LateCategory` from `Init`).
- [`ServerPlugin/Preloader.cs`](../../ServerPlugin/Preloader.cs.md) — installs the early bootstrap that triggers the uncategorized phase.
- [`ClientPlugin/Plugin.cs`](../../ClientPlugin/Plugin.cs.md) — the single-phase client caller (`HarmonyPatchAll`).
- [merge-and-paste](../../../modules/merge-and-paste.md) — one of the modules whose `Configure()` / `Update()` this class drives.
- [keen-overhead-removal](../../../modules/keen-overhead-removal.md) — another module applied through this class.

---

*[Handbook](../../../TOC.md) · [Module: Patch Infrastructure](../../../modules/patch-infrastructure.md) · [Index](../../../Index.md)*
