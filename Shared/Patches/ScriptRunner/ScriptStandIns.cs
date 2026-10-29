using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Shared.Config;
using Shared.Plugin;

namespace Shared.Patches;

// The mechanisms of StandIn in effect, and what they share.
//
// Their patches (StandIns/*.cs) are applied at startup whenever the PB counter is per thread,
// whatever the options say, and act only during the parallel phase (InPhase). Applied later,
// a patched method could still run unpatched where a caller compiled earlier inlined it. The
// ScriptOffMain* options decide which scripts may leave the main thread: a script runs on a
// worker only while every mechanism it needs is on (ScriptRoots.State).
public static class ScriptStandIns
{
    private static IPluginConfig Config => Common.Config;

    public static readonly StandIn[] All = Enum.GetValues(typeof(StandIn))
        .Cast<StandIn>()
        .Where(s => s != StandIn.None)
        .ToArray();

    // The mechanisms in effect, set on the main thread between frames
    public static StandIn Active { get; private set; }

    // True during the parallel phase of ScriptScheduler, when scripts run on several threads,
    // the main thread included. The locks of the mechanisms are taken only then.
    public static volatile bool InPhase;

    // The logical grid group of the script running on this thread in the parallel phase
    [ThreadStatic]
    public static object CurrentGroup;

    // Serializes the game's number formatting (MyValueFormatter, StringBuilderExtensions_2),
    // which keeps its settings in one shared NumberFormatInfo
    public static readonly object FormatLock = new object();

    // Serializes the antenna system, which walks its relays with shared scratch sets
    public static readonly object AntennaLock = new object();

    // Takes a lock in the parallel phase only; returns whether it did, for Exit
    public static bool Enter(object lockObject)
    {
        if (!InPhase)
            return false;
        Monitor.Enter(lockObject);
        return true;
    }

    public static void Exit(object lockObject, bool entered)
    {
        if (entered)
            Monitor.Exit(lockObject);
    }

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

    // Whether a mechanism is in effect: only while scripts run in parallel
    public static bool Wanted(StandIn standIn) => ScriptScheduler.Parallel && IsOn(standIn);

    // Called on every update, on the main thread between frames
    public static void Update()
    {
        InventoryStandIn.Update();

        var active = StandIn.None;
        foreach (var standIn in All)
            if (Wanted(standIn))
                active |= standIn;
        Active = active;
    }

    // On the main thread right before the parallel phase
    public static void BeforePhase()
    {
        var active = Active;
        if ((active & StandIn.Gravity) != 0 && !GravityStandIn.BeforePhase())
            Active = active & ~StandIn.Gravity;
    }

    // The methods of a type that load any of the fields, for a transpiler replacing them
    public static IEnumerable<MethodBase> Loading(Type type, params FieldInfo[] fields) =>
        type.GetMethods(
                BindingFlags.Static
                    | BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.DeclaredOnly
            )
            .Where(m =>
                !m.IsGenericMethodDefinition
                && m.GetMethodBody() != null
                && PatchProcessor
                    .ReadMethodBody(m)
                    .Any(i => i.Value is FieldInfo f && fields.Contains(f))
            );

    // At startup: warns of the standin policies whose members lack a matching stand-in method,
    // which keep the scripts calling them on the main thread
    public static void Configure()
    {
        foreach (var member in ScriptPolicy.StandInMembers())
        {
            var at = member.LastIndexOf('.');
            var type = AccessTools.TypeByName(member.Substring(0, at));
            var name = member.Substring(at + 1);
            var methods = type
                ?.GetMethods()
                .Where(m => m.Name == name || m.Name == "get_" + name)
                .ToList();
            if (methods == null || methods.Count == 0)
                Common.Logger.Warning($"PB stand-in: {member} not found in the game");
            else
                foreach (var method in methods.Where(m => StandInMethods.Of(m) == null))
                    Common.Logger.Warning(
                        $"PB stand-in: no stand-in method for {method} of {member}"
                    );
        }
    }

    public static string Names(StandIn set) =>
        set == StandIn.None
            ? "none"
            : string.Join(", ", All.Where(s => (set & s) != 0).Select(s => s.ToString()));
}
