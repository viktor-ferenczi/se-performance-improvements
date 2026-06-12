using System;
using System.Threading;
using ClientPlugin.Settings;
using ClientPlugin.Settings.Layouts;
using HarmonyLib;
using Sandbox.Graphics.GUI;
using Shared.Config;
using Shared.Logging;
using Shared.Patches;
using Shared.Plugin;
using VRage.FileSystem;
using VRage.Game;
using VRage.Plugins;

// Define assembly version when compiled by Pulsar
#if !DEV_BUILD
using System.Reflection;

[assembly: AssemblyVersion("1.11.22")]
[assembly: AssemblyFileVersion("1.11.22")]
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

        var gameVersion = MyFinalBuildConstants.APP_VERSION_STRING.ToString();
        Common.SetPlugin(this, gameVersion, MyFileSystem.UserDataPath);

        if (!PatchHelpers.HarmonyPatchAll(Log, new Harmony(Name)))
        {
            failed = true;
            return;
        }

        Log.Debug("Successfully loaded");
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