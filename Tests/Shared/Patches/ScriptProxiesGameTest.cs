using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Shared.Patches;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Shared.Patches;

// Emits the proxy of every game class with script interfaces, from the game's own assemblies,
// to catch signatures the generator can't handle. Skipped when the game is not installed;
// SE_BIN64 overrides the Bin64 folder.
public class ScriptProxiesGameTest
{
    private readonly ITestOutputHelper output;

    public ScriptProxiesGameTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    private static readonly string[] ScriptNamespaces =
    {
        "Sandbox.ModAPI.Ingame",
        "SpaceEngineers.Game.ModAPI.Ingame",
        "VRage.Game.ModAPI.Ingame",
        "Sandbox.ModAPI.Interfaces",
    };

    [Fact]
    public void EveryGameClassGetsAProxy()
    {
        var bin64 =
            Environment.GetEnvironmentVariable("SE_BIN64")
            ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                OperatingSystem.IsWindows()
                    ? @"C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64"
                    : ".steam/steam/steamapps/common/SpaceEngineers/Bin64"
            );
        if (!File.Exists(Path.Combine(bin64, "Sandbox.Game.dll")))
            return;

        AssemblyLoadContext.Default.Resolving += (c, name) =>
        {
            var file = Path.Combine(bin64, name.Name + ".dll");
            return File.Exists(file) ? c.LoadFromAssemblyPath(file) : null;
        };
        // The fake interfaces of ScriptProxiesTest keep working with the default filter
        var fallback = ScriptProxies.IsScriptInterface;
        ScriptProxies.IsScriptInterface = t =>
            ScriptNamespaces.Contains(t.Namespace) || fallback(t);

        var classes = new List<Type>();
        foreach (var name in new[] { "Sandbox.Game", "SpaceEngineers.Game" })
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(
                Path.Combine(bin64, name + ".dll")
            );
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray();
            }
            classes.AddRange(
                types.Where(t =>
                    t.IsClass
                    && !t.IsGenericTypeDefinition
                    && t.GetInterfaces().Any(i => i.IsPublic && ScriptProxies.IsScriptInterface(i))
                )
            );
        }

        var failures = new List<string>();
        foreach (var type in classes)
        {
            try
            {
                ScriptProxies.Prepare(type);
            }
            catch (Exception e)
            {
                failures.Add($"{type.FullName}: {e.GetType().Name}: {e.Message}");
            }
        }
        output.WriteLine($"{classes.Count} classes, {failures.Count} failed");
        foreach (var failure in failures.Take(20))
            output.WriteLine(failure);
        Assert.True(classes.Count > 100, $"only {classes.Count} classes found");
        Assert.Empty(failures);
    }
}
