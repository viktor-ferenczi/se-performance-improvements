using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using HarmonyLib;
using Sandbox;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.World;
using Sandbox.ModAPI.Ingame;
using VRage.FileSystem;
using VRage.Plugins;
using VRage.Utils;

namespace PbProbe;

// Measurement probe for SE-626, client (Pulsar) and dedicated server (Magnetar).
// Times every programmable block run once the world is ready and writes, into PB_PROBE_DIR:
//   runs.tsv    one line per ExecuteCode call: frame, PB, update type, total and script time, instructions
//   frames.tsv  one line per measured frame: session update time, PB runs and PB time in it
//   pbs.tsv     one line per PB: grid, block name, program hash and size, state at the end
//   compile.tsv one line per Compile call (world load included)
// "total" is ExecuteCode: the pre-run ownership writes plus the script. "script" is
// RunSandboxedProgramActionCore, the counted run alone. PB_PROBE_WARMUP frames are skipped,
// then PB_PROBE_FRAMES frames are measured; then DONE goes into probe.log.
public class Plugin : IPlugin
{
    public void Init(object gameInstance)
    {
        new Harmony("PbProbe").PatchAll(typeof(Plugin).Assembly);
        Probe.Log("loaded");
    }

    public void Update() { }

    public void Dispose() { }
}

public static class Probe
{
    private static readonly string Dir =
        Environment.GetEnvironmentVariable("PB_PROBE_DIR")
        ?? Path.Combine(MyFileSystem.UserDataPath, "pb-probe");
    private static readonly int Warmup = Env("PB_PROBE_WARMUP", 1200);
    private static readonly int Frames = Env("PB_PROBE_FRAMES", 3600);
    private static readonly double TickUs = 1e6 / Stopwatch.Frequency;

    private static readonly FieldInfo Instructions = AccessTools.Field(
        AccessTools.TypeByName("VRage.Library.Compiler.IlInjector"),
        "m_numInstructions"
    );
    private static readonly FieldInfo ProgramData = AccessTools.Field(
        typeof(MyProgrammableBlock),
        "m_programData"
    );
    private static readonly FieldInfo Instance = AccessTools.Field(
        typeof(MyProgrammableBlock),
        "m_instance"
    );
    private static readonly FieldInfo Termination = AccessTools.Field(
        typeof(MyProgrammableBlock),
        "m_terminationReason"
    );

    private static readonly StringBuilder RunLines = new();
    private static readonly StringBuilder FrameLines = new();
    private static readonly StringBuilder CompileLines = new();
    private static readonly HashSet<MyProgrammableBlock> Seen = new();

    private static int frame = -1;
    private static int frameRuns;
    private static long frameTicks;

    // Per thread: the Performance plugin runs scripts on worker threads too (SE-626 stage 4)
    [ThreadStatic]
    private static int depth;

    [ThreadStatic]
    private static long scriptStart;

    [ThreadStatic]
    private static long scriptTicks;
    private static bool done;

    public static bool Measuring => frame >= Warmup && !done;

    private static int Env(string name, int fallback) =>
        int.TryParse(Environment.GetEnvironmentVariable(name), out var v) ? v : fallback;

    public static void Log(string message)
    {
        Directory.CreateDirectory(Dir);
        File.AppendAllText(
            Path.Combine(Dir, "probe.log"),
            $"{DateTime.Now:HH:mm:ss.fff} {message}{Environment.NewLine}"
        );
        MyLog.Default.WriteLine("PbProbe: " + message);
    }

    public static void BeginFrame()
    {
        if (done || MySession.Static?.Ready != true)
            return;
        frame++;
        frameRuns = 0;
        frameTicks = 0;
        if (frame == 0)
            Log($"world ready, warmup {Warmup} frames, measuring {Frames}");
        if (RunnerTest.Expect != null)
        {
            // Test mode measures nothing: it runs the ScriptRunner checks once and ends
            if (frame == 300)
            {
                done = true;
                RunnerTest.Run();
            }
            return;
        }
        if (frame == Warmup)
            Log("window open");
    }

    public static void EndFrame(long sessionTicks)
    {
        if (!Measuring)
            return;
        FrameLines.Append(
            FormattableString.Invariant(
                $"{frame}\t{sessionTicks * TickUs:F1}\t{frameRuns}\t{frameTicks * TickUs:F1}\n"
            )
        );
        if (frame == Warmup + Frames - 1)
        {
            // An exception escaping into MySession.Update would stop the server
            try
            {
                Finish();
            }
            catch (Exception e)
            {
                Log($"FAILED to write the tables: {e}");
            }
        }
    }

    public static long BeginRun()
    {
        depth++;
        return Stopwatch.GetTimestamp();
    }

    public static void EndRun(
        MyProgrammableBlock pb,
        UpdateType type,
        long start,
        MyProgrammableBlock.ScriptTerminationReason result
    )
    {
        var ticks = Stopwatch.GetTimestamp() - start;
        depth--;
        if (!Measuring)
            return;
        var instructions = Instructions != null ? (int)Instructions.GetValue(null) : -1;
        var main = Thread.CurrentThread == MySandboxGame.Static.UpdateThread ? 1 : 0;
        lock (RunLines)
        {
            Seen.Add(pb);
            // A nested TryRun shares the caller's counters and is inside the caller's time.
            // With worker threads, the PB time of a frame is the sum over all threads.
            if (depth == 0)
            {
                frameRuns++;
                frameTicks += ticks;
            }
            RunLines.Append(
                FormattableString.Invariant(
                    $"{frame}\t{pb.EntityId}\t{(int)type}\t{depth}\t{ticks * TickUs:F1}\t{scriptTicks * TickUs:F1}\t{instructions}\t{result}\t{main}\n"
                )
            );
        }
    }

    public static void BeginScript() => scriptStart = Stopwatch.GetTimestamp();

    public static void EndScript() => scriptTicks = Stopwatch.GetTimestamp() - scriptStart;

    public static void Compiled(MyProgrammableBlock pb, long ticks)
    {
        var program = ProgramData?.GetValue(pb) as string ?? "";
        // World load compiles PBs on the parallel entity loading threads
        lock (CompileLines)
            CompileLines.Append(
                FormattableString.Invariant(
                    $"{pb.EntityId}\t{program.Length}\t{ticks * TickUs:F0}\n"
                )
            );
    }

    private static void Finish()
    {
        done = true;
        // The Compile prefix makes it too, but a small world can compile its PBs before this
        // plugin loads (seen with Magnetar 2.4.2)
        Directory.CreateDirectory(Path.Combine(Dir, "programs"));
        File.WriteAllText(
            Path.Combine(Dir, "runs.tsv"),
            "frame\tpb\tupdate_type\tdepth\ttotal_us\tscript_us\tinstructions\tresult\tmain\n" + RunLines
        );
        File.WriteAllText(
            Path.Combine(Dir, "frames.tsv"),
            "frame\tsession_us\tpb_runs\tpb_us\n" + FrameLines
        );
        File.WriteAllText(
            Path.Combine(Dir, "compile.tsv"),
            "pb\tprogram_chars\tcompile_us\n" + CompileLines
        );

        var pbs = new StringBuilder(
            "pb\tgrid\tname\tprogram_sha1\tprogram_chars\tenabled\tworking\tinstance\ttermination\tran\tinfo\tcustom_data\n"
        );
        foreach (
            var pb in MyEntities
                .GetEntities()
                .OfType<MyCubeGrid>()
                .SelectMany(g => g.GetFatBlocks<MyProgrammableBlock>())
        )
        {
            var program = ProgramData?.GetValue(pb) as string ?? "";
            var hash =
                program.Length == 0
                    ? ""
                    : BitConverter
                        .ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(program)))
                        .Replace("-", "")
                        .ToLowerInvariant();
            var info = pb
                .DetailedInfo.ToString()
                .Replace('\n', ' ')
                .Replace('\r', ' ')
                .Replace('\t', ' ');
            pbs.Append(
                $"{pb.EntityId}\t{Clean(pb.CubeGrid.DisplayName)}\t{Clean(pb.CustomName.ToString())}\t{hash}\t{program.Length}\t"
                    + $"{pb.Enabled}\t{pb.IsWorking}\t{Instance?.GetValue(pb) != null}\t{Termination?.GetValue(pb)}\t{Seen.Contains(pb)}\t"
                    + $"{(info.Length > 200 ? info.Substring(0, 200) : info)}\t{Clean(pb.CustomData)}\n"
            );
            if (program.Length > 0)
                File.WriteAllText(Path.Combine(Dir, "programs", hash + ".cs"), program);
        }
        File.WriteAllText(Path.Combine(Dir, "pbs.tsv"), pbs.ToString());
        Log("DONE");
    }

    private static string Clean(string text) => (text ?? "").Replace('\t', ' ').Replace('\n', ' ');

    internal static void EnsureProgramsDir() =>
        Directory.CreateDirectory(Path.Combine(Dir, "programs"));
}

[HarmonyPatch(typeof(MySession), nameof(MySession.Update))]
public static class SessionUpdatePatch
{
    public static void Prefix(out long __state)
    {
        Probe.BeginFrame();
        __state = Stopwatch.GetTimestamp();
    }

    public static void Postfix(long __state) => Probe.EndFrame(Stopwatch.GetTimestamp() - __state);
}

[HarmonyPatch(typeof(MyProgrammableBlock), "ExecuteCode")]
public static class ExecuteCodePatch
{
    public static void Prefix(out long __state) => __state = Probe.BeginRun();

    public static void Postfix(
        MyProgrammableBlock __instance,
        UpdateType updateSource,
        long __state,
        MyProgrammableBlock.ScriptTerminationReason __result
    ) => Probe.EndRun(__instance, updateSource, __state, __result);
}

[HarmonyPatch(typeof(MyProgrammableBlock), "RunSandboxedProgramActionCore")]
public static class ScriptPatch
{
    public static void Prefix() => Probe.BeginScript();

    public static void Postfix() => Probe.EndScript();
}

[HarmonyPatch(typeof(MyProgrammableBlock), "Compile")]
public static class CompilePatch
{
    public static void Prefix(out long __state)
    {
        Probe.EnsureProgramsDir();
        __state = Stopwatch.GetTimestamp();
    }

    public static void Postfix(MyProgrammableBlock __instance, long __state) =>
        Probe.Compiled(__instance, Stopwatch.GetTimestamp() - __state);
}
