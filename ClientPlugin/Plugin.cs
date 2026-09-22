using System;
using System.Threading;
using ClientPlugin.Settings;
using ClientPlugin.Settings.Layouts;
using HarmonyLib;
using Sandbox;
using Sandbox.Graphics.GUI;
using Shared.Config;
using Shared.Logging;
using Shared.Patches;
using Shared.Plugin;
using VRage.FileSystem;
using VRage.Game;
using VRage.Plugins;

// Define assembly version when compiled by Pulsar
#if !LOCAL_BUILD
using System.Reflection;

[assembly: AssemblyVersion("1.13.0")]
[assembly: AssemblyFileVersion("1.13.0")]
#endif

namespace ClientPlugin;

// ReSharper disable once UnusedType.Global
public class Plugin : IPlugin, ICommonPlugin
{
    public const string Name = "Performance";
    public static Plugin Instance { get; private set; }
    private SettingsGenerator settingsGenerator;
    public long Tick { get; private set; }
    private static bool failed;
    private static bool earlyStarted;

    public IPluginLogger Log => Logger;
    private static readonly IPluginLogger Logger = new PluginLogger(Name);

    // The in-game settings dialog (ClientPlugin.Config) is the client's
    // configuration. It implements the shared IPluginConfig the patches use.
    public IPluginConfig Config => ClientPlugin.Config.Current;

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public void Init(object gameInstance)
    {
#if DEBUG
        // Allow the debugger some time to connect once the plugin assembly is loaded
        Thread.Sleep(100);
#endif

        Instance = this;
        Instance.settingsGenerator = new SettingsGenerator();

        Log.Info("Loading");

        // Normally already done from the Preloader's hook. Run it here as a fallback for the
        // case that path did not execute (e.g. an older loader which does not call Finish):
        // the "Early" patches come too late for this session's preloading then, but they are
        // still verified and applied. Idempotent.
        OnGameInitialized();

        var clientBuildNumber = Sandbox.Game.MyPerGameSettings.BasicGameInfo.ClientBuildNumber.GetValueOrDefault();
        var gameVersion = $"{MyFinalBuildConstants.APP_VERSION_STRING_DOTS} b{clientBuildNumber}";
        Common.SetPlugin(this, gameVersion, MyFileSystem.UserDataPath);

        // The "Early" category has already been applied from the MyInitializer.InvokeBeforeRun
        // hook (see InstallEarlyBootstrap), so only the rest is applied here. Together the three
        // phases cover every patch in the assembly.
        var harmony = new Harmony(Name);
        if (!PatchHelpers.HarmonyPatchUncategorized(Log, harmony) ||
            !PatchHelpers.HarmonyPatchCategory(Log, harmony, PatchHelpers.LateCategory))
        {
            failed = true;
            return;
        }

        Log.Debug("Successfully loaded");
    }

    // Called from the Preloader's Finish() hook, before the game starts. Installs a Harmony
    // postfix on MyInitializer.InvokeBeforeRun so the patches which must be in place before the
    // game's own startup work can be applied in time. The startup preloading of the vanilla voxel
    // storages is started from MySandboxGame's startup path, which runs before IPlugin.Init, so a
    // patch applied from Init would come too late for it.
    // ReSharper disable once UnusedMember.Global
    public static void InstallEarlyBootstrap()
    {
        try
        {
            var target = AccessTools.Method(typeof(MyInitializer), nameof(MyInitializer.InvokeBeforeRun));
            if (target == null)
            {
                Logger.Critical("Early bootstrap: MyInitializer.InvokeBeforeRun not found; the early patches will not be applied");
                return;
            }

            var postfix = new HarmonyMethod(AccessTools.Method(typeof(Plugin), nameof(OnGameInitialized)));
            new Harmony($"{Name}.Bootstrap").Patch(target, postfix: postfix);
        }
        catch (Exception ex)
        {
            Logger.Critical(ex, "Early bootstrap: failed to install the MyInitializer.InvokeBeforeRun hook");
        }
    }

    // Harmony postfix on MyInitializer.InvokeBeforeRun. Public so Harmony can resolve it;
    // intentionally NOT decorated with [HarmonyPatch], so it is never re-applied by the patch
    // scan. Runs once the game's filesystem, logging and config are ready, but before the game
    // starts loading anything.
    // ReSharper disable once UnusedMember.Global
    public static void OnGameInitialized()
    {
        if (earlyStarted)
            return;
        earlyStarted = true;

        try
        {
            // Common's full setup needs the plugin instance and does file work; this early only
            // the logger and the config are needed, so a stand-in is attached and Init swaps in
            // the live instance via Common.SetPlugin.
            Common.AttachPlugin(EarlyPlugin.Instance);

            PatchHelpers.ConfigureEarly();

            // A failure here disables only the early patches, which is why it does not set the
            // failed flag: Init still applies everything else.
            PatchHelpers.HarmonyPatchCategory(Logger, new Harmony(Name), PatchHelpers.EarlyCategory);
        }
        catch (Exception ex)
        {
            Logger.Critical(ex, "Early bootstrap failed");
        }
    }

    // Lightweight ICommonPlugin for the early bootstrap window, before the game has created the
    // plugin's own state. Mirrors ServerPlugin.Plugin.EarlyPlugin.
    private sealed class EarlyPlugin : ICommonPlugin
    {
        public static readonly EarlyPlugin Instance = new EarlyPlugin();
        public IPluginLogger Log => Logger;
        public IPluginConfig Config => ClientPlugin.Config.Current;
        public long Tick => 0; // No simulation ticks this early
    }

    public void Dispose()
    {
        try
        {
            // TODO: Save state and close resources here, called when the game exists (not guaranteed!)
            // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.
        }
        catch (Exception ex)
        {
            Log.Critical(ex, "Dispose failed");
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
            Log.Critical(e, "Update failed");
            failed = true;
        }
#endif
    }

    private void CustomUpdate()
    {
        // TODO: Put your update code here. It is called on every simulation frame!
        PatchHelpers.PatchUpdates();
    }

    // ReSharper disable once UnusedMember.Global
    public void OpenConfigDialog()
    {
        Instance.settingsGenerator.SetLayout<Simple>();
        MyGuiSandbox.AddScreen(Instance.settingsGenerator.Dialog);
    }

    //TODO: Uncomment and use this method to load asset files
    /*public void LoadAssets(string folder)
    {

    }*/
}
