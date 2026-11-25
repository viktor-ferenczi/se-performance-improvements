using System;
using System.IO;
using System.Reflection;
using Shared.Config;
using Shared.Logging;
using Shared.Patches;
using Shared.Tools;

namespace Shared.Plugin;

public static class Common
{
    public static ICommonPlugin Plugin { get; private set; }
    public static IPluginLogger Logger { get; private set; }
    public static IPluginConfig Config { get; private set; }

    public static string GameVersion { get; private set; }
    public const string PluginVersion = "1.17.2";

    public static string DataDir { get; private set; }
    public static string CacheDir { get; private set; }
    public static string CompilationCacheDir { get; private set; }
    public static string DebugDir { get; private set; }

    private static string CacheGameVersionPath => Path.Combine(CacheDir, "GameVersion.txt");
    private static string PluginVersionPath => Path.Combine(DebugDir, "PluginVersion.txt");

    // One-time setup: resolve storage paths, clean stale cache/debug files and configure the
    // patches. Called once per process — from the client's Init, or from the dedicated server's
    // early bootstrap (with a stand-in plugin; the live instance is attached later via
    // AttachPlugin). See ServerPlugin.Plugin.EarlyStartup.
    public static void SetPlugin(ICommonPlugin plugin, string gameVersion, string userDataPath)
    {
        AttachPlugin(plugin);

        GameVersion = gameVersion;

        var storage = PerformanceStorage.Prepare(userDataPath, message => Logger.Warning(message));
        DataDir = Path.Combine(storage, PerformanceStorage.DefaultRoot);
        CompilationCacheDir = PerformanceStorage.CompilationCache(
            storage,
            Config.CompilationCacheRoot
        );
        CacheDir = Path.Combine(DataDir, "Cache");
        DebugDir = Path.Combine(DataDir, "Debug");

        var hasGameVersionChanged =
            !File.Exists(CacheGameVersionPath)
            || File.ReadAllText(CacheGameVersionPath) != GameVersion;
        var hasPluginVersionChanged =
            !File.Exists(PluginVersionPath) || File.ReadAllText(PluginVersionPath) != PluginVersion;

        if (hasGameVersionChanged)
        {
            Directory.CreateDirectory(CacheDir);
            File.WriteAllText(CacheGameVersionPath, GameVersion);
        }
        CleanupDebug(hasGameVersionChanged || hasPluginVersionChanged);

        // Must run before any mod or script compilation, so the versions of the loaded
        // mod rewriting plugins contribute to the compilation cache keys. Throws on failure;
        // the exception propagates to Pulsar/Magnetar, which logs it as an ERROR and reports
        // the plugin as failed. On the dedicated server detection has already run from the
        // preloader hook (see ServerPlugin.Preloader.Finish), here it only logs.
        ModRewriterVersions.Initialize();
        ModRewriterVersions.LogVersions(Logger);

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

    private static void CleanupDebug(bool clear)
    {
        Directory.CreateDirectory(DebugDir);

        if (!clear)
            return;

        foreach (var path in Directory.EnumerateFiles(DebugDir, "*.il"))
        {
            if (PerformanceStorage.IsDebugDump(Path.GetFileName(path)))
                File.Delete(path);
        }

        File.WriteAllText(PluginVersionPath, PluginVersion);
    }
}
