// DO NOT USE A NAMESPACE HERE!
// CRITICAL: Magnetar (and Pulsar) locate this class via assembly.GetType("Preloader"),
// which only succeeds for a top-level type with no namespace.

// The only Cecil pre-patch is IlInjectorPrepatch (FixScriptCounter). TargetDLLs names its
// assembly only while that option is on in the config file, so with the option off the loader
// leaves VRage.Library alone and only the Finish() post-hook is used. Magnetar runs the hook
// either way, because HasPatches counts post-hooks (see Magnetar Shared/Preloader.cs).
//
// Finish() runs in SetupPlugins, right after the game assembly resolver is set up and
// before the game starts (Program.cs: PreHooks -> Patch -> SetupGameResolver -> PostHooks).
// At that point the game has not initialized yet, so we can install a Harmony hook on
// MyInitializer.InvokeBeforeRun. When it later runs (early in the dedicated server startup,
// after logging/filesystem are ready but before world-load compilation), the hook applies
// the rest of the plugin's patches. See ServerPlugin.Plugin.InstallEarlyBootstrap.
//
// ReSharper disable once UnusedType.Global
public class Preloader
{
    private static readonly bool FixScriptCounter = Shared.Tools.PreloadConfig.ReadBool(
        true,
        "FixScriptCounter"
    );

    // ReSharper disable once UnusedMember.Global
    public static System.Collections.Generic.IEnumerable<string> TargetDLLs =>
        FixScriptCounter ? new[] { "VRage.Library.dll" } : new string[0];

    // ReSharper disable once UnusedMember.Global
    public static void Patch(Mono.Cecil.AssemblyDefinition assembly)
    {
        if (assembly.Name.Name == "VRage.Library")
            Shared.Patches.IlInjectorPrepatch.Prepatch(assembly);
    }

    // ReSharper disable once UnusedMember.Global
    public static void Finish()
    {
        // Detect the versions of the loaded mod rewriting plugins while still inside
        // Magnetar's loader: on failure the exception is caught by Magnetar's SafeInvoke,
        // logged as an ERROR and the loader terminates (Environment.Exit). The game log
        // does not exist yet, so this only detects; the versions are logged later, from
        // Common.SetPlugin. The client has no preloader; there Pulsar catches the same
        // exception when thrown from IPlugin.Init.
        Shared.Tools.ModRewriterVersions.Initialize();

        ServerPlugin.Plugin.InstallEarlyBootstrap();
    }
}
