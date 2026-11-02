using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;
using SpaceEngineers.Game.EntityComponents.GameLogic;

namespace Shared.Patches
{
    // Every change of a solar panel's MaxOutput raises OnProductionChanged, which makes the whole
    // power network of the grid recompute its distribution on the next frame. The panels update on
    // staggered frames and the sun angle drifts constantly, so with 100+ panels the network
    // recomputes nearly every frame. Dropping the tiny changes removes most of those recomputes.
    //
    // The setter calls in OnSunAngleComputed are redirected instead of prefixing the setter,
    // because the tiny setter is a JIT inlining candidate and an inlined copy would bypass a
    // prefix. The 100th frame update also sets MaxOutput to zero when the panel stops working or
    // gets occluded; those calls are left alone, so going dark is still reflected at once.
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch]
    public static class MySolarGameLogicComponentPatch
    {
        private static IPluginConfig Config => Common.Config;

        // Applied only while the option is on, see OptionalPatch
        private static readonly OptionalPatch Optional = new OptionalPatch(
            typeof(MySolarGameLogicComponentPatch),
            () => Config.Enabled && Config.FixSolarPanel
        );

        private static bool Prepare() => Optional.Prepare();

        public static void Update() => Optional.Update();

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MySolarGameLogicComponent), "OnSunAngleComputed")]
        [EnsureCode("f9113c1a")]
        private static IEnumerable<CodeInstruction> LegacyOnSunAngleComputedTranspiler(
            IEnumerable<CodeInstruction> instructions
        ) =>
            RedirectMaxOutputSetter(
                instructions,
                typeof(MySolarGameLogicComponent),
                nameof(SetLegacyMaxOutput)
            );

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MyNewSolarGameLogicComponent), "OnSunAngleComputed")]
        [EnsureCode("83540a46")]
        private static IEnumerable<CodeInstruction> NewOnSunAngleComputedTranspiler(
            IEnumerable<CodeInstruction> instructions
        ) =>
            RedirectMaxOutputSetter(
                instructions,
                typeof(MyNewSolarGameLogicComponent),
                nameof(SetNewMaxOutput)
            );

        private static IEnumerable<CodeInstruction> RedirectMaxOutputSetter(
            IEnumerable<CodeInstruction> instructions,
            Type component,
            string replacementName
        )
        {
            var setter = AccessTools.PropertySetter(component, "MaxOutput");
            var replacement = AccessTools.DeclaredMethod(
                typeof(MySolarGameLogicComponentPatch),
                replacementName
            );
            var replaced = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(setter))
                {
                    // Only the operand changes, so the labels on the instruction stay in place
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = replacement;
                    replaced++;
                }
                yield return instruction;
            }

            if (replaced == 0)
                throw new InvalidOperationException(
                    $"No MaxOutput setter call found in {component.Name}.OnSunAngleComputed"
                );
        }

        // Static with (instance, value) parameters, so they consume exactly the stack the
        // instance setter call did
        private static void SetLegacyMaxOutput(MySolarGameLogicComponent component, float value)
        {
            if (IsWorthApplying(component.MaxOutput, value))
                component.MaxOutput = value;
        }

        private static void SetNewMaxOutput(MyNewSolarGameLogicComponent component, float value)
        {
            if (IsWorthApplying(component.MaxOutput, value))
                component.MaxOutput = value;
        }

        // MaxOutput is the 0..1 ratio of the block's defined maximum output. Comparing against the
        // last applied value, not the last computed one, keeps the accumulated error below the
        // threshold.
        private static bool IsWorthApplying(float current, float value)
        {
            // Turning on or going dark must still be reflected at once
            if (current == 0f || value == 0f)
                return true;

            return Math.Abs(value - current) >= Config.SolarPanelThresholdPercent / 100f;
        }
    }
}
