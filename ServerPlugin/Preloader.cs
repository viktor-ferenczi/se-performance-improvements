// DO NOT USE A NAMESPACE HERE!
// CRITICAL: Magnetar (and Pulsar) locate this class via assembly.GetType("Preloader"),
// which only succeeds for a top-level type with no namespace.

// This plugin does no Cecil pre-patching, so it declares neither TargetDLLs nor a Patch
// method. Only the Finish() post-hook is used. Magnetar still runs the hook because
// HasPatches counts post-hooks (see Magnetar Shared/Preloader.cs).
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
    // ReSharper disable once UnusedMember.Global
    public static void Finish()
    {
        ServerPlugin.Plugin.InstallEarlyBootstrap();
    }
}
