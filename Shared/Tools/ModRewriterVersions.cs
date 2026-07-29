using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Shared.Logging;

namespace Shared.Tools;

// Some plugins rewrite mod scripts before compilation, so the compiled assembly depends on
// their version as well, not only on the script source code and the game version. Their
// versions must therefore contribute to the compilation cache keys, otherwise upgrading such
// a plugin would keep serving assemblies rewritten by the old version from the cache.
public static class ModRewriterVersions
{
    // Plugin IDs known to rewrite mod scripts. Currently these are the compat plugins
    // Pulsar/Magnetar load implicitly (see GetCorePlugins in their Program.cs).
    private static readonly string[] ModRewritingPluginIds =
    {
        "se-dotnet-compat",
        "se-linux-compat",
    };

    // SHA1 over the sorted "id version" pairs of the loaded mod rewriting plugins.
    // Null if none of them are loaded, keeping the cache keys unchanged in that case.
    public static byte[] Hash { get; private set; }

    // Human readable form of the hashed version pairs, kept for LogVersions
    private static string combinedVersions;

    private static bool initialized;

    // Detects the loaded mod rewriting plugins and captures the hash of their versions.
    // Throws on any failure, terminating plugin initialization: the exception is caught by
    // Pulsar/Magnetar, logged as an ERROR and reported. Failing hard is deliberate, silently
    // skipping the versions would poison the compilation caches with wrongly keyed entries.
    //
    // Must be called before any mod or script compilation. No logging here: on the dedicated
    // server this runs from the preloader hook, before the game log exists (see LogVersions).
    // Idempotent, because the dedicated server calls it again on the Init fallback path.
    public static void Initialize()
    {
        if (initialized)
            return;
        initialized = true;

        var versions = FindModRewritingPluginVersions();
        if (versions.Count == 0)
            return;

        versions.Sort(StringComparer.Ordinal);
        combinedVersions = string.Join(";", versions);

        using (var sha1 = SHA1.Create())
            Hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(combinedVersions));
    }

    // Logs what Initialize detected, once a working logger is available
    public static void LogVersions(IPluginLogger log)
    {
        if (combinedVersions != null)
            log.Info("Mod rewriting plugins included in the compilation cache keys: {0}", combinedVersions);
        else
            log.Info("No mod rewriting plugins are loaded, compilation cache keys are not affected");
    }

    // The loader keeps the list of loaded plugins in Pulsar.Shared.Loader.Instance.Plugins,
    // a List<(PluginData, Assembly)>. The same type lives in both Pulsar (client) and
    // Magnetar (dedicated server), but it is not part of any public API, so it can only
    // be accessed via reflection.
    private static List<string> FindModRewritingPluginVersions()
    {
        var loaderType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.GetType("Pulsar.Shared.Loader"))
            .FirstOrDefault(t => t != null);
        if (loaderType == null)
            throw new Exception("ModRewriterVersions: Pulsar.Shared.Loader not found; this plugin must be loaded by Pulsar or Magnetar");

        var loader = (loaderType.GetField("Instance", BindingFlags.Public | BindingFlags.Static)
                      ?? throw new Exception("ModRewriterVersions: Pulsar.Shared.Loader.Instance field not found; has the loader layout changed?"))
            .GetValue(null)
            ?? throw new Exception("ModRewriterVersions: Pulsar.Shared.Loader.Instance is null; the loader has not run yet");

        var plugins = (loaderType.GetField("Plugins", BindingFlags.Public | BindingFlags.Instance)
                       ?? throw new Exception("ModRewriterVersions: Pulsar.Shared.Loader.Plugins field not found; has the loader layout changed?"))
            .GetValue(loader) as IEnumerable
            ?? throw new Exception("ModRewriterVersions: Pulsar.Shared.Loader.Plugins is not enumerable; has the loader layout changed?");

        var versions = new List<string>();
        foreach (var item in plugins)
        {
            // Item type is ValueTuple<PluginData, Assembly>
            var itemType = item.GetType();
            var data = (itemType.GetField("Item1")
                        ?? throw new Exception("ModRewriterVersions: Loader.Plugins items are not tuples; has the loader layout changed?"))
                .GetValue(item);
            if (data == null)
                continue;

            var idProperty = data.GetType().GetProperty("Id")
                             ?? throw new Exception("ModRewriterVersions: PluginData.Id property not found; has the loader layout changed?");
            if (!(idProperty.GetValue(data) is string id))
                continue;

            if (!ModRewritingPluginIds.Contains(id, StringComparer.OrdinalIgnoreCase))
                continue;

            var version = data.GetType().GetProperty("Version")?.GetValue(data) as Version
                          ?? (itemType.GetField("Item2")?.GetValue(item) as Assembly)?.GetName().Version;

            versions.Add($"{id.ToLowerInvariant()} {version?.ToString() ?? "unknown"}");
        }

        return versions;
    }
}
