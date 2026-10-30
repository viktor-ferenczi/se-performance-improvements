using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using VRage.Scripting.MemorySafeTypes;

namespace Shared.Patches;

// Number formatting of PB scripts on several threads (FixScriptCounter, SE-626).
//
// The StringBuilder extensions the game gives scripts (AppendDecimal, AppendFormatedDecimal)
// set the decimal digits in one static NumberFormatInfo and format with it right after. Two
// scripts formatting at the same time on two threads would get each other's digits. Each
// thread gets a NumberFormatInfo of its own instead. Applied at startup whenever the counter is
// per thread, before any script is compiled, so no script inlines the shared one.
[HarmonyPatch]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class ScriptFormatPatch
{
    [ThreadStatic]
    private static NumberFormatInfo numberFormat;

    public static NumberFormatInfo NumberFormat() =>
        numberFormat ??= (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();

    private static bool Prepare() => ScriptRunner.CounterPerThread;

    private static IEnumerable<MethodBase> TargetMethods() =>
        ScriptStandIns.Loading(
            typeof(MemorySafeStringBuilderExtensions),
            AccessTools.Field(typeof(MemorySafeStringBuilderExtensions), "m_numberFormatInfoHelper")
        );

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code)
    {
        var field = AccessTools.Field(
            typeof(MemorySafeStringBuilderExtensions),
            "m_numberFormatInfoHelper"
        );
        foreach (var instruction in code)
        {
            if (instruction.LoadsField(field))
                yield return new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(ScriptFormatPatch), nameof(NumberFormat))
                ).MoveLabelsFrom(instruction);
            else
                yield return instruction;
        }
    }
}
