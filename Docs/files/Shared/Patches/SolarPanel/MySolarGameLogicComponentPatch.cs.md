# `Shared/Patches/SolarPanel/MySolarGameLogicComponentPatch.cs`

*Drops changes of a solar panel's `MaxOutput` smaller than a configurable threshold, so the grid's power network recomputes its distribution far less often.*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MySolarGameLogicComponentPatch.cs`](../../../../../Shared/Patches/SolarPanel/MySolarGameLogicComponentPatch.cs) (118 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

Every change of a solar panel's `MaxOutput` raises `OnProductionChanged`, which makes the whole power network of the grid recompute its distribution on the next frame. The panels compute their output on staggered frames, once per 100 frames each, and the sun angle drifts constantly, so with 100+ panels on a grid the network recomputes nearly every frame.

Two transpilers redirect the `MaxOutput` setter calls in `OnSunAngleComputed` of both solar components (the legacy `MySolarGameLogicComponent` and `MyNewSolarGameLogicComponent`) to static replacements that apply the value only when it differs from the last applied one by at least `Config.SolarPanelThresholdPercent` of the panel's maximum output (1% by default). Zero is always applied at once, so a panel turning on or going dark is reflected immediately; the zero assignments made from the 100th frame update when a panel stops working or gets occluded are not touched at all. The call sites are rewritten rather than the setter prefixed because the tiny setter is a JIT inlining candidate, and an inlined copy would bypass a prefix.

The patch is applied only while `Config.FixSolarPanel` is on, through `OptionalPatch.cs`. See the "Fewer solar panel output updates" section in `Docs/PerformanceFixes.md`.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `Optional` / `Prepare()` / `Update()` | `OptionalPatch` wiring | Applies the class at startup only if the option is on and applies or removes it when the option changes at runtime. |
| `LegacyOnSunAngleComputedTranspiler` / `NewOnSunAngleComputedTranspiler` | Transpilers | Redirect the `MaxOutput` setter calls of the respective component to the replacement below. |
| `RedirectMaxOutputSetter` | Static method | The shared IL rewrite: swaps the setter operand for the replacement, keeping the labels on the instruction; throws if no call was found. |
| `SetLegacyMaxOutput` / `SetNewMaxOutput` | Static methods | Replacement setters with `(instance, value)` parameters, so they consume the same stack as the instance call. |
| `IsWorthApplying` | Static method | The threshold rule: zero always passes, otherwise the change must reach the configured percentage. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MySolarGameLogicComponent.OnSunAngleComputed` | Transpiler | Routes the `MaxOutput` assignments through the threshold check. |
| `MyNewSolarGameLogicComponent.OnSunAngleComputed` | Transpiler | Routes the `MaxOutput` assignments through the threshold check. |

## References

- `OptionalPatch.cs` — applies the patch only while its option is on.
- [`EnsureCode.cs`](../../Tools/EnsureCode.cs.md) — verifies the IL of both target methods before patching.
- [`MyWindTurbinePatch.cs`](../WindTurbine/MyWindTurbinePatch.cs.md) — the neighbouring per-block optimization with the same option wiring.
- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
