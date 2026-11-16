using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shared.Patches;
using Xunit;

namespace Tests.Shared.Patches;

// Since 1.211.017 the PB compiler rewrites static Regex calls into calls of
// VRage.Scripting.CompilerMethods.MyScriptRegex (RegexTimeoutRewriter). A script using them
// must still be able to run off the main thread, while the rest of that namespace
// (ModPerfCounter) keeps it on the main thread.
public class ScriptRegexEligibilityTest
{
    private const string Game =
        @"namespace VRage.Scripting.CompilerMethods
{
    public static class MyScriptRegex { public static bool IsMatch(string input, string pattern) => true; }
    public static class ModPerfCounter { public static void Hit() { } }
}";

    private static Assembly Script(string body)
    {
        var context = new AssemblyLoadContext(null, isCollectible: true);
        var game = Compile("FakeGame", Game);
        var gameAssembly = context.LoadFromStream(new MemoryStream(game.bytes));
        context.Resolving += (_, name) => name.Name == "FakeGame" ? gameAssembly : null;
        var script = Compile(
            "FakeScript",
            $"public class Program {{ public void Main() {{ {body} }} }}",
            game.reference
        );
        return context.LoadFromStream(new MemoryStream(script.bytes));
    }

    private static (byte[] bytes, MetadataReference reference) Compile(
        string name,
        string source,
        params MetadataReference[] extra
    )
    {
        var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location);
        var references = new[] { "System.Private.CoreLib.dll", "System.Runtime.dll" }
            .Select(f =>
                (MetadataReference)MetadataReference.CreateFromFile(Path.Combine(runtime, f))
            )
            .Concat(extra);
        var compilation = CSharpCompilation.Create(
            name,
            new[] { CSharpSyntaxTree.ParseText(source) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        using var stream = new MemoryStream();
        var emitted = compilation.Emit(stream);
        Assert.True(emitted.Success, string.Join("\n", emitted.Diagnostics));
        return (stream.ToArray(), compilation.ToMetadataReference());
    }

    [Fact]
    public void RewrittenRegexCallsStayOffMain()
    {
        var result = ScriptEligibility.Of(
            Script("VRage.Scripting.CompilerMethods.MyScriptRegex.IsMatch(\"a\", \"a\");")
        );
        Assert.True(result.OffMain, string.Join(", ", result.Blocking));
    }

    [Fact]
    public void OtherCompilerMethodsKeepTheScriptOnMain()
    {
        var result = ScriptEligibility.Of(
            Script("VRage.Scripting.CompilerMethods.ModPerfCounter.Hit();")
        );
        Assert.False(result.OffMain);
        Assert.Contains("VRage.Scripting.CompilerMethods.ModPerfCounter.Hit", result.Blocking);
    }
}
