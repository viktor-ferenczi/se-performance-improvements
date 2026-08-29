using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Shared.Logging;

namespace Shared.Tools;

// Some plugins rewrite mod scripts before compilation, so the compiled assembly depends on
// their identity as well, not only on the script source code and the game version. The exact
// build of each such plugin must therefore contribute to the compilation cache keys, otherwise
// upgrading or recompiling one of them would keep serving assemblies from the cache which were
// rewritten by the old build or reference its no longer loadable assembly name.
public static class ModRewriterVersions
{
    // The plugin interface implemented by every plugin main type, matched by name to avoid
    // loading the game assembly which declares it (Initialize runs from the preloader hook
    // on the dedicated server, before the game starts).
    private const string PluginInterfaceName = "VRage.Plugins.IPlugin";

    // The mod rewriting hook, see FindModRewriters
    private const string RewriteMethodName = "Rewrite";

    private const BindingFlags DeclaredMembers = BindingFlags.Public | BindingFlags.NonPublic |
                                                 BindingFlags.Instance | BindingFlags.Static |
                                                 BindingFlags.DeclaredOnly;

    // SHA1 over the sorted module version IDs of the loaded mod rewriting plugin assemblies.
    // Null if none of them are loaded, keeping the cache keys unchanged in that case.
    public static byte[] Hash { get; private set; }

    // Human readable form of what was hashed, kept for LogVersions
    private static string rewriterDescriptions;

    private static bool initialized;

    // Detects the loaded mod rewriting plugins and captures the hash of their module version IDs.
    // Throws on any failure, terminating plugin initialization: the exception is caught by
    // Pulsar/Magnetar, logged as an ERROR and reported. Failing hard is deliberate, silently
    // skipping the rewriters would poison the compilation caches with wrongly keyed entries.
    //
    // Must be called before any mod or script compilation. No logging here: on the dedicated
    // server this runs from the preloader hook, before the game log exists (see LogVersions).
    // Idempotent, because the dedicated server calls it again on the Init fallback path.
    public static void Initialize()
    {
        if (initialized)
            return;
        initialized = true;

        var rewriters = FindModRewriters();
        if (rewriters.Count == 0)
            return;

        rewriters.Sort((a, b) => a.Mvid.CompareTo(b.Mvid));

        var mvids = new byte[rewriters.Count * 16];
        for (var i = 0; i < rewriters.Count; i++)
            rewriters[i].Mvid.ToByteArray().CopyTo(mvids, i * 16);

        using (var sha1 = SHA1.Create())
            Hash = sha1.ComputeHash(mvids);

        rewriterDescriptions = string.Join("; ", rewriters.Select(r => r.Description));
    }

    // Logs what Initialize detected, once a working logger is available
    public static void LogVersions(IPluginLogger log)
    {
        if (rewriterDescriptions != null)
            log.Info("Mod rewriting plugins included in the compilation cache keys: {0}", rewriterDescriptions);
        else
            log.Info("No mod rewriting plugins are loaded, compilation cache keys are not affected");
    }

    // Pulsar and Magnetar wire up mod rewriting by looking for a method named Rewrite declared
    // on the plugin's main type, the one implementing IPlugin (PluginInstance.DependencyInject
    // in their loaders). That contract is detected here directly, on the loaded types, instead
    // of reflecting into the loader's own bookkeeping, whose layout differs between the two and
    // changes between releases.
    //
    // The assembly's module version ID identifies the exact build and nothing else is needed:
    // Pulsar compiles plugins non-deterministically, so every recompilation mints a fresh random
    // MVID, and plugins shipped as prebuilt DLLs are normally built deterministically, where the
    // MVID is a content hash of the assembly. Either way it changes precisely when the build
    // does, unlike the assembly version, which recompilations routinely leave untouched.
    private static List<(Guid Mvid, string Description)> FindModRewriters()
    {
        var rewriters = new List<(Guid Mvid, string Description)>();
        var pluginTypeCount = 0;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
                continue;

            foreach (var type in GetLoadableTypes(assembly))
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (!type.GetInterfaces().Any(i => i.FullName == PluginInterfaceName))
                    continue;

                pluginTypeCount++;

                // GetMethods instead of GetMethod, the latter would throw on overloads
                if (!type.GetMethods(DeclaredMembers).Any(m => m.Name == RewriteMethodName))
                    continue;

                var mvid = assembly.ManifestModule.ModuleVersionId;
                var name = assembly.GetName();
                rewriters.Add((mvid, $"{name.Name ?? "unknown"} {name.Version?.ToString() ?? "unknown"} {type.FullName} {mvid:D}"));

                // One entry per assembly, no matter how many plugin types it declares
                break;
            }
        }

        // This plugin itself implements IPlugin, so not finding a single implementation means
        // the detection ran before the plugin assemblies were loaded, or the interface moved.
        if (pluginTypeCount == 0)
            throw new Exception($"ModRewriterVersions: no {PluginInterfaceName} implementations are loaded; has the loader or the game changed?");

        return rewriters;
    }

    // Assemblies with unresolvable references yield only the types which did load
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }
}
