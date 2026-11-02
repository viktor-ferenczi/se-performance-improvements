using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.World;
using SpaceEngineers.Game.Entities.Blocks;
using SpaceEngineers.Game.EntityComponents.GameLogic;
using VRage.Plugins;
using VRage.Utils;

namespace SolarProbe;

// Measurement probe for the solar panel fix (FixSolarPanel) of the Performance plugin, on the
// dedicated server or an offline client. Every 10 seconds it writes one "SolarProbe:" line into
// the game log: whether the fix is applied at that moment, how many times the solar components
// computed an output, how many of those reached the panels' production change handler (and so
// the power network), and how many times the power distributor recomputed its distribution and
// how long that took in total. Toggling the fix live in Quasar shows up in the next line.
public class Plugin : IPlugin
{
    public void Init(object gameInstance)
    {
        new Harmony("SolarProbe").PatchAll(typeof(Plugin).Assembly);
        Probe.Log("loaded");
    }

    public void Update() => Probe.Tick();

    public void Dispose() { }
}

public static class Probe
{
    private const int PeriodFrames = 600;

    public static readonly MethodBase LegacySunAngle = AccessTools.Method(
        typeof(MySolarGameLogicComponent),
        "OnSunAngleComputed"
    );
    public static readonly MethodBase NewSunAngle = AccessTools.Method(
        typeof(MyNewSolarGameLogicComponent),
        "OnSunAngleComputed"
    );

    private static int frames;
    private static int computed;
    private static int changed;
    private static int recomputes;
    private static long recomputeTicks;

    public static void Computed() => Interlocked.Increment(ref computed);

    public static void Changed() => Interlocked.Increment(ref changed);

    public static void Recomputed(long ticks)
    {
        Interlocked.Increment(ref recomputes);
        Interlocked.Add(ref recomputeTicks, ticks);
    }

    // The fix is applied while the Performance plugin's transpiler is on the method; the plugin
    // adds and removes it at runtime as the option changes
    private static bool FixApplied()
    {
        var info = Harmony.GetPatchInfo(NewSunAngle);
        return info != null
            && info.Transpilers.Any(p =>
                p.PatchMethod.DeclaringType?.Name == "MySolarGameLogicComponentPatch"
            );
    }

    public static void Tick()
    {
        if (MySession.Static == null)
        {
            frames = 0;
            return;
        }

        if (++frames < PeriodFrames)
            return;

        frames = 0;
        var ms = Interlocked.Exchange(ref recomputeTicks, 0) * 1000.0 / Stopwatch.Frequency;
        Log(
            $"fix={(FixApplied() ? "on" : "off")}"
                + $" computed={Interlocked.Exchange(ref computed, 0)}"
                + $" changed={Interlocked.Exchange(ref changed, 0)}"
                + $" recomputes={Interlocked.Exchange(ref recomputes, 0)}"
                + $" recomputeMs={ms:0.00}"
        );
    }

    public static void Log(string message) =>
        MyLog.Default.WriteLineAndConsole($"SolarProbe: {message}");
}

[HarmonyPatch]
public static class SunAnglePatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return Probe.LegacySunAngle;
        yield return Probe.NewSunAngle;
    }

    public static void Prefix() => Probe.Computed();
}

[HarmonyPatch(typeof(MySolarPanel), "OnProductionChanged")]
public static class ProductionChangedPatch
{
    public static void Prefix() => Probe.Changed();
}

[HarmonyPatch(
    typeof(MyResourceDistributorComponent),
    nameof(MyResourceDistributorComponent.RecomputeResourceDistribution)
)]
public static class RecomputePatch
{
    public static void Prefix(out long __state) => __state = Stopwatch.GetTimestamp();

    public static void Postfix(long __state) =>
        Probe.Recomputed(Stopwatch.GetTimestamp() - __state);
}
