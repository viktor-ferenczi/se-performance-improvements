using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.World;
using VRage;
using VRage.Game;
using VRage.ObjectBuilders;
using VRage.Plugins;
using VRage.Utils;
using VRageMath;

namespace AttachProbe;

// Regression probe for issue #161 on the dedicated server or an offline client. Once the world is
// ready it builds a static grid of stators without tops, then on every frame creates rotor tops
// whose ParentEntityId names one of those stators, with CreateFromObjectBuilderParallel, which
// runs their Init on worker threads as a client does for streamed grids. The tops are never
// added to the scene and are closed once made.
//
// It counts the changes of BlocksWaitingOnAttachUpdate made off the main thread on a grid in
// the scene (the race), and the exceptions thrown by the methods changing that set. Every 10
// seconds it writes one "AttachProbe:" line into the game log, and when the run is over a
// "AttachProbe: done" line with the totals and a consistency check of the stators' grid.
public class Plugin : IPlugin
{
    public void Init(object gameInstance)
    {
        new Harmony("AttachProbe").PatchAll(typeof(Plugin).Assembly);
        Probe.Log("loaded");
    }

    public void Update() => Probe.Tick();

    public void Dispose() { }
}

public static class Probe
{
    private const int PeriodFrames = 600;
    private static readonly int Stators = Setting("ATTACH_PROBE_STATORS", 64);
    private static readonly int TopsPerFrame = Setting("ATTACH_PROBE_TOPS", 16);
    private static readonly int RunFrames = Setting("ATTACH_PROBE_SECONDS", 60) * 60;

    private static int frames;
    private static int runFrames;
    private static bool done;
    private static MyCubeGrid baseGrid;
    private static MyMechanicalConnectionBlockBase[] stators;
    private static readonly Random Random = new Random(161);

    private static int spawned;
    private static int created;
    private static int offMain;
    private static int exceptions;
    private static int totalSpawned;
    private static int totalOffMain;
    private static int totalExceptions;

    private static int Setting(string name, int fallback) =>
        int.TryParse(Environment.GetEnvironmentVariable(name), out var value) ? value : fallback;

    public static void AttachStatusChanged(MyCubeGrid grid)
    {
        if (Thread.CurrentThread != MyUtils.MainThread && grid.InScene)
            Interlocked.Increment(ref offMain);
    }

    public static void Failed(string where, Exception exception)
    {
        if (Interlocked.Increment(ref exceptions) <= 5)
            Log(
                $"exception in {where} on thread {Thread.CurrentThread.ManagedThreadId}: {exception}"
            );
    }

    private static string FixState()
    {
        var init = AccessTools.DeclaredMethod(typeof(MyAttachableTopBlockBase), "Init");
        var info = Harmony.GetPatchInfo(init);
        return
            info != null
            && info.Transpilers.Any(p =>
                p.PatchMethod.DeclaringType?.Name == "MyAttachableTopBlockBasePatch"
            )
            ? "on"
            : "off";
    }

    public static void Tick()
    {
        if (done || MySession.Static == null || !MySession.Static.Ready)
            return;

        if (baseGrid == null)
        {
            BuildStators();
            Log($"fix={FixState()} stators={stators.Length} topsPerFrame={TopsPerFrame}");
            return;
        }

        if (runFrames < RunFrames)
        {
            for (var i = 0; i < TopsPerFrame; i++)
                SpawnTop();
        }

        runFrames++;
        if (++frames >= PeriodFrames)
        {
            frames = 0;
            Report();
        }

        // Lets the last tops finish their Init and the stators their reattach update
        if (runFrames == RunFrames + 120)
        {
            Report();
            Finish();
        }
    }

    private static void BuildStators()
    {
        var grid = new MyObjectBuilder_CubeGrid
        {
            GridSizeEnum = MyCubeSize.Large,
            IsStatic = true,
            PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
            PositionAndOrientation = new MyPositionAndOrientation(
                new Vector3D(5000, 5000, 5000),
                Vector3.Forward,
                Vector3.Up
            ),
        };
        for (var i = 0; i < Stators; i++)
        {
            grid.CubeBlocks.Add(
                new MyObjectBuilder_MotorStator
                {
                    SubtypeName = "LargeStator",
                    Min = new SerializableVector3I(3 * (i % 8), 0, 3 * (i / 8)),
                }
            );
        }

        baseGrid = (MyCubeGrid)MyEntities.CreateFromObjectBuilderAndAdd(grid, false);
        stators = baseGrid.GetFatBlocks<MyMechanicalConnectionBlockBase>().ToArray();
    }

    private static void SpawnTop()
    {
        var stator = stators[Random.Next(stators.Length)];
        var top = new MyObjectBuilder_CubeGrid
        {
            GridSizeEnum = MyCubeSize.Large,
            PersistentFlags = MyPersistentEntityFlags2.Enabled,
            PositionAndOrientation = new MyPositionAndOrientation(
                new Vector3D(-5000, -5000, -5000),
                Vector3.Forward,
                Vector3.Up
            ),
        };
        top.CubeBlocks.Add(
            new MyObjectBuilder_MotorRotor
            {
                SubtypeName = "LargeRotor",
                ParentEntityId = stator.EntityId,
            }
        );

        Interlocked.Increment(ref spawned);
        MyEntities.CreateFromObjectBuilderParallel(
            top,
            false,
            entity =>
            {
                Interlocked.Increment(ref created);
                entity?.Close();
            }
        );
    }

    private static void Report()
    {
        var s = Interlocked.Exchange(ref spawned, 0);
        var o = Interlocked.Exchange(ref offMain, 0);
        var e = Interlocked.Exchange(ref exceptions, 0);
        totalSpawned += s;
        totalOffMain += o;
        totalExceptions += e;
        Log($"fix={FixState()} spawned={s} created={created} offMain={o} exceptions={e}");
    }

    // The set must hold exactly the stators waiting on an attach update
    private static void Finish()
    {
        done = true;
        int inconsistent;
        try
        {
            var set = baseGrid.BlocksWaitingOnAttachUpdate;
            var expected = stators.Where(s => s.IsWaitingOnAttachUpdate).ToList();
            inconsistent =
                expected.Count(s => !set.Contains(s))
                + set.Count(s => s == null || !expected.Contains(s))
                + Math.Abs(set.Count - set.Distinct().Count());
        }
        catch (Exception exception)
        {
            Failed("consistency check", exception);
            inconsistent = -1;
        }

        Log(
            $"done fix={FixState()} spawned={totalSpawned} created={created}"
                + $" offMain={totalOffMain} exceptions={totalExceptions + exceptions}"
                + $" inconsistent={inconsistent}"
        );
    }

    public static void Log(string message) =>
        MyLog.Default.WriteLineAndConsole($"AttachProbe: {message}");
}

// Every change of the grid's BlocksWaitingOnAttachUpdate made through the grid's method, which
// is how MarkForReattach reaches it
[HarmonyPatch(typeof(MyCubeGrid), "MechanicalConnectionBlockAttachUpdateStatusChanged")]
public static class AttachStatusChangedPatch
{
    public static void Prefix(MyCubeGrid __instance) => Probe.AttachStatusChanged(__instance);

    // Counted and swallowed, so a torn set shows in the numbers instead of crashing the run
    public static Exception Finalizer(Exception __exception)
    {
        if (__exception != null)
            Probe.Failed("MechanicalConnectionBlockAttachUpdateStatusChanged", __exception);
        return null;
    }
}

// The main thread's side of the race: the stator's update removes itself from the set directly
[HarmonyPatch(typeof(MyMechanicalConnectionBlockBase), "UpdateAttachState")]
public static class UpdateAttachStatePatch
{
    public static Exception Finalizer(Exception __exception)
    {
        if (__exception != null)
            Probe.Failed("UpdateAttachState", __exception);
        return null;
    }
}
