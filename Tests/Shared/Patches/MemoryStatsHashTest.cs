using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shared.Tools;
using Xunit;

namespace Tests.Shared.Patches
{
    // MyWindowsSystemPatchForStats must accept the game's getter body and the Linux compatibility
    // layer's, and nothing else. A refused body stops every uncategorized patch from being applied.
    public class MemoryStatsHashTest
    {
        private const string WindowsBody = "4d18b11a";
        private const string LinuxBody = "4a3cb737";

        private static readonly MethodInfo IsAllowed = typeof(EnsureCode).GetMethod(
            "IsAllowed",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        [Fact]
        public void BothPatchMethodsAllowExactlyTheTwoGetterBodies()
        {
            var allowed = EnsureCodeArguments("Shared/Patches/Bullshit/MyWindowsSystemPatchForStats.cs");

            Assert.Equal(new[] { "ProcessPrivateMemoryGetterPostfix", "ProcessPrivateMemoryGetterPrefix" }, allowed.Select(a => a.Method).OrderBy(m => m));
            Assert.All(allowed, a => Assert.Equal($"{WindowsBody}|{LinuxBody}", a.Hashes));
        }

        [Theory]
        [InlineData(WindowsBody, true)]
        [InlineData(LinuxBody, true)]
        [InlineData("00000000", false)]
        [InlineData("4a3cb738", false)]
        [InlineData("4d18b11", false)]
        [InlineData("d18b11a", false)]
        [InlineData("", false)]
        public void OnlyTheListedBodiesPass(string hash, bool expected)
        {
            var check = new EnsureCode($"{WindowsBody}|{LinuxBody}");

            Assert.Equal(expected, (bool)IsAllowed.Invoke(check, new object[] { hash }));
        }

        private static (string Method, string Hashes)[] EnsureCodeArguments(string relativePath)
        {
            var source = File.ReadAllText(Path.Combine(RepositoryRoot(), relativePath));
            return CSharpSyntaxTree.ParseText(source)
                .GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .SelectMany(method => method.AttributeLists
                    .SelectMany(list => list.Attributes)
                    .Where(attribute => attribute.Name.ToString() == "EnsureCode")
                    .Select(attribute => (method.Identifier.Text, ((LiteralExpressionSyntax)attribute.ArgumentList.Arguments[0].Expression).Token.ValueText)))
                .ToArray();
        }

        private static string RepositoryRoot()
        {
            var folder = new DirectoryInfo(AppContext.BaseDirectory);
            while (folder != null && !File.Exists(Path.Combine(folder.FullName, "Performance.sln")))
                folder = folder.Parent;
            return folder?.FullName ?? throw new DirectoryNotFoundException("Performance.sln not found above the test output");
        }
    }
}
