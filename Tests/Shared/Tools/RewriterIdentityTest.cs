using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools
{
    public class RewriterIdentityTest
    {
        private const string Source = """
            using System;
            [AttributeUsage(AttributeTargets.All)] public class MarkAttribute : Attribute { public MarkAttribute(string text) { } }
            public interface IShim { string Apply(string text); }
            [Mark("a")] public class Shim : IShim
            {
                private static readonly int[] Table = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                public event Action Done;
                public int Size => Table.Length;
                public string Apply(string text)
                {
                    Done?.Invoke();
                    return "prefix" + text.ToUpperInvariant() + new { Size }.ToString();
                }
            }
            """;

        private static byte[] Compile(string source, string assemblyName)
        {
            var trusted = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { CSharpSyntaxTree.ParseText(source) },
                trusted.Split(Path.PathSeparator).Select(p => MetadataReference.CreateFromFile(p)),
                new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    deterministic: false
                )
            );
            using var stream = new MemoryStream();
            Assert.True(compilation.Emit(stream).Success);
            return stream.ToArray();
        }

        private static string RandomName() => "Rewriter_" + Path.GetRandomFileName();

        [Fact]
        public void Rebuild_under_another_random_name_keeps_the_id()
        {
            var first = Compile(Source, RandomName());
            var expected = RewriterIdentity.ContentId(first);
            for (var i = 0; i < 20; i++)
            {
                var again = Compile(Source, RandomName());
                Assert.False(first.SequenceEqual(again));
                Assert.Equal(expected, RewriterIdentity.ContentId(again));
            }
        }

        [Theory]
        [InlineData("text.ToUpperInvariant()", "text.ToLowerInvariant()")] // member reference
        [InlineData("\"prefix\"", "\"suffix\"")] // user string
        [InlineData("10, 11, 12", "10, 11, 13")] // field data
        [InlineData("[Mark(\"a\")]", "[Mark(\"b\")]")] // attribute value
        [InlineData(
            "public string Apply(string text)\n",
            "public virtual string Apply(string text)\n"
        )] // method attributes
        [InlineData("Done?.Invoke();", "")] // method body
        public void Changing_the_code_changes_the_id(string from, string to)
        {
            var changed = Source.Replace(from, to);
            Assert.NotEqual(Source, changed);
            Assert.NotEqual(
                RewriterIdentity.ContentId(Compile(Source, RandomName())),
                RewriterIdentity.ContentId(Compile(changed, RandomName()))
            );
        }

        [Fact]
        public void Renaming_a_method_changes_the_id()
        {
            Assert.NotEqual(
                RewriterIdentity.ContentId(Compile(Source, RandomName())),
                RewriterIdentity.ContentId(
                    Compile(Source.Replace("Apply", "Convert"), RandomName())
                )
            );
        }

        [Theory]
        [InlineData("LinuxCompat_qej1e43n.ipq", "LinuxCompat")]
        [InlineData("Some_Plugin_abcdefgh.012", "Some_Plugin")]
        [InlineData("LinuxCompat", null)]
        [InlineData("Some_Plugin", null)]
        [InlineData(null, null)]
        public void Plugin_name_drops_only_the_random_suffix(string assemblyName, string expected)
        {
            Assert.Equal(expected, RewriterIdentity.PluginNameOf(assemblyName));
        }

        [Fact]
        public void Reference_to_an_old_build_resolves_to_the_loaded_build()
        {
            var name = "RedirectTarget_" + Path.GetRandomFileName();
            var loaded = Assembly.Load(Compile(Source, name));
            RewriterIdentity.RedirectOldBuilds(new[] { loaded });

            var resolved = Assembly.Load("RedirectTarget_" + Path.GetRandomFileName());

            Assert.Same(loaded, resolved);
            Assert.Throws<FileNotFoundException>(() =>
                Assembly.Load("Unrelated_" + Path.GetRandomFileName())
            );
        }
    }
}
