using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.World;
using SpaceEngineers.Game.EntityComponents.GameLogic;
using VRage.Plugins;
using VRage.Utils;

namespace SolarProbe;

// Measurement probe for the solar panel fix (FixSolarPanel) of the Performance plugin, on the
// dedicated server or an offline client. Every 10 seconds it writes one "SolarProbe:" line into
// the game log: whether the fix is applied at that moment, how many times the solar components
// computed an output, how many of those changed the output (which is what reaches the power
// network, or the gas output of oxygen farms and the farm plots' production), and how many times
// the power distributor recomputed its distribution and how long that took in total. Toggling the
// fix live in Quasar shows up in the next line.
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

    public static void Computed(bool outputChanged)
    {
        Interlocked.Increment(ref computed);
        if (outputChanged)
            Interlocked.Increment(ref changed);
    }

    public static void Recomputed(long ticks)
    {
        Interlocked.Increment(ref recomputes);
        Interlocked.Add(ref recomputeTicks, ticks);
    }

    // The fix is applied while the Performance plugin's transpilers are on the methods; the
    // plugin adds and removes them at runtime as the option changes
    private static bool FixApplied(MethodBase method)
    {
        var info = Harmony.GetPatchInfo(method);
        return info != null
            && info.Transpilers.Any(p =>
                p.PatchMethod.DeclaringType?.Name == "MySolarGameLogicComponentPatch"
            );
    }

    private static string FixState()
    {
        var legacy = FixApplied(LegacySunAngle);
        var current = FixApplied(NewSunAngle);
        if (legacy && current)
            return "on";
        if (!legacy && !current)
            return "off";
        return legacy ? "legacy-only" : "new-only";
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
            $"fix={FixState()}"
                + $" computed={Interlocked.Exchange(ref computed, 0)}"
                + $" changed={Interlocked.Exchange(ref changed, 0)}"
                + $" recomputes={Interlocked.Exchange(ref recomputes, 0)}"
                + $" recomputeMs={ms:0.00}"
        );
    }

    public static void Log(string message) =>
        MyLog.Default.WriteLineAndConsole($"SolarProbe: {message}");
}

// Compares the output before and after the computation, so it counts exactly the changes the fix
// filters, of every solar component: panels, oxygen farms and farm plots
[HarmonyPatch]
public static class SunAnglePatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return Probe.LegacySunAngle;
        yield return Probe.NewSunAngle;
    }

    public static void Prefix(object __instance, out float __state) =>
        __state = ((IMySolarPanelComponent)__instance).MaxOutput;

    public static void Postfix(object __instance, float __state) =>
        Probe.Computed(((IMySolarPanelComponent)__instance).MaxOutput != __state);
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
