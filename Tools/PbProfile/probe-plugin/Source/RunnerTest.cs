using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI.Ingame;

namespace PbProbe;

// Test of the Performance plugin's ScriptRunner (SE-626 stage 2), enabled by PB_PROBE_TEST.
// The world comes from runner_test.py: PBs named "PBTEST <kind>" carry the test programs. Once
// the world is ready this runs them on the main thread and on worker threads, while the main
// thread waits, and logs PASS or FAIL per check, then DONE.
public static class RunnerTest
{
    public static readonly string Expect = Environment.GetEnvironmentVariable("PB_PROBE_TEST");

    private static MethodInfo execute;
    private static bool enabled;

    public static void Run()
    {
        var runner = AppDomain
            .CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("Shared.Patches.ScriptRunner", false))
            .FirstOrDefault(t => t?.GetMethod("Execute") != null);
        if (runner == null)
        {
            Probe.Log("FAIL ScriptRunner not found (Performance not loaded?)");
            Probe.Log("DONE");
            return;
        }

        execute = runner.GetMethod("Execute");
        enabled = (bool)runner.GetProperty("Enabled").GetValue(null);
        Probe.Log(
            $"ScriptRunner.Enabled={enabled}, CounterPerThread={runner.GetField("CounterPerThread").GetValue(null)}"
        );

        var pbs = MyEntities
            .GetEntities()
            .OfType<MyCubeGrid>()
            .SelectMany(g => g.GetFatBlocks<MyProgrammableBlock>())
            .Where(pb => pb.CustomName.ToString().StartsWith("PBTEST "))
            .ToDictionary(pb => pb.CustomName.ToString().Substring(7));
        Probe.Log($"test PBs: {string.Join(", ", pbs.Keys.OrderBy(k => k))}");

        if (!enabled)
        {
            // On a worker thread the runner must refuse without the per-thread counter
            var refused = Worker(() => Exec(pbs["normal1"], out _));
            Check(
                "worker refused without FixScriptCounter",
                refused is InvalidOperationException,
                refused?.GetType().Name
            );
            Probe.Log("DONE");
            return;
        }

        var main = Exec(pbs["normal1"], out var mainText);
        Check(
            "normal on main thread",
            main == "None" && mainText.Contains("sum=49995000"),
            $"{main}: {mainText}"
        );

        string workerResult = null,
            workerText = null;
        Worker(() => workerResult = Exec(pbs["normal2"], out workerText));
        Check(
            "normal on a worker",
            workerResult == "None" && workerText == mainText,
            $"{workerResult}: {workerText}"
        );

        // Two workers at once, released together: the runaway script overflows while the long one
        // counts close to the limit, so a shared counter would stop the long one too
        string runaway = null,
            runawayText = null,
            beside = null,
            besideText = null;
        long runawayEnd = 0,
            besideStart = 0,
            besideEnd = 0;
        using (var start = new Barrier(2))
        {
            Task.WaitAll(
                Task.Run(() =>
                {
                    start.SignalAndWait();
                    runaway = Exec(pbs["runaway"], out runawayText);
                    runawayEnd = Stopwatch.GetTimestamp();
                }),
                Task.Run(() =>
                {
                    start.SignalAndWait();
                    besideStart = Stopwatch.GetTimestamp();
                    beside = Exec(pbs["long"], out besideText);
                    besideEnd = Stopwatch.GetTimestamp();
                })
            );
        }
        var overlapMs =
            (Math.Min(runawayEnd, besideEnd) - besideStart) * 1000.0 / Stopwatch.Frequency;
        Check(
            "runaway on a worker stopped",
            runaway == "InstructionOverflow",
            $"{runaway}: {Short(runawayText)}"
        );
        Check(
            "long script beside it unaffected",
            beside == "None"
                && besideText.Contains("sum=1012477500 instructions=45001")
                && overlapMs > 0,
            $"{beside}: {Short(besideText)}, overlap {overlapMs:F2} ms"
        );

        // "slow" sorts for about a second with few counted instructions; Expect says whether the
        // time limit stops it
        var slowMain = Exec(pbs["slow1"], out var slowMainText);
        string slowWorker = null,
            slowWorkerText = null;
        Worker(() => slowWorker = Exec(pbs["slow2"], out slowWorkerText));
        var killed = Expect == "limit";
        foreach (
            var (where, result, text) in new[]
            {
                ("main thread", slowMain, slowMainText),
                ("worker", slowWorker, slowWorkerText),
            }
        )
        {
            var ok = killed
                ? result == "InstructionOverflow" && text.Contains("ms limit per run")
                : result == "None" && text.Contains("sorted");
            Check(
                $"slow on {where} {(killed ? "stopped by the limit" : "within the limit")}",
                ok,
                $"{result}: {Short(text)}"
            );
        }

        Probe.Log("DONE");
    }

    private static string Exec(MyProgrammableBlock pb, out string response)
    {
        var args = new object[] { pb, "", UpdateType.Terminal, null };
        var result = execute.Invoke(null, args).ToString();
        response = (string)args[3] ?? "";
        return result;
    }

    // Runs on a worker thread while the main thread waits, as the parallel phase will
    private static Exception Worker(Action action)
    {
        try
        {
            Task.Run(action).Wait();
            return null;
        }
        catch (AggregateException e)
        {
            var inner = e.InnerException;
            return inner is TargetInvocationException t ? t.InnerException : inner;
        }
    }

    private static void Check(string name, bool ok, string detail) =>
        Probe.Log($"{(ok ? "PASS" : "FAIL")} {name}: {detail}");

    private static string Short(string text) =>
        (text ?? "").Replace('\n', ' ').Substring(0, Math.Min(160, (text ?? "").Length));
}
