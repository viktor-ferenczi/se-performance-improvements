// Lists the game API members PB scripts use, by compiling each script the way the game wraps it
// (MyScriptCompiler.GetInGameScript) and walking the semantic model.
//
// Usage: PbScan <Bin64> <corpus dir> <out.json>
// The corpus dir holds one folder per script (Workshop id) with its .cs files.

using System.Reflection;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: PbScan <Bin64> <corpus dir> <out.json>");
    return 2;
}

var bin64 = args[0];
var net48 = Assembly
    .GetExecutingAssembly()
    .GetCustomAttributes<AssemblyMetadataAttribute>()
    .First(a => a.Key == "Net48Refs")
    .Value;

// Same assemblies as MySpaceGameDefaultIlCompiler.InitIlCompiler, plus VRage.Scripting for MemorySafeTypes
string[] gameDlls =
[
    "Sandbox.Game",
    "Sandbox.Common",
    "Sandbox.Graphics",
    "VRage",
    "VRage.Library",
    "VRage.Math",
    "VRage.Game",
    "VRage.Render",
    "VRage.Input",
    "SpaceEngineers.ObjectBuilders",
    "SpaceEngineers.Game",
    "System.Collections.Immutable",
    "ProtoBuf.Net.Core",
    "VRage.Scripting",
];
var references = Directory
    .GetFiles(net48, "*.dll")
    .Concat(Directory.GetFiles(Path.Combine(net48, "Facades"), "*.dll"))
    .Where(p =>
        !gameDlls.Contains(Path.GetFileNameWithoutExtension(p)) && !p.Contains("EnterpriseServices")
    )
    .Concat(gameDlls.Select(n => Path.Combine(bin64, n + ".dll")))
    .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
    .ToList();

// MyScriptCompiler ctor and MySpaceGameDefaultIlCompiler: implicit namespaces and type mappings
string[] usings =
[
    "System",
    "System.Text",
    "System.Collections",
    "System.Collections.Generic",
    "System.Linq",
    "VRage.Scripting.MemorySafeTypes",
    "VRage",
    "VRageMath",
    "VRage.Game",
    "Sandbox.ModAPI.Interfaces",
    "Sandbox.ModAPI.Ingame",
    "Sandbox.Game.EntityComponents",
    "VRage.Game.Components",
    "VRage.Collections",
    "VRage.Game.ObjectBuilders.Definitions",
    "VRage.Game.ModAPI.Ingame",
    "VRage.Game.ModAPI.Ingame.Utilities",
    "System.Collections.Immutable",
    "SpaceEngineers.Game.ModAPI.Ingame",
    "VRage.Game.GUI.TextPanel",
];
string[] mappedTypes =
[
    "INotifyPropertyChanging",
    "PropertyChangingEventHandler",
    "PropertyChangingEventArgs",
    "INotifyPropertyChanged",
    "PropertyChangedEventHandler",
    "PropertyChangedEventArgs",
];
var header =
    string.Concat(usings.Select(u => $"using {u};\n"))
    + string.Concat(mappedTypes.Select(t => $"using {t} = System.ComponentModel.{t};\n"))
    + "public sealed class Program : MyGridProgram {\n#line 1 \"Program\"\n";

var parseOptions = new CSharpParseOptions(
    LanguageVersion.CSharp6,
    DocumentationMode.None
).WithPreprocessorSymbols("BRANCH_STABLE", "STABLE", "VERSION_210", "BUILD_14");
var compileOptions = new CSharpCompilationOptions(
    OutputKind.DynamicallyLinkedLibrary,
    platform: Platform.X64
);

var results = new List<object>();
foreach (var dir in Directory.GetDirectories(args[1]).OrderBy(d => d))
{
    var files = Directory
        .GetFiles(dir, "*.cs", SearchOption.AllDirectories)
        .OrderBy(f => f)
        .ToList();
    if (files.Count == 0)
        continue;

    // The game compiles one text; Workshop items built with MDK publish a single Script.cs
    var code = string.Join("\n", files.Select(File.ReadAllText));
    var tree = CSharpSyntaxTree.ParseText(header + code + "\n}\n", parseOptions);
    var compilation = CSharpCompilation.Create("Script", [tree], references, compileOptions);
    var model = compilation.GetSemanticModel(tree);
    var errors = compilation
        .GetDiagnostics()
        .Where(d => d.Severity == DiagnosticSeverity.Error)
        .Select(d => d.ToString())
        .ToList();

    var members = new SortedDictionary<string, Usage>();
    foreach (var name in tree.GetRoot().DescendantNodes().OfType<SimpleNameSyntax>())
    {
        // Skip the wrapper's usings and base type
        if (name.SpanStart < header.Length)
            continue;
        var symbol = model.GetSymbolInfo(name).Symbol;
        // Named arguments bind to parameters, which are no API surface of their own
        if (symbol == null || symbol is IParameterSymbol || !IsGame(symbol))
            continue;
        if (symbol is IMethodSymbol { ReducedFrom: { } reduced })
            symbol = reduced;
        symbol = symbol.OriginalDefinition;

        string key;
        string kind;
        if (symbol is INamedTypeSymbol type)
        {
            key = type.ToDisplayString();
            kind = "type";
        }
        else if (symbol.ContainingType != null)
        {
            key = symbol.ContainingType.ToDisplayString() + "." + symbol.Name;
            kind = symbol switch
            {
                IMethodSymbol { MethodKind: MethodKind.Constructor } => "ctor",
                IMethodSymbol m => m.IsExtensionMethod ? "extension" : "method",
                IPropertySymbol => "property",
                IFieldSymbol f => f.IsConst || f.ContainingType.TypeKind == TypeKind.Enum
                    ? "const"
                    : "field",
                IEventSymbol => "event",
                _ => symbol.Kind.ToString().ToLowerInvariant(),
            };
            if (symbol.IsStatic && kind is "method" or "property" or "field")
                kind = "static " + kind;
        }
        else
            continue;

        if (!members.TryGetValue(key, out var usage))
            members[key] = usage = new Usage { Kind = kind };
        var access = Access(name);
        if ((access & 1) != 0)
            usage.Reads++;
        if ((access & 2) != 0)
            usage.Writes++;
    }

    results.Add(
        new
        {
            Id = Path.GetFileName(dir),
            Bytes = code.Length,
            Errors = errors,
            Members = members,
        }
    );
    Console.WriteLine($"{Path.GetFileName(dir)}: {members.Count} members, {errors.Count} errors");
}

File.WriteAllText(
    args[2],
    JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true })
);
return 0;

static bool IsGame(ISymbol symbol)
{
    var assembly = symbol.ContainingAssembly?.Name ?? "";
    return assembly.StartsWith("Sandbox.")
        || assembly.StartsWith("VRage")
        || assembly.StartsWith("SpaceEngineers.");
}

// 1 = read, 2 = write, 3 = both (compound assignment, ++/--, ref/out)
static int Access(SimpleNameSyntax name)
{
    SyntaxNode expr = name.Parent is MemberAccessExpressionSyntax ma && ma.Name == name ? ma : name;
    if (expr.Parent is MemberBindingExpressionSyntax)
        expr = expr.Parent;
    return expr.Parent switch
    {
        AssignmentExpressionSyntax a when a.Left == expr => a.IsKind(
            SyntaxKind.SimpleAssignmentExpression
        )
            ? 2
            : 3,
        PrefixUnaryExpressionSyntax u
            when u.IsKind(SyntaxKind.PreIncrementExpression)
                || u.IsKind(SyntaxKind.PreDecrementExpression) => 3,
        PostfixUnaryExpressionSyntax => 3,
        ArgumentSyntax arg when !arg.RefKindKeyword.IsKind(SyntaxKind.None) => 3,
        _ => 1,
    };
}

class Usage
{
    public string Kind { get; set; }
    public int Reads { get; set; }
    public int Writes { get; set; }
}
