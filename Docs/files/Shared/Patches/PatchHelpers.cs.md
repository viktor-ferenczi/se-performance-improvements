# `Shared/Patches/PatchHelpers.cs`

*Central patch engine: verifies the targeted game methods (via [`EnsureCode.cs`](../Tools/EnsureCode.cs.md)) then applies the Harmony patches in phases — the `"Early"` category from the preloader bootstrap, the uncategorized ones, then the deferred `"Late"` category — logging each applied patch, plus the per-tick update and configuration hooks for every patch module.*

|  |  |
| --- | --- |
| **Module** | [Patch Infrastructure](../../../modules/patch-infrastructure.md) |
| **Source** | [`PatchHelpers.cs`](../../../../Shared/Patches/PatchHelpers.cs) (207 lines) |
| **Kind** | Static utility class |
| **Role** | Plugin entry point (patch lifecycle) |

## Purpose

`PatchHelpers` is the glue between the plugin's startup sequence and every Harmony patch class in the assembly. It is not itself a game patch; it is the engine that applies all the others.

Three public entry points share one private scaffold, `VerifyAndApply`, and both sides apply the patches in phases under the same Harmony id. `HarmonyPatchCategory` → `harmony.PatchCategory` applies the `EarlyCategory` (`"Early"`) from the `MyInitializer.InvokeBeforeRun` hook installed by each side's `Preloader`, before the game starts loading anything — the only point early enough for the voxel-preload patches, since the game's own preloading runs before `IPlugin.Init` on the client. `HarmonyPatchUncategorized` → `harmony.PatchAllUncategorized` applies every patch *without* a category: from `Init` on the client, and early (before world-load compilation) on the dedicated server. The same `HarmonyPatchCategory` then applies the deferred `LateCategory` (`"Late"`) from `Init` on both sides, once its by-name target assembly (VRage.EOS) is loaded. `HarmonyPatchAll` → `harmony.PatchAll` remains as the single-call entry point, unused by either plugin today. See [`Plugin.cs`](../../ServerPlugin/Plugin.cs.md) and [`ClientPlugin/Plugin.cs`](../../ClientPlugin/Plugin.cs.md) for the callers.

`VerifyAndApply` first runs the phase's [`EnsureCode.cs`](../Tools/EnsureCode.cs.md) scan (`Verify`, `VerifyUncategorized`, or `VerifyCategory`) to check the targeted game methods for unexpected bytecode changes (e.g. from a game update or a conflicting plugin). If any divergence is found it logs the details and returns `false` — or throws, when `handleExceptions` is `false` or the `SE_PLUGIN_THROW_ON_FAILED_METHOD_VERIFICATION` environment variable is set. Only then does it apply the patches. It snapshots `harmony.GetPatchedMethods()` before and after applying, so `LogAppliedPatches` can debug-log each game method this phase patched (naming the patch class targeting it) with a running count, then an info line with the total — a test run can be verified against the log. Note the applied set is fixed at **build time**, not by config: the `Fix*` flags gate behavior inside the patch bodies, not whether a patch is applied, so the count is the same regardless of which fixes are enabled.

`Configure()` is called once after the plugin configuration is loaded (but before patching) to let each patch module read its config and prepare static state. `PatchUpdates()` is called on every simulation tick and drives time-based logic in cache patches (expiry, hit-rate reporting). Both methods enumerate the full set of known patch types explicitly, so the call order is deterministic.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `EarlyCategory` | `const string` | Harmony patch category (`"Early"`) for patches which must be in place before the game starts its own startup work; applied from the preloader bootstrap on both sides. |
| `LateCategory` | `const string` | Harmony patch category (`"Late"`) for patches deferred to `Init` because their target type lives in an assembly not loaded at the server's early bootstrap (VRage.EOS). |
| `HarmonyPatchAll(log, harmony, handleExceptions)` | `static bool` | Single-call entry: verifies all targeted methods via [`EnsureCode.cs`](../Tools/EnsureCode.cs.md), then `harmony.PatchAll`. Returns `false` on failure; throws if `handleExceptions` is `false` or the env var is set. Kept for callers which want everything in one phase. |
| `HarmonyPatchUncategorized(log, harmony, handleExceptions)` | `static bool` | Verifies and applies every patch *without* a category via `harmony.PatchAllUncategorized`. |
| `HarmonyPatchCategory(log, harmony, category, handleExceptions)` | `static bool` | Verifies and applies only the given category via `harmony.PatchCategory`. Called with `EarlyCategory` from the preloader bootstrap and with `LateCategory` from `Init`. |
| `ConfigureEarly()` | `static void` | Configures the patch classes in the `"Early"` category, before they are applied by the preloader bootstrap. Also called from `Configure()`. |
| `Configure()` | `static void` | Calls `Configure()` on every patch class that needs one-time post-config initialisation. Called once before patching. |
| `PatchUpdates()` | `static void` | Drives per-tick logic (cache expiry, debug reporting) by calling `Update()` on cache-based patch classes. Called every game simulation tick. |


## References

- [`EnsureCode.cs`](../Tools/EnsureCode.cs.md) — bytecode verification; `VerifyUncategorized` / `VerifyCategory` scope it to one phase.
- [`ServerPlugin/Plugin.cs`](../../ServerPlugin/Plugin.cs.md) — the server caller (`EarlyCategory` and the uncategorized patches early, `LateCategory` from `Init`).
- [`ServerPlugin/Preloader.cs`](../../ServerPlugin/Preloader.cs.md) — installs the early bootstrap that triggers those phases.
- [`ClientPlugin/Preloader.cs`](../../ClientPlugin/Preloader.cs.md) — the client counterpart, which triggers the `EarlyCategory` phase.
- [`ClientPlugin/Plugin.cs`](../../ClientPlugin/Plugin.cs.md) — the client caller: `EarlyCategory` from the bootstrap, then the uncategorized patches and `LateCategory` from `Init`.
- [merge-and-paste](../../../modules/merge-and-paste.md) — one of the modules whose `Configure()` / `Update()` this class drives.
- [keen-overhead-removal](../../../modules/keen-overhead-removal.md) — another module applied through this class.

---

*[Handbook](../../../TOC.md) · [Module: Patch Infrastructure](../../../modules/patch-infrastructure.md) · [Index](../../../Index.md)*
