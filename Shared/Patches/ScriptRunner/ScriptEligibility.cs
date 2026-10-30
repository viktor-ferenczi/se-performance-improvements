using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Shared.Patches;

// Decides from a compiled script's IL whether the script can run off the main thread (SE-626).
//
// Every method the script calls is looked up: one of the game's (not of the .NET libraries or
// of the script itself) must have the pass or queue policy, unless its type is plain data (a
// value type, VRageMath, the ini and command line parsers). A script calling anything else keeps
// running on the main thread. The scan reads the IL once per script assembly.
public static class ScriptEligibility
{
    public sealed class Result
    {
        public bool OffMain;

        // The members that keep the script on the main thread, at most MaxListed of them
        public string[] Blocking;
    }

    private const int MaxListed = 5;

    private static readonly ConditionalWeakTable<Assembly, Result> Results =
        new ConditionalWeakTable<Assembly, Result>();

    // Value types with methods that act on game objects; every other value type is plain data
    private static readonly HashSet<string> GameValueTypes = new HashSet<string>
    {
        "VRage.Game.GUI.TextPanel.MySpriteDrawFrame",
    };

    // Classes of plain data or parsers, used freely. MemorySafeTypes are the collections the
    // game's compiler puts in place of the .NET ones in scripts; they count memory through the
    // per-thread counter and size types through a ConcurrentDictionary.
    private static readonly string[] PlainNamespaces =
    {
        "VRageMath",
        "VRage.Game.ModAPI.Ingame.Utilities",
        "VRage.Scripting.MemorySafeTypes",
    };

    public static Result Of(Assembly assembly) => Results.GetValue(assembly, Scan);

    private static Result Scan(Assembly assembly)
    {
        var blocking = new SortedSet<string>();
        try
        {
            foreach (var type in assembly.GetTypes())
            foreach (
                var method in type.GetMethods(Everything)
                    .Cast<MethodBase>()
                    .Concat(type.GetConstructors(Everything))
            )
            {
                if (method.DeclaringType != type)
                    continue;
                foreach (var called in Calls(method))
                {
                    if (!Allowed(called, assembly))
                        blocking.Add(
                            $"{ScriptPolicy.TypeName(called.DeclaringType)}.{called.Name}"
                        );
                }
            }
        }
        catch (Exception e)
        {
            blocking.Add($"(unreadable: {e.GetType().Name})");
        }

        return new Result
        {
            OffMain = blocking.Count == 0,
            Blocking = blocking.Take(MaxListed).ToArray(),
        };
    }

    private const BindingFlags Everything =
        BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Instance
        | BindingFlags.Static
        | BindingFlags.DeclaredOnly;

    private static bool Allowed(MethodBase called, Assembly script)
    {
        var type = called.DeclaringType;
        if (type == null || type.Assembly == script || IsLibrary(type.Assembly))
            return true;

        var name = ScriptPolicy.TypeName(type);
        if (type.IsValueType && !GameValueTypes.Contains(name))
            return true;
        if (
            PlainNamespaces.Any(ns =>
                type.Namespace == ns || type.Namespace?.StartsWith(ns + ".") == true
            )
        )
            return true;
        // The script's own base class: Echo, Storage and the roots, which hand out proxies
        if (name == "Sandbox.ModAPI.Ingame.MyGridProgram")
            return true;
        // The instruction counting the game compiles into every script, per thread with
        // FixScriptCounter, which the parallel phase needs anyway
        if (name.StartsWith("VRage.Library.Compiler.IlInjector"))
            return true;

        var policy = called is MethodInfo method
            ? ScriptProxies.PolicyOf(method)
            : ScriptPolicy.Of(called);
        return policy == MemberPolicy.Pass || policy == MemberPolicy.Queue;
    }

    private static bool IsLibrary(Assembly assembly)
    {
        var name = assembly.GetName().Name;
        return name == "mscorlib"
            || name == "netstandard"
            || name == "System"
            || name.StartsWith("System.")
            || name.StartsWith("Microsoft.");
    }

    // The methods a method body refers to: calls, constructors and delegate targets
    public static IEnumerable<MethodBase> Calls(MethodBase method)
    {
        var body = method.GetMethodBody()?.GetILAsByteArray();
        if (body == null)
            yield break;

        var typeArgs =
            method.DeclaringType?.IsGenericType == true
                ? method.DeclaringType.GetGenericArguments()
                : null;
        var methodArgs = method.IsGenericMethod ? method.GetGenericArguments() : null;
        var module = method.Module;

        for (var i = 0; i < body.Length; )
        {
            OpCode op;
            if (body[i] == 0xFE)
            {
                op = TwoByte[body[i + 1]];
                i += 2;
            }
            else
            {
                op = OneByte[body[i]];
                i += 1;
            }

            if (op.OperandType == OperandType.InlineMethod)
            {
                // A call the resolver cannot bind throws, and the scan counts the script as blocked
                yield return module.ResolveMethod(
                    BitConverter.ToInt32(body, i),
                    typeArgs,
                    methodArgs
                );
            }
            i += OperandSize(op, body, i);
        }
    }

    private static int OperandSize(OpCode op, byte[] body, int at)
    {
        switch (op.OperandType)
        {
            case OperandType.InlineNone:
                return 0;
            case OperandType.ShortInlineBrTarget:
            case OperandType.ShortInlineI:
            case OperandType.ShortInlineVar:
                return 1;
            case OperandType.InlineVar:
                return 2;
            case OperandType.InlineI8:
            case OperandType.InlineR:
                return 8;
            case OperandType.InlineSwitch:
                return 4 + 4 * BitConverter.ToInt32(body, at);
            default:
                return 4;
        }
    }

    private static readonly OpCode[] OneByte = new OpCode[256];
    private static readonly OpCode[] TwoByte = new OpCode[256];

    static ScriptEligibility()
    {
        foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var op = (OpCode)field.GetValue(null);
            var value = (ushort)op.Value;
            if (op.Size == 1)
                OneByte[value] = op;
            else
                TwoByte[value & 0xFF] = op;
        }
    }
}
