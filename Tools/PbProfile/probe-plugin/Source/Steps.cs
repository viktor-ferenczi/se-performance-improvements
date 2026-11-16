using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;

namespace PbProbe;

// SE-1588 test mode: actions at given frames after the world is ready, from PB_PROBE_STEPS,
// for example "600:FixScriptProxies=false;900:recompile;1200:FixScriptProxies=true;1500:sighup".
//   Name=value  sets that Performance config option in the running server, as Quasar does
//   recompile   recompiles every PB with a program
//   sighup      sends SIGHUP to the server: Magnetar saves the world and reloads its config
public static class Steps
{
    private static readonly List<(int Frame, string Action)> Pending = (
        Environment.GetEnvironmentVariable("PB_PROBE_STEPS") ?? ""
    )
        .Split(';', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Split(':', 2))
        .Select(p => (int.Parse(p[0]), p[1].Trim()))
        .OrderBy(s => s.Item1)
        .ToList();

    public static void Frame(int frame)
    {
        while (Pending.Count != 0 && Pending[0].Frame <= frame)
        {
            var action = Pending[0].Action;
            Pending.RemoveAt(0);
            try
            {
                Probe.Log($"step at frame {frame}: {action}: {Do(action)}");
            }
            catch (Exception e)
            {
                Probe.Log($"step at frame {frame}: {action} FAILED: {e}");
            }
        }
    }

    private static string Do(string action)
    {
        if (action == "recompile")
            return $"recompiled {Recompile()} PBs";
        if (action == "sighup")
        {
            System
                .Diagnostics.Process.Start("kill", $"-HUP {Environment.ProcessId}")
                ?.WaitForExit();
            return "sent";
        }
        var (name, value) = (action.Split('=', 2)[0], action.Split('=', 2)[1]);
        // Other plugins built from the same template have a Shared.Plugin.Common too
        var (config, property) = AppDomain
            .CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("Shared.Plugin.Common"))
            .Where(t => t != null)
            .Select(t => AccessTools.Property(t, "Config")?.GetValue(null))
            .Where(c => c != null)
            .Select(c => (c, AccessTools.Property(c.GetType(), name)))
            .First(x => x.Item2 != null);
        property.SetValue(config, Convert.ChangeType(value, property.PropertyType));
        return $"{name} is {property.GetValue(config)}";
    }

    public static int Recompile()
    {
        var recompile = AccessTools.Method(typeof(MyProgrammableBlock), "Recompile");
        var programs = 0;
        foreach (
            var pb in MyEntities
                .GetEntities()
                .OfType<MyCubeGrid>()
                .SelectMany(g => g.GetFatBlocks<MyProgrammableBlock>())
                .ToList()
        )
        {
            if (string.IsNullOrEmpty(((Sandbox.ModAPI.IMyProgrammableBlock)pb).ProgramData))
                continue;
            recompile.Invoke(pb, new object[] { true });
            programs++;
        }
        return programs;
    }
}
