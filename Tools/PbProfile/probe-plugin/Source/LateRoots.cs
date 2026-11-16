using System;
using System.Linq;
using HarmonyLib;

namespace PbProbe;

// SE-1588 test mode (PB_PROBE_LATE_ROOTS=1): makes every PB script instance before Performance's
// ScriptRoots patches are in place, as when the plugin can only patch from Init after the world
// has loaded. Once the world is ready it takes those patches off, recompiles every PB with a
// program (new instances with the game's own Runtime and Me), and puts the patches back. Runs
// after that set GridTerminalSystem with the patches on.
public static class LateRoots
{
    public static readonly bool Enabled =
        Environment.GetEnvironmentVariable("PB_PROBE_LATE_ROOTS") == "1";

    public static void Run()
    {
        var roots = AccessTools.TypeByName("Shared.Patches.ScriptRoots+RootsPatch");
        if (roots == null)
        {
            Probe.Log("late roots: Performance's ScriptRoots.RootsPatch not found");
            return;
        }

        var patched = Harmony
            .GetAllPatchedMethods()
            .SelectMany(original =>
            {
                var info = Harmony.GetPatchInfo(original);
                return info
                    .Prefixes.Concat(info.Postfixes)
                    .Concat(info.Finalizers)
                    .Where(p => p.PatchMethod.DeclaringType == roots)
                    .Select(p => (original, p));
            })
            .ToList();
        foreach (var (original, patch) in patched)
            new Harmony(patch.owner).Unpatch(original, patch.PatchMethod);

        var programs = Steps.Recompile();

        var owner = patched.Select(x => x.p.owner).FirstOrDefault() ?? "Performance";
        new Harmony(owner).CreateClassProcessor(roots).Patch();
        Probe.Log(
            $"late roots: took off {patched.Count} patches, recompiled {programs} PBs, patched again"
        );
    }
}
