using System;
using System.IO;
using System.Reflection;
using Shared.Config;
using Shared.Logging;
using Shared.Patches;

namespace Shared.Plugin;

public static class Common
{
    private const int CacheExpirationDays = 90;

    public static ICommonPlugin Plugin { get; private set; }
    public static IPluginLogger Logger { get; private set; }
    public static IPluginConfig Config { get; private set; }

    public static string GameVersion { get; private set; }
    public const string PluginVersion = "1.11.22";

    public static string DataDir { get; private set; }
    public static string CacheDir { get; private set; }
    public static string DebugDir { get; private set; }

    private static string CacheGameVersionPath => Path.Combine(CacheDir, "GameVersion.txt");
    private static string PluginVersionPath => Path.Combine(DebugDir, "PluginVersion.txt");

    // One-time setup: resolve storage paths, clean stale cache/debug files and configure the
    // patches. Called once per process — from the client's Init, or from the dedicated server's
    // early bootstrap (with a stand-in plugin; the live instance is attached later via
    // AttachPlugin). See ServerPlugin.Plugin.EarlyStartup.
    public static void SetPlugin(ICommonPlugin plugin, string gameVersion, string storageDir)
    {
        AttachPlugin(plugin);

        GameVersion = gameVersion;

        DataDir = Path.Combine(storageDir, "Performance");
        CacheDir = Path.Combine(DataDir, "Cache");
        DebugDir = Path.Combine(DataDir, "Debug");

        var hasGameVersionChanged = !File.Exists(CacheGameVersionPath) || File.ReadAllText(CacheGameVersionPath) != GameVersion;
        var hasPluginVersionChanged = !File.Exists(PluginVersionPath) || File.ReadAllText(PluginVersionPath) != PluginVersion;

        CleanupCache(hasGameVersionChanged);
        CleanupDebug(hasGameVersionChanged || hasPluginVersionChanged);

        PatchHelpers.Configure();
    }

    // Points the shared accessors at the given plugin instance. On the dedicated server the early
    // bootstrap runs against a stand-in; Init calls this to swap in the live instance once it
    // exists, so per-tick code reaches the real Tick counter.
    public static void AttachPlugin(ICommonPlugin plugin)
    {
        Plugin = plugin;
        Logger = plugin.Log;
        Config = plugin.Config;
    }

    private static void CleanupCache(bool clear)
    {
        Directory.CreateDirectory(CacheDir);

        var now = DateTime.UtcNow;
        foreach (var path in Directory.EnumerateFiles(CacheDir, "*.cache", SearchOption.AllDirectories))
        {
            if (clear || (now - File.GetCreationTimeUtc(path)).TotalDays >= CacheExpirationDays)
                File.Delete(path);
        }

        if (clear)
            File.WriteAllText(CacheGameVersionPath, GameVersion);
    }

    private static void CleanupDebug(bool clear)
    {
        Directory.CreateDirectory(DebugDir);

        if (!clear)
            return;

        foreach (var path in Directory.EnumerateFiles(DebugDir, "*.il", SearchOption.AllDirectories))
        {
            File.Delete(path);
        }

        File.WriteAllText(PluginVersionPath, PluginVersion);
    }
}
