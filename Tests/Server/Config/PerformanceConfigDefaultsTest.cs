using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Tests.Server.Config
{
    // Quasar records the server config's defaults in its own process, and every cluster node
    // compares its defaults with those at startup. A default computed from the process kind
    // or from environment variables differs between the two and stops every node (SE-1956).
    // So each default in PerformanceConfig has to be a compile-time constant.
    public class PerformanceConfigDefaultsTest
    {
        // Machine dependent on purpose: the UI shows the automatic thread count this host
        // gets. They only match while Quasar runs on the same host as the nodes.
        private static readonly string[] MachineDependent =
        {
            "HavokThreadCount",
            "ScriptThreadCount",
        };

        private static string RepoRoot([CallerFilePath] string path = "") =>
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..", "..", ".."));

        [Fact]
        public void Every_default_is_a_compile_time_constant()
        {
            var root = RepoRoot();
            var trees = new[]
            {
                "ServerPlugin/Config/PerformanceConfig.cs",
                "Shared/Config/IPluginConfig.cs",
                "Shared/Tools/PerformanceStorage.cs",
            }
                .Select(file =>
                    CSharpSyntaxTree.ParseText(
                        File.ReadAllText(Path.Combine(root, file)),
                        path: file
                    )
                )
                .ToArray();

            // PluginSdk is not referenced: its types stay unresolved, which does not keep the
            // compiler from folding the constants used by the initializers.
            var trusted = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            var compilation = CSharpCompilation.Create(
                "PerformanceConfigDefaults",
                trees,
                trusted.Split(Path.PathSeparator).Select(p => MetadataReference.CreateFromFile(p)),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
            var tree = trees[0];
            var model = compilation.GetSemanticModel(tree);

            var properties = tree.GetRoot()
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => p.Initializer != null)
                .ToList();
            Assert.Contains(properties, p => p.Identifier.Text == "SleepSupportedGrids");

            var computed = properties
                .Where(p => !MachineDependent.Contains(p.Identifier.Text))
                .Where(p => !model.GetConstantValue(p.Initializer.Value).HasValue)
                .Select(p => $"{p.Identifier.Text} = {p.Initializer.Value}")
                .ToList();
            Assert.Empty(computed);
        }
    }
}
