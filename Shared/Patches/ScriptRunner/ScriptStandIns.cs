using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.Config;
using Shared.Plugin;

namespace Shared.Patches;

public static class ScriptStandIns
{
    private static IPluginConfig Config => Common.Config;

    public static readonly StandIn[] All = Enum.GetValues(typeof(StandIn))
        .Cast<StandIn>()
        .Where(s => s != StandIn.None)
        .ToArray();

    // The mechanisms in effect, set on the main thread between frames
    public static StandIn Active { get; private set; }

    private static readonly Dictionary<Type, OptionalPatch> Patches = typeof(ScriptStandIns)
        .Assembly.GetTypes()
        .Select(t => (Type: t, Attribute: t.GetCustomAttribute<StandInPatchAttribute>()))
        .Where(x => x.Attribute != null)
        .ToDictionary(
            x => x.Type,
            x => new OptionalPatch(x.Type, () => Wanted(x.Attribute.StandIn))
        );

    public static bool Prepare(Type patchClass) => Patches[patchClass].Prepare();

    public static bool IsOn(StandIn standIn)
    {
        switch (standIn)
        {
            case StandIn.Sprites:
                return Config.ScriptOffMainSprites;
            case StandIn.Access:
                return Config.ScriptOffMainAccess;
            case StandIn.Terminal:
                return Config.ScriptOffMainTerminal;
            case StandIn.DetailedInfo:
                return Config.ScriptOffMainDetailedInfo;
            case StandIn.Igc:
                return Config.ScriptOffMainIgc;
            case StandIn.Gravity:
                return Config.ScriptOffMainGravity;
            case StandIn.Sensing:
                return Config.ScriptOffMainSensing;
            case StandIn.Transfers:
                return Config.ScriptOffMainTransfers;
            case StandIn.TryRun:
                return Config.ScriptOffMainTryRun;
            case StandIn.Inventory:
                return Config.ScriptOffMainInventory;
            default:
                return false;
        }
    }

    // Whether the patches of a mechanism are wanted: only while scripts run in parallel
    public static bool Wanted(StandIn standIn) => ScriptScheduler.Parallel && IsOn(standIn);

    // Called on every update, on the main thread between frames
    public static void Update()
    {
        foreach (var patch in Patches.Values)
            patch.Update();

        var active = StandIn.None;
        foreach (var standIn in All)
            if (Wanted(standIn))
                active |= standIn;
        Active = active;
    }

    public static string Names(StandIn set) =>
        set == StandIn.None
            ? "none"
            : string.Join(", ", All.Where(s => (set & s) != 0).Select(s => s.ToString()));
}
