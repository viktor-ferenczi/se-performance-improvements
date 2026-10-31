using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    // Runs programmable block scripts on any thread and applies the per-run time limits
    // (FixScriptCounter, ScriptRunLimitMs, ScriptSoftLimitMs). Needs the per-thread counter of
    // IlInjectorPrepatch, which the Preloader applies before the game loads; without it
    // everything here stays off.
    //
    // The hard limit, ScriptRunLimitMs, replaces the game's 3 s and stops a script in the middle
    // of a run. The soft limit, ScriptSoftLimitMs, is checked after a run: a script whose own
    // code (without the game's work before it, like the ownership walk) ran longer than that
    // SoftLimitRuns times in a row is stopped the same way. One slow run, say from a garbage
    // collection or a busy machine, does not stop a script; a script that is slow every time does.
    //
    // Execute runs a script the way MyProgrammableBlock.Run does on the server, minus the
    // DetailedInfo update, which raises network events and so belongs on the main thread
    // (MyProgrammableBlock.SetDetailedInfo). On a worker thread the caller must make sure
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

        // The smallest limit accepted, as the settings allow 0.1 ms steps
        public const float MinLimitMs = 0.1f;

        // Runs in a row over the soft limit that stop a script
        public const int SoftLimitRuns = 3;

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

        // Set while this thread runs a script for Execute on a worker thread
        [ThreadStatic]
        private static bool offMain;

        private static float appliedLimitMs = VanillaRunLimitMs;
        private static string vanillaText;

        // Time of the last script run on this thread, -1 while ExecuteCode has not run one
        [ThreadStatic]
        private static long scriptTicks;

        // Runs in a row over the soft limit, per PB
        private sealed class SlowRuns
        {
            public int Count;
        }

        private static readonly ConditionalWeakTable<MyProgrammableBlock, SlowRuns> SlowRunsOf =
            new ConditionalWeakTable<MyProgrammableBlock, SlowRuns>();

        private static readonly Action<
            MyProgrammableBlock,
            MyProgrammableBlock.ScriptTerminationReason
        > Terminate = AccessTools.MethodDelegate<
            Action<MyProgrammableBlock, MyProgrammableBlock.ScriptTerminationReason>
        >(AccessTools.Method(typeof(MyProgrammableBlock), "OnProgramTermination"));

        // Patches the run path of every script, so it is applied only while it is used
        private static readonly OptionalPatch SoftLimit = new OptionalPatch(
            typeof(SoftLimitPatch),
            () => Enabled
        );

        // The run limit in effect, in milliseconds
        public static float RunLimitMs => appliedLimitMs;

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
            SoftLimit.Update();

            if (!CounterPerThread || Common.Plugin.Tick % 60 != 0)
                return;

            var limitMs = Enabled
                ? Math.Max(MinLimitMs, Math.Min(VanillaRunLimitMs, Config.ScriptRunLimitMs))
                : VanillaRunLimitMs;
            if (limitMs != appliedLimitMs)
            {
                MaxAllowedTicks.SetValue(null, (long)(limitMs * Stopwatch.Frequency / 1000.0));
                appliedLimitMs = limitMs;
            }

            // The game formats the vanilla text with its 3 s constant, so a lower limit needs a text
            // of its own. A language reload brings the vanilla text back, which is kept for restoring.
            var current = MyTexts.GetString(MySpaceTexts.ProgrammableBlock_Exception_OutOfTime);
            if (limitMs < VanillaRunLimitMs)
            {
                var text =
                    $"Script execution terminated, script ran longer than this server's {Ms(limitMs)} ms limit per run. Spread the work over several runs.";
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

        // A limit for the player's eyes: 100, 2.5
        public static string Ms(float ms) => ms.ToString("0.#", CultureInfo.InvariantCulture);

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
                    "Running a programmable block on a worker thread needs FixScriptCounter"
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

        // RunSandboxedProgramAction reports a run on a worker thread as a mod rule violation.
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

        // Applied only while the limits are in effect, see OptionalPatch
        [HarmonyPatch]
        private static class SoftLimitPatch
        {
            private static bool Prepare() => SoftLimit.Prepare();

            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyProgrammableBlock), "ExecuteCode")]
            private static void ExecuteCodePrefix()
            {
                scriptTicks = -1;
            }

            // Times the script itself; also runs for the constructor, but the run of Main comes
            // after it and overwrites the time
            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyProgrammableBlock), "RunSandboxedProgramActionCore")]
            private static void RunCorePrefix(out long __state)
            {
                __state = Stopwatch.GetTimestamp();
            }

            [HarmonyFinalizer]
            [HarmonyPatch(typeof(MyProgrammableBlock), "RunSandboxedProgramActionCore")]
            private static void RunCoreFinalizer(long __state)
            {
                scriptTicks = Stopwatch.GetTimestamp() - __state;
            }

            // A run nested in another (TryRun) is judged here before the outer run ends, so the
            // outer run's time, which includes it, is the one left for the outer postfix
            [HarmonyPostfix]
            [HarmonyPatch(typeof(MyProgrammableBlock), "ExecuteCode")]
            private static void ExecuteCodePostfix(
                MyProgrammableBlock __instance,
                ref MyProgrammableBlock.ScriptTerminationReason __result,
                ref string response
            )
            {
                // Only a completed run counts; a failed one has already ended the script
                if (__result != MyProgrammableBlock.ScriptTerminationReason.None || scriptTicks < 0)
                    return;

                var limitMs = Math.Max(MinLimitMs, Config.ScriptSoftLimitMs);
                var slowRuns = SlowRunsOf.GetOrCreateValue(__instance);
                if (scriptTicks * 1000.0 <= limitMs * (double)Stopwatch.Frequency)
                {
                    slowRuns.Count = 0;
                    return;
                }

                if (++slowRuns.Count < SoftLimitRuns)
                    return;

                slowRuns.Count = 0;
                Terminate(
                    __instance,
                    MyProgrammableBlock.ScriptTerminationReason.InstructionOverflow
                );
                var text =
                    $"Script execution terminated, script ran longer than this server's {Ms(limitMs)} ms limit in {SoftLimitRuns} runs in a row. Spread the work over several runs.";
                response = string.IsNullOrEmpty(response) ? text : response + "\n" + text;
                __result = MyProgrammableBlock.ScriptTerminationReason.InstructionOverflow;
            }
        }
    }
}
