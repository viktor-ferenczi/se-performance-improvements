using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Shared.Patches;

// What a script may do with a game API member while it runs on a worker thread (SE-626).
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

    // On a worker thread the proxy calls a replacement made for workers instead, see
    // StandInForAttribute
    StandIn,
}

// The policy of each game member for scripts running on a worker thread, from the table in
// ScriptPolicyTable. A policy other than pass, queue and standin reads as main. A policy may
// name the mechanism it relies on after an @, like pass@Terminal: then a script calling the
// member runs on a worker thread only while that mechanism is on (StandIn).
public static class ScriptPolicy
{
    private static Dictionary<
        string,
        (MemberPolicy Get, MemberPolicy Set, StandIn GetNeeds, StandIn SetNeeds)
    > table = new Dictionary<string, (MemberPolicy, MemberPolicy, StandIn, StandIn)>();

    // Replaces the table, for the plugin at startup and for tests
    public static void Load(TextReader reader)
    {
        var loaded = new Dictionary<string, (MemberPolicy, MemberPolicy, StandIn, StandIn)>();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Length == 0 || line[0] == '#')
                continue;
            var columns = line.Split('\t');
            if (columns.Length < 3)
                continue;
            var (get, getNeeds) = Parse(columns[1]);
            var (set, setNeeds) = Parse(columns[2]);
            loaded[columns[0].Trim()] = (get, set, getNeeds, setNeeds);
        }
        table = loaded;
    }

    public static void LoadDefault() => Load(new StringReader(ScriptPolicyTable.Text));

    public static int Count => table.Count;

    // The members with a standin policy
    public static IEnumerable<string> StandInMembers() =>
        table
            .Where(row =>
                row.Value.Get == MemberPolicy.StandIn || row.Value.Set == MemberPolicy.StandIn
            )
            .Select(row => row.Key);

    private static readonly string[] AccessorPrefixes = { "get_", "set_", "add_", "remove_" };

    private static (MemberPolicy, StandIn) Parse(string text)
    {
        text = text.Trim();
        var needs = StandIn.None;
        var at = text.IndexOf('@');
        if (at >= 0)
        {
            if (!Enum.TryParse(text.Substring(at + 1), out needs))
                return (MemberPolicy.Main, StandIn.None);
            text = text.Substring(0, at);
        }
        switch (text)
        {
            case "pass":
                return (MemberPolicy.Pass, needs);
            case "queue":
                return (MemberPolicy.Queue, needs);
            case "standin":
                return (MemberPolicy.StandIn, needs);
            default:
                return (MemberPolicy.Main, StandIn.None);
        }
    }

    // The policy of a method, a property accessor or an event accessor
    public static MemberPolicy Of(MethodBase method) => Lookup(method).Policy;

    // The mechanism the policy of a member relies on
    public static StandIn NeedsOf(MethodBase method) => Lookup(method).Needs;

    private static (MemberPolicy Policy, StandIn Needs) Lookup(MethodBase method)
    {
        var type = method.DeclaringType;
        if (type == null)
            return (MemberPolicy.Main, StandIn.None);

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

        if (!table.TryGetValue(MemberName(type, name), out var policy))
            return (MemberPolicy.Main, StandIn.None);
        return setter ? (policy.Set, policy.SetNeeds) : (policy.Get, policy.GetNeeds);
    }

    public static string MemberName(Type type, string name) => $"{TypeName(type)}.{name}";

    // The table name of the member a method belongs to: get_X and set_X are both X
    public static string MemberName(MethodBase method)
    {
        var name = method.Name;
        if (method.IsSpecialName)
            foreach (var prefix in AccessorPrefixes)
                if (name.StartsWith(prefix))
                    return MemberName(method.DeclaringType, name.Substring(prefix.Length));
        return MemberName(method.DeclaringType, name);
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
