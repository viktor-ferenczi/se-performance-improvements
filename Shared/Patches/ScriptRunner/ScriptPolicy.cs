using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Shared.Patches;

// What a script may do with a game API member while it runs off the main thread (SE-626).
public enum MemberPolicy
{
    // Needs the main thread; a script using it keeps running there
    Main,

    // Safe to call on a worker: reads game state, or writes only caches of the objects of the
    // script's own grid group, which no other worker touches meanwhile (a block's world matrix)
    Pass,

    // A write without a result, or with a bool result that is true (WriteText): recorded on the
    // worker, applied on the main thread afterwards
    Queue,
}

// The policy of each game member for scripts running off the main thread, from the table in
// ScriptPolicyTable. A policy other than pass and queue reads as main.
public static class ScriptPolicy
{
    private static Dictionary<string, (MemberPolicy Get, MemberPolicy Set)> table =
        new Dictionary<string, (MemberPolicy, MemberPolicy)>();

    // Replaces the table, for the plugin at startup and for tests
    public static void Load(TextReader reader)
    {
        var loaded = new Dictionary<string, (MemberPolicy, MemberPolicy)>();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Length == 0 || line[0] == '#')
                continue;
            var columns = line.Split('\t');
            if (columns.Length < 3)
                continue;
            loaded[columns[0].Trim()] = (Parse(columns[1]), Parse(columns[2]));
        }
        table = loaded;
    }

    public static void LoadDefault() => Load(new StringReader(ScriptPolicyTable.Text));

    public static int Count => table.Count;

    private static readonly string[] AccessorPrefixes = { "get_", "set_", "add_", "remove_" };

    private static MemberPolicy Parse(string text)
    {
        switch (text.Trim())
        {
            case "pass":
                return MemberPolicy.Pass;
            case "queue":
                return MemberPolicy.Queue;
            default:
                return MemberPolicy.Main;
        }
    }

    // The policy of a method, a property accessor or an event accessor
    public static MemberPolicy Of(MethodBase method)
    {
        var type = method.DeclaringType;
        if (type == null)
            return MemberPolicy.Main;

        var name = method.Name;
        var setter = false;
        if (method.IsSpecialName)
        {
            foreach (var prefix in AccessorPrefixes)
            {
                if (!name.StartsWith(prefix))
                    continue;
                setter = prefix == "set_";
                name = name.Substring(prefix.Length);
                break;
            }
        }

        if (!table.TryGetValue($"{TypeName(type)}.{name}", out var policy))
            return MemberPolicy.Main;
        return setter ? policy.Set : policy.Get;
    }

    // Namespace and name, without generic arity or arguments, nested types joined with a dot
    public static string TypeName(Type type)
    {
        if (type.IsGenericType && !type.IsGenericTypeDefinition)
            type = type.GetGenericTypeDefinition();
        var name = type.Name;
        var tick = name.IndexOf('`');
        if (tick >= 0)
            name = name.Substring(0, tick);
        if (type.DeclaringType != null)
            return $"{TypeName(type.DeclaringType)}.{name}";
        return string.IsNullOrEmpty(type.Namespace) ? name : $"{type.Namespace}.{name}";
    }
}
