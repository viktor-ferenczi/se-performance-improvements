using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Sandbox;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Localization;
using Sandbox.ModAPI.Ingame;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;
using VRage;
using VRage.Library.Compiler;
using VRage.Scripting;

namespace Shared.Patches
{
    // Runs programmable block scripts on any thread and applies the per-run time limit
    // (FixScriptCounter, ScriptRunLimitMs). Needs the per-thread counter of IlInjectorPrepatch,
    // which the Preloader applies before the game loads; without it everything here stays off.
    //
    // Execute runs a script the way MyProgrammableBlock.Run does on the server, minus the
    // DetailedInfo update, which raises network events and so belongs on the main thread
    // (MyProgrammableBlock.SetDetailedInfo). Off the main thread the caller must make sure
    // nothing else touches the block's grid group meanwhile: before the script starts, the game
    // writes the PB access flags of every block in the group.
    [HarmonyPatch]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class ScriptRunner
    {
        private static IPluginConfig Config => Common.Config;

        // The game's own limit and the text that goes with it
        public const int VanillaRunLimitMs = 3000;

        // True if IlInjectorPrepatch made the counter per thread in this process
        public static readonly bool CounterPerThread =
            typeof(IlInjector)
                .GetField("m_numInstructions", BindingFlags.NonPublic | BindingFlags.Static)
                ?.IsDefined(typeof(ThreadStaticAttribute), false) == true;

        private static readonly FieldInfo MaxAllowedTicks = typeof(IlInjector).GetField(
            "m_maxAllowedTicks",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        private static readonly MyLocalizationPackage Texts =
            typeof(MyTexts)
                .GetField("m_package", BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null) as MyLocalizationPackage;

        // Set while this thread runs a script for Execute off the main thread
        [ThreadStatic]
        private static bool offMain;

        private static int appliedLimitMs = VanillaRunLimitMs;
        private static string vanillaText;

        public static bool Enabled => CounterPerThread && Config.Enabled && Config.FixScriptCounter;

        public static void Configure()
        {
            if (CounterPerThread)
                Common.Plugin.Log.Info(
                    $"PB instruction counter per thread: {IlInjectorPrepatch.Result}"
                );
            else if (Config.FixScriptCounter && !CounterPerThread)
            {
                Common.Plugin.Log.Warning(
                    $"FixScriptCounter is on, but the instruction counter is not per thread ({IlInjectorPrepatch.Result}). It takes effect after a restart."
                );
            }
        }

        // Called on every update: applies a changed ScriptRunLimitMs, and the matching text again
        // after a language reload replaced it
        public static void Update()
        {
            if (!CounterPerThread || Common.Plugin.Tick % 60 != 0)
                return;

            var limitMs = Enabled
                ? Math.Max(1, Math.Min(VanillaRunLimitMs, Config.ScriptRunLimitMs))
                : VanillaRunLimitMs;
            if (limitMs != appliedLimitMs)
            {
                MaxAllowedTicks.SetValue(null, limitMs * Stopwatch.Frequency / 1000);
                appliedLimitMs = limitMs;
            }

            // The game formats the vanilla text with its 3 s constant, so a lower limit needs a text
            // of its own. A language reload brings the vanilla text back, which is kept for restoring.
            var current = MyTexts.GetString(MySpaceTexts.ProgrammableBlock_Exception_OutOfTime);
            if (limitMs < VanillaRunLimitMs)
            {
                var text =
                    $"Script execution terminated, script ran longer than this server's {limitMs} ms limit per run. Spread the work over several runs.";
                if (current != text)
                {
                    vanillaText = current;
                    SetText(text);
                }
            }
            else if (vanillaText != null)
            {
                SetText(vanillaText);
                vanillaText = null;
            }
        }

        private static void SetText(string text) =>
            Texts?.AddMessage(
                MySpaceTexts.ProgrammableBlock_Exception_OutOfTime.String,
                text,
                overwrite: true
            );

        // Runs the program on the calling thread with the vanilla checks and limits. Returns what
        // MyProgrammableBlock.ExecuteCode returns; pass the response to SetDetailedInfo on the main
        // thread afterwards, as MyProgrammableBlock.Run does.
        public static MyProgrammableBlock.ScriptTerminationReason Execute(
            MyProgrammableBlock pb,
            string argument,
            UpdateType updateSource,
            out string response
        )
        {
            if (Thread.CurrentThread == MySandboxGame.Static.UpdateThread)
                return pb.ExecuteCode(argument, updateSource, out response);

            if (!Enabled)
                throw new InvalidOperationException(
                    "Running a programmable block off the main thread needs FixScriptCounter"
                );

            offMain = true;
            try
            {
                return pb.ExecuteCode(argument, updateSource, out response);
            }
            finally
            {
                offMain = false;
            }
        }

        private static bool Prepare() => CounterPerThread;

        // RunSandboxedProgramAction reports a run off the main thread as a mod rule violation.
        // That report indexes the mod table with the thread's root mod id and throws when no mod
        // script was ever compiled, so runs started by Execute skip it.
        [HarmonyPrefix]
        [HarmonyPatch(
            typeof(MyVRageScriptingInternal),
            nameof(MyVRageScriptingInternal.ReportIncorrectBehaviour)
        )]
        [EnsureCode("8185b09b")]
        private static bool ReportIncorrectBehaviourPrefix(ref bool __result)
        {
            if (!offMain)
                return true;

            __result = false;
            return false;
        }
    }
}
