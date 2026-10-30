using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using VRage.Scripting;

namespace PbProbe;

// With PB_PROBE_WHITELIST=<file>, writes every member of the game assemblies that the game's own
// whitelist (MyScriptWhitelist.IsMemberWhitelisted) allows for PB scripts, one line each:
//   type_kind  type  member  member_kind  static  get  set
// The type is namespace and name without generic arity, nested types joined with a dot, as in
// the Performance plugin's script policy table. Roslyn is reached by reflection, so the plugin
// compiles without a reference to it.
public static class WhitelistDump
{
    public static readonly string Target = Environment.GetEnvironmentVariable("PB_PROBE_WHITELIST");

    private static Assembly roslyn;

    public static void Run()
    {
        var whitelist = MyScriptCompiler.Static.Whitelist;
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var compilation = typeof(MyScriptWhitelist)
            .GetMethod("CreateCompilation", flags)
            .Invoke(whitelist, null);
        var isWhitelisted = typeof(MyScriptWhitelist).GetMethod("IsMemberWhitelisted", flags);
        roslyn = compilation.GetType().BaseType.Assembly;

        var lines = new SortedSet<string>(StringComparer.Ordinal);
        var getAssembly = compilation.GetType().GetMethod("GetAssemblyOrModuleSymbol");
        foreach (
            var reference in (IEnumerable)Get(
                compilation,
                "References",
                "Microsoft.CodeAnalysis.Compilation"
            )
        )
        {
            var assembly = getAssembly.Invoke(compilation, new[] { reference });
            if (assembly == null)
                continue;
            var name = (string)Get(assembly, "Name", "Microsoft.CodeAnalysis.ISymbol");
            if (
                name == "mscorlib"
                || name == "netstandard"
                || name == "System"
                || name.StartsWith("System.")
                || name.StartsWith("Microsoft.")
            )
                continue;
            var global = Get(assembly, "GlobalNamespace", "Microsoft.CodeAnalysis.IAssemblySymbol");
            foreach (var type in Types(global))
            foreach (
                var member in (IEnumerable)Call(
                    type,
                    "GetMembers",
                    "Microsoft.CodeAnalysis.INamespaceOrTypeSymbol"
                )
            )
            {
                var kind = Get(member, "Kind", "Microsoft.CodeAnalysis.ISymbol").ToString();
                if (kind != "Method" && kind != "Property" && kind != "Event" && kind != "Field")
                    continue;
                if (
                    Get(member, "DeclaredAccessibility", "Microsoft.CodeAnalysis.ISymbol")
                        .ToString() != "Public"
                )
                    continue;
                if (
                    kind == "Method"
                    && Get(member, "MethodKind", "Microsoft.CodeAnalysis.IMethodSymbol").ToString()
                        is var mk
                    && mk != "Ordinary"
                    && mk != "Constructor"
                    && mk != "UserDefinedOperator"
                    && mk != "Conversion"
                )
                    continue;
                if (
                    !(bool)
                        isWhitelisted.Invoke(
                            whitelist,
                            new[] { member, (object)MyWhitelistTarget.Ingame }
                        )
                )
                    continue;
                var get =
                    kind == "Property"
                    && Get(member, "GetMethod", "Microsoft.CodeAnalysis.IPropertySymbol") != null;
                var set =
                    kind == "Property"
                    && Get(member, "SetMethod", "Microsoft.CodeAnalysis.IPropertySymbol") != null;
                lines.Add(
                    $"{Get(type, "TypeKind", "Microsoft.CodeAnalysis.ITypeSymbol")}\t{TypeName(type)}\t"
                        + $"{Get(member, "Name", "Microsoft.CodeAnalysis.ISymbol")}\t{kind}\t"
                        + $"{Get(member, "IsStatic", "Microsoft.CodeAnalysis.ISymbol")}\t{get}\t{set}"
                );
            }
        }
        File.WriteAllText(
            Target,
            "type_kind\ttype\tmember\tmember_kind\tstatic\tget\tset\n"
                + string.Join("\n", lines)
                + "\n"
        );
        Probe.Log($"whitelist: {lines.Count} members written to {Target}");
    }

    private static IEnumerable<object> Types(object ns)
    {
        foreach (
            var type in (IEnumerable)Call(
                ns,
                "GetTypeMembers",
                "Microsoft.CodeAnalysis.INamespaceOrTypeSymbol"
            )
        )
        foreach (var t in Nested(type))
            yield return t;
        foreach (
            var child in (IEnumerable)Call(
                ns,
                "GetNamespaceMembers",
                "Microsoft.CodeAnalysis.INamespaceSymbol"
            )
        )
        foreach (var t in Types(child))
            yield return t;
    }

    private static IEnumerable<object> Nested(object type)
    {
        yield return type;
        foreach (
            var inner in (IEnumerable)Call(
                type,
                "GetTypeMembers",
                "Microsoft.CodeAnalysis.INamespaceOrTypeSymbol"
            )
        )
        foreach (var t in Nested(inner))
            yield return t;
    }

    // Namespace and name, without generic arity, nested types joined with a dot
    private static string TypeName(object type)
    {
        var name = (string)Get(type, "Name", "Microsoft.CodeAnalysis.ISymbol");
        var outer = Get(type, "ContainingType", "Microsoft.CodeAnalysis.ISymbol");
        if (outer != null)
            return TypeName(outer) + "." + name;
        var ns = Get(type, "ContainingNamespace", "Microsoft.CodeAnalysis.ISymbol");
        var nsName =
            ns == null
            || (bool)Get(ns, "IsGlobalNamespace", "Microsoft.CodeAnalysis.INamespaceSymbol")
                ? ""
                : ns.ToString();
        return nsName.Length == 0 ? name : nsName + "." + name;
    }

    private static object Get(object target, string property, string iface) =>
        roslyn.GetType(iface).GetProperty(property).GetValue(target);

    private static object Call(object target, string method, string iface) =>
        roslyn.GetType(iface).GetMethod(method, Type.EmptyTypes).Invoke(target, null);
}
