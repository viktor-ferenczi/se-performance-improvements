# Patch Infrastructure

`PatchHelpers`: discovers every Harmony patch class, applies them in a controlled order, and verifies that the targeted game code still matches what the patches expect.

The patch infrastructure module contains exactly one file — [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) — which is the bootstrap layer for the entire plugin. It is responsible for two things: verifying that the targeted game methods still match what the patches expect (via [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md)), and then applying the `[HarmonyPatch]`-annotated classes in the shared assembly. The client applies them all in one `harmony.PatchAll` call; the dedicated server applies them in two phases (uncategorized patches early, before world-load compilation, then the deferred `"Late"` category from `Init`).

Every other patch module in the plugin depends on this module: the patch application (`HarmonyPatchAll` on the client, `HarmonyPatchUncategorized` + `HarmonyPatchCategory` on the server) must run (and succeed) before any game method is modified. In addition, `PatchHelpers.Configure()` and `PatchHelpers.PatchUpdates()` provide the ordered hooks through which cache-based patch classes initialise themselves and drive their per-tick update logic.

## Files

| File | Summary |
| --- | --- |
| [`PatchHelpers.cs`](../files/Shared/Patches/PatchHelpers.cs.md) | Verifies game-method bytecodes via [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md), applies the Harmony patches (all at once on the client, in two phases on the server), and provides `Configure()` / `PatchUpdates()` lifecycle hooks. |

## How it fits together

At plugin startup the host calls `PatchHelpers.Configure()` first (from `Common.SetPlugin`), which fans out to every patch class that needs one-time configuration from the plugin config (cache sizes, algorithm choices, etc.). Then the patches are applied through one of three entry points, all built on the shared private `VerifyAndApply` scaffold: it runs the matching [`EnsureCode.cs`](../files/Shared/Tools/EnsureCode.cs.md) scan (`Verify`, `VerifyUncategorized`, or `VerifyCategory`), which compares the actual IL hashes of targeted methods against the recorded values embedded in `[EnsureCode("…")]` attributes. Any mismatch aborts that phase cleanly.

If verification passes, the patches are applied — `harmony.PatchAll` (client), `harmony.PatchAllUncategorized` (server early phase), or `harmony.PatchCategory` (server `"Late"` phase) — and `VerifyAndApply` logs each game method it patched (computed as a before/after `harmony.GetPatchedMethods()` delta) so a test run can be checked against the log. From that point on, `PatchHelpers.PatchUpdates()` is called once per simulation tick by the plugin; it delegates to `Update()` on cache patches ([merge-and-paste](merge-and-paste.md) has none; modules like the safe-zone and conveyor-system patches use this to expire cached entries and emit periodic debug hit-rate logs).

The module has no shared state of its own — it is a pure coordinator.

---

*[Handbook TOC](../TOC.md) · [Index](../Index.md)*
