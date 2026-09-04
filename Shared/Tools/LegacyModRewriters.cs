using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shared.Tools;

// LEGACY FALLBACK. Remove this file, its entry in Shared.projitems and its single call site
// in ModRewriterVersions.FindModRewriters once the old Pulsar and Magnetar releases, the ones
// loading the compat plugins by their legacy IDs (se-dotnet-compat, se-linux-compat), are no
// longer supported.
//
// Those loaders predate the Rewrite hook. The compat plugin builds they pin on PluginHub and
// MagnetarHub (dotnet-compat 90b08f2 on the client and 0f01870 on the server, linux-compat
// 0d7204b on both) rewrite mod scripts on their own instead: dotnet-compat patches
// MyScriptCompiler.CreateCompilation and exposes the CompilerHookExtensions.RewriterFactories
// list, and linux-compat appends its PathSubstitutionRewriter to that list from its
// RewriterRegistration type. Neither of them declares a Rewrite method on its main type,
// so the modern detection does not see them, while the cached mod assemblies still depend
// on their exact builds (they even reference shim types inside the linux-compat assembly).
//
// They are recognized here by the presence of those hook types. The types exist as soon as
// the plugin assemblies are loaded, so this works from the preloader hook and regardless of
// the plugin Init order, and the assemblies' MVIDs feed the same hash as the modern detection.
internal static class LegacyModRewriters
{
    // Full names of the marker types with the plugin each of them identifies. The compat
    // plugins use the ClientPlugin root namespace in the client and ServerPlugin in the
    // dedicated server build.
    private static readonly (string TypeName, string PluginName)[] MarkerTypes =
    {
        ("ClientPlugin.Rewriter.CompilerHookExtensions", "dotnet-compat"),
        ("ServerPlugin.Rewriter.CompilerHookExtensions", "dotnet-compat"),
        ("ClientPlugin.Rewriter.RewriterRegistration", "linux-compat"),
        ("ServerPlugin.Rewriter.RewriterRegistration", "linux-compat"),
    };

    // Same result shape as ModRewriterVersions.FindModRewriters, empty if none are loaded
    public static List<(Guid Mvid, string Description)> Find()
    {
        var rewriters = new List<(Guid Mvid, string Description)>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
                continue;

            foreach (var (typeName, pluginName) in MarkerTypes)
            {
                if (GetTypeOrNull(assembly, typeName) == null)
                    continue;

                var mvid = assembly.ManifestModule.ModuleVersionId;
                var name = assembly.GetName();
                rewriters.Add((mvid, $"legacy {pluginName} {name.Name ?? "unknown"} {name.Version?.ToString() ?? "unknown"} {typeName} {mvid:D}"));

                // One entry per assembly
                break;
            }
        }

        return rewriters;
    }

    // Assembly.GetType can throw instead of returning null when the type's dependencies fail to resolve
    private static Type GetTypeOrNull(Assembly assembly, string typeName)
    {
        try
        {
            return assembly.GetType(typeName, throwOnError: false);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
