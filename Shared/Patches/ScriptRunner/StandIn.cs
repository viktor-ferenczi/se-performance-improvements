using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shared.Patches;

// The mechanisms that make game members usable by scripts running off the main thread
// (SE-626). A policy table row like "pass@Terminal" means pass while that mechanism is on, and
// main otherwise. Each one can be switched off on its own (ScriptOffMain* options); a script
// runs on a worker only if every mechanism it needs is on.
[Flags]
public enum StandIn
{
    None = 0,

    // LCD sprites: the sprites of a DrawFrame are sent to the surface on the main thread
    Sprites = 1 << 0,

    // IMyGridTerminalSystem.CanAccess
    Access = 1 << 1,

    // Terminal property and action lookups, values and the extension methods over them
    Terminal = 1 << 2,

    // IMyTerminalBlock.DetailedInfo
    DetailedInfo = 1 << 3,

    // Intergrid communication
    Igc = 1 << 4,

    // Artificial and total gravity of ship controllers and parachutes
    Gravity = 1 << 5,

    // Turret, turret controller and sensor targets, camera ray casts, nearest player, hacking
    Sensing = 1 << 6,

    // Item transfers between inventories and conveyor reachability
    Transfers = 1 << 7,

    // IMyProgrammableBlock.TryRun
    TryRun = 1 << 8,

    // The inventory extension methods of TerminalBlockExtentions
    Inventory = 1 << 9,
}

// Marks a public static method that replaces an interface member for scripts running off the
// main thread: the proxy calls it with the game object and the arguments instead of the member.
// It must be public, as the proxies live in an assembly of their own.
[AttributeUsage(AttributeTargets.Method)]
public sealed class StandInForAttribute : Attribute
{
    public readonly string Member;

    public StandInForAttribute(string member)
    {
        Member = member;
    }
}

// Marks a Harmony patch class a mechanism needs; it is applied only while the mechanism is
// wanted (ScriptStandIns.Wanted). Its Prepare must return ScriptStandIns.Prepare(its type).
[AttributeUsage(AttributeTargets.Class)]
public sealed class StandInPatchAttribute : Attribute
{
    public readonly StandIn StandIn;

    public StandInPatchAttribute(StandIn standIn)
    {
        StandIn = standIn;
    }
}

// The stand-in methods by the member they replace
public static class StandInMethods
{
    private static Dictionary<string, MethodInfo> methods;

    // The stand-in method of an interface member, by its policy table name, or null
    public static MethodInfo Of(string member)
    {
        methods ??= typeof(StandInMethods)
            .Assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Select(m => (Method: m, For: m.GetCustomAttribute<StandInForAttribute>()))
            .Where(x => x.For != null)
            .ToDictionary(x => x.For.Member, x => x.Method);
        return methods.TryGetValue(member, out var method) ? method : null;
    }
}
