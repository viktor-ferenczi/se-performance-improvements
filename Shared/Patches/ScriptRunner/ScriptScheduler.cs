using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
    //
    // With the proxies on too (FixScriptProxies) and more than one thread, the scripts that got
    // proxies run in a parallel phase after the others: on the main thread and ScriptThreadCount - 1
    // workers, each with a budget of its own, while the main thread does nothing else. Two PBs of
    // one logical grid group never run at the same time, as a run writes the PB access flags of
    // the whole group, and the policy of every member a proxied script calls assumes that only
    // its own group changes meanwhile. The writes the proxies queued are made on the main thread
    // after the phase, PB by PB in EntityId order, so the result does not depend on which worker
    // ran what; then each PB gets its DetailedInfo, as MyProgrammableBlock.Run does.
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

        private static readonly AccessTools.FieldRef<
            MyProgrammableBlock,
            Sandbox.ModAPI.IMyGridProgram
        > InstanceOf = AccessTools.FieldRefAccess<
            MyProgrammableBlock,
            Sandbox.ModAPI.IMyGridProgram
        >("m_instance");

        private static readonly Action<MyProgrammableBlock, string> SetDetailedInfo =
            AccessTools.MethodDelegate<Action<MyProgrammableBlock, string>>(
                AccessTools.Method(typeof(MyProgrammableBlock), "SetDetailedInfo")
            );

        private static readonly RunQueue<MyProgrammableBlock>.Parallel ParallelRuns =
            new RunQueue<MyProgrammableBlock>.Parallel
            {
                Pool = new WorkerPool("PB worker"),
                GroupOf = GroupOf,
                Run = RunOffMain,
            };

        // The runs of the parallel phase, waiting for their writes and DetailedInfo
        private static readonly List<(
            MyProgrammableBlock Pb,
            ScriptWrites Writes,
            string Response
        )> Finished = new List<(MyProgrammableBlock, ScriptWrites, string)>();
        private static readonly Stack<ScriptWrites> SpareWrites = new Stack<ScriptWrites>();

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
        private static long parallelRuns;
        private static int failedWrites;

        public static bool Enabled => ScriptRunner.Enabled && Config.FixScriptScheduler;

        public static int Threads =>
            Config.ScriptThreadCountMode == ScriptThreadCountMode.Auto
                ? ScriptThreads.Auto
                : Math.Max(
                    ScriptThreads.Min,
                    Math.Min(ScriptThreads.Max, Config.ScriptThreadCount)
                );

        // Whether the scripts with proxies run in a parallel phase
        public static bool Parallel => Enabled && ScriptRoots.Enabled && Threads > 1;

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
            var parallel = Parallel ? ParallelRuns : null;
            if (parallel != null)
            {
                parallel.Threads = Threads;
                ScriptStandIns.BeforePhase();
                ScriptStandIns.InPhase = true;
            }
            try
            {
                result = Queue.Drain(usedTicks, BudgetTicks, RunBlock, Clock, parallel);
            }
            finally
            {
                ScriptStandIns.InPhase = false;
                draining = false;
                usedTicks = 0;
                ApplyFinished();
            }

            frames++;
            dueRuns += result.Due;
            ranRuns += result.Ran;
            frameTicks += result.UsedTicks;
            longestWait = Math.Max(longestWait, result.MaxWait);
            if (parallel != null)
                parallelRuns += Finished.Count;
            if (result.MaxWait > OverloadWaitFrames)
                overloadFrames++;

            if (frames < LogPeriodFrames)
                return;

            if (failedWrites != 0)
                Common.Logger.Warning(
                    $"PB scheduler: {failedWrites} writes of scripts run off the main thread failed in the last {frames} frames"
                );

            if (overloadFrames != 0)
            {
                Common.Logger.Warning(
                    $"PB scheduler over budget in {overloadFrames} of the last {frames} frames: "
                        + $"{(double)dueRuns / frames:0.0} runs due per frame, {(double)ranRuns / frames:0.0} ran ({100.0 * ranRuns / Math.Max(1, dueRuns):0}%), "
                        + $"longest wait {longestWait} frames, PB time {1000.0 * frameTicks / Stopwatch.Frequency / frames:0.00} ms per frame "
                        + $"(budget {ScriptRunner.Ms(Config.ScriptFrameBudgetMs)} ms per thread, {(Parallel ? Threads : 1)} threads, "
                        + $"{100.0 * parallelRuns / Math.Max(1, ranRuns):0}% of the runs in parallel, run limit {ScriptRunner.Ms(ScriptRunner.RunLimitMs)} ms)"
                );
            }

            frames = 0;
            overloadFrames = 0;
            dueRuns = 0;
            ranRuns = 0;
            frameTicks = 0;
            longestWait = 0;
            parallelRuns = 0;
            failedWrites = 0;
        }

        // On the main thread before the runs: the logical group of a PB whose script can run in
        // the parallel phase, or null to run it on the main thread. The checks of
        // MyProgrammableBlock.Run are made here, as the parallel phase calls ExecuteCode directly.
        private static object GroupOf(MyProgrammableBlock pb)
        {
            if (
                pb.MarkedForClose
                || pb.Closed
                || !pb.IsWorking
                || !pb.IsFunctional
                || pb.CubeGrid?.Physics == null
            )
                return null;

            var program = InstanceOf(pb);
            if (program == null)
                return null;
            var state = ScriptRoots.State(program);
            if (Config.ScriptLogThreadDecisions)
                LogDecision(pb, program, state);
            if (state != 0)
                return null;

            ScriptRoots.Prepare(program, pb);
            return MyCubeGridGroups.Static.Logical.GetGroup(pb.CubeGrid);
        }

        // The state last logged per PB, see ScriptRoots.State
        private sealed class Logged
        {
            public int State = int.MinValue;
        }

        private static readonly ConditionalWeakTable<MyProgrammableBlock, Logged> LoggedOf =
            new ConditionalWeakTable<MyProgrammableBlock, Logged>();

        // Logs where a script runs and why, when the PB is seen first and when that changes
        private static void LogDecision(
            MyProgrammableBlock pb,
            Sandbox.ModAPI.IMyGridProgram program,
            int state
        )
        {
            var logged = LoggedOf.GetOrCreateValue(pb);
            if (logged.State == state)
                return;
            logged.State = state;
            Common.Logger.Info(
                $"PB thread decision: \"{pb.CustomName}\" ({pb.EntityId}) on \"{pb.CubeGrid?.DisplayName}\": {ScriptRoots.Explain(program)}"
            );
        }

        // On any thread of the parallel phase
        private static void RunOffMain(MyProgrammableBlock pb, int updateType)
        {
            ScriptWrites writes;
            lock (Finished)
                writes = SpareWrites.Count != 0 ? SpareWrites.Pop() : new ScriptWrites();

            string response;
            writes.Begin();
            ScriptStandIns.CurrentGroup = MyCubeGridGroups.Static.Logical.GetGroup(pb.CubeGrid);
            try
            {
                ScriptRunner.Execute(pb, string.Empty, (UpdateType)updateType, out response);
            }
            finally
            {
                ScriptStandIns.CurrentGroup = null;
                ScriptWrites.End();
            }

            lock (Finished)
                Finished.Add((pb, writes, response));
        }

        // On the main thread after the parallel phase
        private static void ApplyFinished()
        {
            if (Finished.Count == 0)
                return;

            Finished.Sort((a, b) => a.Pb.EntityId.CompareTo(b.Pb.EntityId));
            foreach (var (pb, writes, response) in Finished)
            {
                writes.Replay(e =>
                {
                    failedWrites++;
                    Common.Logger.Debug(
                        $"PB {pb.EntityId}: write of a script run off the main thread failed: {e}"
                    );
                });
                SpareWrites.Push(writes);
                if (!pb.Closed)
                    SetDetailedInfo(pb, response);
            }
            Finished.Clear();
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
