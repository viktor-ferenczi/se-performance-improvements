using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using HarmonyLib;
using PluginSdk.Config;
using PluginSdk.Paths;
using ServerPlugin.Config;
using Shared.Config;
using Shared.Logging;
using Shared.Patches;
using Shared.Plugin;
using VRage.FileSystem;
using VRage.Game;
using VRage.Plugins;
using SdkLogger = PluginSdk.Logging.Logger;

// Define assembly version when compiled by Magnetar
#if !DEV_BUILD
using System.Reflection;

[assembly: AssemblyVersion("1.11.22")]
[assembly: AssemblyFileVersion("1.11.22")]
#endif

namespace ServerPlugin;

// ReSharper disable once UnusedType.Global
public class Plugin : IPlugin, ICommonPlugin
{
    public const string Name = "Performance";
    public static Plugin Instance { get; private set; }

    public long Tick { get; private set; }
    private static bool failed;

    // Shared logger, used by the Harmony patch scaffolding via Plugin.Common.Logger.
    public IPluginLogger Log => Logger;
    private static readonly IPluginLogger Logger = new PluginLogger(Name);

    // PluginSdk logger: writes to the Magnetar game log when standalone, or
    // structured JSON when managed by Quasar. Used for config lifecycle logging.
    private static readonly SdkLogger SdkLog = SdkLogger.Create(Name);

    // The PluginSdk-managed configuration is the single source of truth on the
    // server. It also implements the shared IPluginConfig so the patches gate on it.
    public IPluginConfig Config => config;
    private static PerformanceConfig config;

    // Expose the configuration for Quasar to discover via its agent
    // ReSharper disable once UnusedMember.Global
    public PerformanceConfig PluginConfig => config;

    private static string configPath;

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public void Init(object gameInstance)
    {
#if DEBUG
        // Allow the debugger some time to connect once the plugin assembly is loaded
        Thread.Sleep(100);
#endif

        Instance = this;

        SdkLog.Info("Loading");

        // Resolve the config path case-insensitively: a no-op on Windows, the
        // LinuxCompat resolver on Linux. One code path works on both.
        configPath = PathResolver.Normalize(Path.Combine(MyFileSystem.UserDataPath, $"{Name}.cfg"));

        // Load existing values, or a default-constructed instance when absent.
        config = ConfigStorage.LoadXml<PerformanceConfig>(configPath);

        // Persist immediately so a fresh install leaves a sparse on-disk file
        // (only non-default values) to inspect.
        TrySaveConfig();

        // Re-persist whenever the live config changes (e.g. pushed by Quasar).
        config.PropertyChanged += OnConfigChanged;

        var gameVersion = MyFinalBuildConstants.APP_VERSION_STRING.ToString();
        Common.SetPlugin(this, gameVersion, MyFileSystem.UserDataPath);

        if (!PatchHelpers.HarmonyPatchAll(Log, new Harmony(Name)))
        {
            failed = true;
            return;
        }

        SdkLog.Info("Successfully loaded");
    }

    private static void OnConfigChanged(object sender, PropertyChangedEventArgs e)
    {
        SdkLog.Info($"Config changed: {e.PropertyName}");
        TrySaveConfig();
    }

    internal static void TrySaveConfig()
    {
        if (config == null || configPath == null)
            return;

        try
        {
            ConfigStorage.SaveXml(config, configPath);
        }
        catch (Exception ex)
        {
            SdkLog.Error("Failed to save config", ex);
        }
    }

    public void Dispose()
    {
        try
        {
            if (config != null)
                config.PropertyChanged -= OnConfigChanged;
        }
        catch (Exception ex)
        {
            SdkLog.Critical("Dispose failed", ex);
        }

        Instance = null;
    }

    public void Update()
    {
        if (failed)
            return;

#if DEBUG
        CustomUpdate();
        Tick++;
#else
        try
        {
            CustomUpdate();
            Tick++;
        }
        catch (Exception e)
        {
            SdkLog.Critical("Update failed", e);
            failed = true;
        }
#endif
    }

    private void CustomUpdate()
    {
        PatchHelpers.PatchUpdates();
    }
}
