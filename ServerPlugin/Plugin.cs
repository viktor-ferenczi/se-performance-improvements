using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using HarmonyLib;
using PluginSdk.Config;
using PluginSdk.Paths;
using Sandbox;
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

[assembly: AssemblyVersion("1.12.0")]
[assembly: AssemblyFileVersion("1.12.0")]
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

        // On the dedicated server the world (including mod and in-game script compilation) is
        // loaded before IPlugin.Init runs, so the patches are normally applied much earlier,
        // bootstrapped from the Preloader's Finish() hook (see InstallEarlyBootstrap). Run it
        // here as a fallback for the case the preloader path did not execute (e.g. Magnetar
        // safe mode): the compilation cache will not have helped this load, but the runtime
        // optimizations still apply for the rest of the session. Idempotent.
        EarlyStartup();
        if (failed)
            return;

        // Early startup ran against a stand-in plugin; point Common at the live instance so
        // per-tick code reaches the real Tick counter.
        Common.AttachPlugin(this);

        // Apply the patches deferred to the "Late" category. By now the world/session has loaded,
        // so their target assemblies (e.g. VRage.EOS) are available. On the dedicated server this
        // is the only point where these patches can be applied; the cache-relevant patches were
        // already applied early, before world-load compilation.
        if (!PatchHelpers.HarmonyPatchCategory(Logger, new Harmony(Name), PatchHelpers.LateCategory))
        {
            failed = true;
            return;
        }

        SdkLog.Info("Successfully loaded");
    }

    // Loads the PluginSdk configuration. Called once, from EarlyStartup.
    private static void LoadConfig()
    {
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
    }

    private static string GetGameVersion()
    {
        try
        {
            var serverBuildNumber = Sandbox.Game.MyPerGameSettings.BasicGameInfo.ServerBuildNumber.GetValueOrDefault();
            return $"{MyFinalBuildConstants.APP_VERSION_STRING_DOTS} b{serverBuildNumber}";
        }
        catch
        {
            // BasicGameInfo not populated yet (should not happen this late, but the
            // version only feeds the cache hash, so degrade gracefully rather than crash).
            return MyFinalBuildConstants.APP_VERSION_STRING_DOTS.ToString();
        }
    }

    // Called from the Preloader's Finish() hook (before the game starts). Installs a Harmony
    // postfix on MyInitializer.InvokeBeforeRun so the rest of the patching runs as soon as the
    // game has finished its core initialization — still well before world-load mod/script
    // compilation.
    //
    // InvokeBeforeRun is the earliest safe trigger: it is where the game calls MyFileSystem.Init
    // (so UserDataPath becomes available) AND assigns MyLog.Default (so the game logger works).
    // This method itself runs even earlier, before MyLog.Default exists, so it logs via SdkLog,
    // whose Magnetar sink is a no-op until the game log is ready.
    // ReSharper disable once UnusedMember.Global
    public static void InstallEarlyBootstrap()
    {
        try
        {
            var target = AccessTools.Method(typeof(MyInitializer), nameof(MyInitializer.InvokeBeforeRun));
            if (target == null)
            {
                SdkLog.Critical("Early bootstrap: MyInitializer.InvokeBeforeRun not found; the compilation cache will not be populated");
                return;
            }

            var postfix = new HarmonyMethod(AccessTools.Method(typeof(Plugin), nameof(OnGameInitialized)));
            new Harmony($"{Name}.Bootstrap").Patch(target, postfix: postfix);
        }
        catch (Exception ex)
        {
            SdkLog.Critical("Early bootstrap: failed to install the MyInitializer.InvokeBeforeRun hook", ex);
        }
    }

    // Harmony postfix on MyInitializer.InvokeBeforeRun. Public so Harmony can resolve it;
    // intentionally NOT decorated with [HarmonyPatch], so it is never re-applied by the patch
    // scan. Runs once the game's filesystem, logging and config are ready, but before any world
    // is loaded (and therefore before mod/script compilation).
    // ReSharper disable once UnusedMember.Global
    public static void OnGameInitialized()
    {
        try
        {
            EarlyStartup();
        }
        catch (Exception ex)
        {
            failed = true;
            SdkLog.Critical("Early startup failed", ex);
        }
    }

    private static bool earlyStarted;

    // One-shot early initialization: load config, run Common's heavy setup (paths, cache cleanup,
    // patch configuration) and apply the uncategorized patches — all before world-load
    // compilation. Called from the InvokeBeforeRun postfix (normal dedicated-server path) and
    // from Init (fallback); both run on the main thread, so a plain flag keeps it one-shot.
    private static void EarlyStartup()
    {
        if (earlyStarted)
            return;
        earlyStarted = true;

        LoadConfig();

        Common.SetPlugin(EarlyPlugin.Instance, GetGameVersion(), MyFileSystem.UserDataPath);

        // PerfCountingRewriterPatch targets a VRage.Scripting type by name, and EnsureCode
        // resolves by-name targets only among already-loaded assemblies. Touch the assembly so
        // it is loaded before verification runs. (MyScriptCompilerPatch references it via typeof,
        // but the order in which the scan visits patch classes is not guaranteed.)
        _ = typeof(VRage.Scripting.MyScriptCompiler);

        // Apply the uncategorized patches now, before world-load compilation. The deferred "Late"
        // category targets assemblies that are not loaded this early (e.g. VRage.EOS); those are
        // applied from Init.
        if (!PatchHelpers.HarmonyPatchUncategorized(Logger, new Harmony(Name)))
        {
            failed = true;
            return;
        }

        SdkLog.Info("Early patches applied, before world load");
    }

    // Lightweight ICommonPlugin used before the real plugin instance is available, so the shared
    // Common state (and every patch reading Common.Plugin) is valid during the early world-load
    // window on the dedicated server. Init swaps in the live instance via Common.AttachPlugin.
    private sealed class EarlyPlugin : ICommonPlugin
    {
        public static readonly EarlyPlugin Instance = new EarlyPlugin();
        public IPluginLogger Log => Logger;
        public IPluginConfig Config => config;
        public long Tick => 0; // No simulation ticks during world load
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
