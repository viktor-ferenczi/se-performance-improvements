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
    // Null if none of them are loaded (vanilla client, Torch), keeping cache keys unchanged.
    public static byte[] Hash { get; private set; }

    public static void Initialize(IPluginLogger log)
    {
        try
        {
            var versions = FindModRewritingPluginVersions();
            if (versions.Count == 0)
                return;

            versions.Sort(StringComparer.Ordinal);
            var combined = string.Join(";", versions);

            using (var sha1 = SHA1.Create())
                Hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(combined));

            log.Info("Mod rewriting plugins included in the compilation cache keys: {0}", combined);
        }
        catch (Exception e)
        {
            // Defensive: a loader layout change must not break the plugin, it may only
            // weaken cache invalidation until this code is updated to match.
            log.Warning(e, "Failed to detect mod rewriting plugin versions, they are not included in the compilation cache keys");
        }
    }

    // The loader keeps the list of loaded plugins in Pulsar.Shared.Loader.Instance.Plugins,
    // a List<(PluginData, Assembly)>. The same type lives in both Pulsar (client) and
    // Magnetar (dedicated server), but it is not part of any public API, so it can only
    // be accessed via reflection.
    private static List<string> FindModRewritingPluginVersions()
    {
        var versions = new List<string>();

        var loaderType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.GetType("Pulsar.Shared.Loader"))
            .FirstOrDefault(t => t != null);
        if (loaderType == null)
            return versions;

        var loader = loaderType.GetField("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
        if (loader == null)
            return versions;

        if (!(loaderType.GetField("Plugins", BindingFlags.Public | BindingFlags.Instance)?.GetValue(loader) is IEnumerable plugins))
            return versions;

        foreach (var item in plugins)
        {
            // Item type is ValueTuple<PluginData, Assembly>
            var itemType = item.GetType();
            var data = itemType.GetField("Item1")?.GetValue(item);
            if (data == null)
                continue;

            if (!(data.GetType().GetProperty("Id")?.GetValue(data) is string id))
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
