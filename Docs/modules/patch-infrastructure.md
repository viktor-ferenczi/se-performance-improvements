# Patch Infrastructure

`PatchHelpers`: discovers every Harmony patch class, applies them in a controlled order, and verifies that the targeted game code still matches what the patches expect.

The patch infrastructure module contains exactly one file — [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) — which is the bootstrap layer for the entire plugin. It is responsible for two things: verifying that the targeted game methods still match what the patches expect (via [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md)), and then applying every `[HarmonyPatch]`-annotated class in the shared assembly in a single `harmony.PatchAll` call.

Every other patch module in the plugin depends on this module: `PatchHelpers.HarmonyPatchAll` must run (and succeed) before any game method is modified. In addition, `PatchHelpers.Configure()` and `PatchHelpers.PatchUpdates()` provide the ordered hooks through which cache-based patch classes initialise themselves and drive their per-tick update logic.

## Files

| File | Summary |
| --- | --- |
| [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) | Verifies game-method bytecodes via [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md), applies all Harmony patches, and provides `Configure()` / `PatchUpdates()` lifecycle hooks. |

## How it fits together

At plugin startup the host calls `PatchHelpers.Configure()` first, which fans out to every patch class that needs one-time configuration from the plugin config (cache sizes, algorithm choices, etc.). Then `PatchHelpers.HarmonyPatchAll(log, harmony)` is called: it runs [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md).`Verify()`, which compares the actual IL hashes of targeted methods against the recorded values embedded in `[EnsureCode("…")]` attributes. Any mismatch aborts patching cleanly.

If verification passes, `harmony.PatchAll(Assembly.GetExecutingAssembly())` applies all patch classes. From that point on, `PatchHelpers.PatchUpdates()` is called once per simulation tick by the plugin; it delegates to `Update()` on cache patches ([merge-and-paste](merge-and-paste.md) has none; modules like the safe-zone and conveyor-system patches use this to expire cached entries and emit periodic debug hit-rate logs).

The module has no shared state of its own — it is a pure coordinator.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
