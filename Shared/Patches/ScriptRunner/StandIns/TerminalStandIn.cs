using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Gui;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage.Game.ModAPI.Ingame;

namespace Shared.Patches;

// Terminal properties and actions for scripts off the main thread (StandIn.Terminal).
//
// The lookups of MyTerminalBlock (GetProperty, GetActionWithName, ...) fill scratch lists of the
// MyTerminalControlFactoryHelper singleton; in the parallel phase every thread gets lists of its
// own. The control lists of a block type are made on first use and published before they are
// filled, so the lookups of the helper are serialized in the phase. A combobox fills one static list per block
// type for its minimum and maximum, so those are serialized too. The rest is reads: a property's
// GetValue calls its Getter, which reads the block. SetValue and Apply stay queued, and the
// extension methods over them (GetValueFloat, ApplyAction, ...) use only these members.
// WriteValue formats numbers with the game's shared formatter, see ScriptStandIns.FormatLock.
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class TerminalStandIn
{
    private static readonly object ControlsLock = new object();
    private static readonly object ComboBoxLock = new object();

    private static readonly AccessTools.FieldRef<
        MyTerminalControlFactoryHelper,
        List<Sandbox.Game.Gui.ITerminalAction>
    > ActionList = AccessTools.FieldRefAccess<
        MyTerminalControlFactoryHelper,
        List<Sandbox.Game.Gui.ITerminalAction>
    >("m_actionList");

    private static readonly AccessTools.FieldRef<
        MyTerminalControlFactoryHelper,
        List<ITerminalProperty>
    > ValueControls = AccessTools.FieldRefAccess<
        MyTerminalControlFactoryHelper,
        List<ITerminalProperty>
    >("m_valueControls");

    [ThreadStatic]
    private static List<Sandbox.Game.Gui.ITerminalAction> actions;

    [ThreadStatic]
    private static List<ITerminalProperty> properties;

    // In place of the helper's m_actionList
    public static List<Sandbox.Game.Gui.ITerminalAction> Actions(
        MyTerminalControlFactoryHelper helper
    ) =>
        ScriptStandIns.InPhase
            ? actions ??= new List<Sandbox.Game.Gui.ITerminalAction>()
            : ActionList(helper);

    // In place of the helper's m_valueControls
    public static List<ITerminalProperty> Properties(MyTerminalControlFactoryHelper helper) =>
        ScriptStandIns.InPhase
            ? properties ??= new List<ITerminalProperty>()
            : ValueControls(helper);

    [StandInFor("Sandbox.ModAPI.Interfaces.ITerminalAction.WriteValue")]
    public static void WriteValue(
        Sandbox.ModAPI.Interfaces.ITerminalAction action,
        IMyCubeBlock block,
        StringBuilder appendTo
    )
    {
        lock (ScriptStandIns.FormatLock)
            action.WriteValue(block, appendTo);
    }

    [HarmonyPatch]
    private static class Patch
    {
        private static bool Prepare() => ScriptRunner.CounterPerThread;

        // GetList itself is small enough for callers compiled before the patch to inline it, so
        // the lock goes around the lookups' entry points
        private static IEnumerable<MethodBase> TargetMethods() =>
            typeof(MyTerminalControlFactory)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m =>
                    !m.IsGenericMethodDefinition
                    && (
                        m.Name == nameof(MyTerminalControlFactory.GetValueControls)
                        || m.Name == nameof(MyTerminalControlFactory.GetActions)
                    )
                    && m.GetParameters().Length == 2
                );

        private static void Prefix(out bool __state) =>
            __state = ScriptStandIns.Enter(ControlsLock);

        private static void Finalizer(bool __state) => ScriptStandIns.Exit(ControlsLock, __state);
    }

    // Every lookup of the helper, with the scratch lists replaced
    [HarmonyPatch]
    private static class HelperPatch
    {
        private static bool Prepare() => ScriptRunner.CounterPerThread;

        private static IEnumerable<MethodBase> TargetMethods() =>
            ScriptStandIns.Loading(
                typeof(MyTerminalControlFactoryHelper),
                AccessTools.Field(typeof(MyTerminalControlFactoryHelper), "m_actionList"),
                AccessTools.Field(typeof(MyTerminalControlFactoryHelper), "m_valueControls")
            );

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code)
        {
            var actionList = AccessTools.Field(
                typeof(MyTerminalControlFactoryHelper),
                "m_actionList"
            );
            var valueControls = AccessTools.Field(
                typeof(MyTerminalControlFactoryHelper),
                "m_valueControls"
            );
            foreach (var instruction in code)
            {
                if (instruction.LoadsField(actionList))
                    yield return new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(typeof(TerminalStandIn), nameof(Actions))
                    ).MoveLabelsFrom(instruction);
                else if (instruction.LoadsField(valueControls))
                    yield return new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(typeof(TerminalStandIn), nameof(Properties))
                    ).MoveLabelsFrom(instruction);
                else
                    yield return instruction;
            }
        }
    }

    // The combobox methods are of a generic class; patching one reference type instantiation
    // patches the code all of them share
    [HarmonyPatch]
    private static class ComboBoxPatch
    {
        private static bool Prepare() => ScriptRunner.CounterPerThread;

        private static IEnumerable<MethodBase> TargetMethods()
        {
            var type = typeof(MyTerminalControlCombobox<MyTerminalBlock>);
            yield return AccessTools.Method(type, "GetMinimum", new[] { typeof(MyTerminalBlock) });
            yield return AccessTools.Method(type, "GetMaximum", new[] { typeof(MyTerminalBlock) });
        }

        private static void Prefix(out bool __state) =>
            __state = ScriptStandIns.Enter(ComboBoxLock);

        private static void Finalizer(bool __state) => ScriptStandIns.Exit(ComboBoxLock, __state);
    }
}
