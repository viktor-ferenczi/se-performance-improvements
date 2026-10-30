using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using HarmonyLib;
using Sandbox;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.Multiplayer;
using Sandbox.ModAPI.Ingame;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;

namespace Shared.Patches
{
    // Holds the programmable block runs of a frame to a time budget (FixScriptScheduler).
    //
    // The game runs a script with an update frequency from MyIngameScriptComponent's
    // UpdateAfterSimulation, as many as are due, without any limit on their total time. With the
    // scheduler on, those runs are queued instead, and once all entities are updated, the queue
    // runs on the main thread until the frame's PB time reaches ScriptFrameBudgetMs. The runs left
    // over wait for the next frame; a PB due again meanwhile still runs once, with the update
    // types of all its due runs, as the game merges them too. Runs from timers, buttons, IGC
    // callbacks, TryRun, mods and players are not delayed (TryRun returns a result), but their
    // time counts against the budget. See RunQueue for the order and how a run is fitted.
    //
    // A run is still limited by ScriptRunLimitMs, which is not lowered under load: a script
    // stopped by the limit stays stopped until it is recompiled. When the budget cannot keep up,
    // scripts run less often instead, and once a minute the log says by how much.
    //
    // It needs the per-thread counter (FixScriptCounter), which makes the run limit settable.
    // This version runs the queue on the main thread only; ScriptThreadCount takes effect once
    // scripts can run off the main thread.
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public static class ScriptScheduler
    {
        private static IPluginConfig Config => Common.Config;

        // A PB passed over more often than this in a row means the budget does not keep up
        public const int OverloadWaitFrames = 10;
        private const int LogPeriodFrames = 3600;

        private static readonly RunQueue<MyProgrammableBlock> Queue =
            new RunQueue<MyProgrammableBlock>(new Random());

        private static readonly Action<MyProgrammableBlock, int> RunBlock = (pb, updateType) =>
        {
            if (!pb.MarkedForClose && !pb.Closed)
                pb.Run(string.Empty, (UpdateType)updateType);
        };

        private static readonly Func<long> Clock = Stopwatch.GetTimestamp;

        private static readonly OptionalPatch Patch = new OptionalPatch(
            typeof(SchedulerPatch),
            () => Enabled
        );

        // Time of the runs outside the queue since the last drain
        private static long usedTicks;
        private static bool draining;
        private static bool measuring;

        // Totals since the last log line
        private static int frames;
        private static int overloadFrames;
        private static long dueRuns;
        private static long ranRuns;
        private static long frameTicks;
        private static int longestWait;

        public static bool Enabled => ScriptRunner.Enabled && Config.FixScriptScheduler;

        private static long BudgetTicks =>
            (long)(
                Math.Max(ScriptRunner.MinLimitMs, Config.ScriptFrameBudgetMs)
                * Stopwatch.Frequency
                / 1000.0
            );

        // Called on every update, on the main thread between frames
        public static void Update()
        {
            Patch.Update();

            // Turned off with runs still waiting: run them now, once
            if (!Enabled && Queue.Count != 0)
                Queue.Drain(0, long.MaxValue / 2, RunBlock, Clock);
        }

        private static void Drain()
        {
            draining = true;
            RunQueue<MyProgrammableBlock>.DrainResult result;
            try
            {
                result = Queue.Drain(usedTicks, BudgetTicks, RunBlock, Clock);
            }
            finally
            {
                draining = false;
                usedTicks = 0;
            }

            frames++;
            dueRuns += result.Due;
            ranRuns += result.Ran;
            frameTicks += result.UsedTicks;
            longestWait = Math.Max(longestWait, result.MaxWait);
            if (result.MaxWait > OverloadWaitFrames)
                overloadFrames++;

            if (frames < LogPeriodFrames)
                return;

            if (overloadFrames != 0)
            {
                Common.Logger.Warning(
                    $"PB scheduler over budget in {overloadFrames} of the last {frames} frames: "
                        + $"{(double)dueRuns / frames:0.0} runs due per frame, {(double)ranRuns / frames:0.0} ran ({100.0 * ranRuns / Math.Max(1, dueRuns):0}%), "
                        + $"longest wait {longestWait} frames, PB time {1000.0 * frameTicks / Stopwatch.Frequency / frames:0.00} ms per frame "
                        + $"(budget {ScriptRunner.Ms(Config.ScriptFrameBudgetMs)} ms, run limit {ScriptRunner.Ms(ScriptRunner.RunLimitMs)} ms)"
                );
            }

            frames = 0;
            overloadFrames = 0;
            dueRuns = 0;
            ranRuns = 0;
            frameTicks = 0;
            longestWait = 0;
        }

        // Applied only while the scheduler is on, see OptionalPatch
        [HarmonyPatch]
        private static class SchedulerPatch
        {
            private static bool Prepare() => Patch.Prepare();

            // Queues the due run instead of running it. Clients ask the server to run, as before.
            [HarmonyPrefix]
            [HarmonyPatch(
                typeof(MyIngameScriptComponent),
                nameof(MyIngameScriptComponent.UpdateAfterSimulation)
            )]
            [EnsureCode("b181ee20")]
            private static bool UpdateAfterSimulationPrefix(
                MyProgrammableBlock ___m_block,
                UpdateType ___m_updateType
            )
            {
                if (!Sync.IsServer)
                    return true;

                if (___m_updateType != UpdateType.None)
                    Queue.Add(___m_block, (int)___m_updateType);
                return false;
            }

            // Every entity has had its after simulation update, the queue is complete
            [HarmonyPostfix]
            [HarmonyPatch(
                typeof(MyParallelEntityUpdateOrchestrator),
                nameof(MyParallelEntityUpdateOrchestrator.DispatchAfterSimulation)
            )]
            private static void DispatchAfterSimulationPostfix()
            {
                Drain();
            }

            // Measures the runs outside the queue. A run nested in another (TryRun) is part of
            // the outer run's time; the queue measures its own runs.
            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyProgrammableBlock), "ExecuteCode")]
            private static void ExecuteCodePrefix(out long __state)
            {
                __state = 0;
                if (
                    draining
                    || measuring
                    || Thread.CurrentThread != MySandboxGame.Static.UpdateThread
                )
                    return;

                measuring = true;
                __state = Stopwatch.GetTimestamp();
            }

            [HarmonyFinalizer]
            [HarmonyPatch(typeof(MyProgrammableBlock), "ExecuteCode")]
            private static void ExecuteCodeFinalizer(long __state)
            {
                if (__state == 0)
                    return;

                usedTicks += Stopwatch.GetTimestamp() - __state;
                measuring = false;
            }
        }
    }
}
