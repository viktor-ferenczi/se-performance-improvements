// DO NOT USE A NAMESPACE HERE!
// CRITICAL: Pulsar locates this class via assembly.GetType("Preloader"), which only succeeds
// for a top-level type with no namespace.

// This plugin does no Cecil pre-patching, so it declares neither TargetDLLs nor a Patch method.
// Only the Finish() post-hook is used. It runs after Pulsar's game assembly resolver is in place
// but before the game's Main, which is the only point early enough to get a Harmony hook onto
// MyInitializer.InvokeBeforeRun. When that hook runs, the game's filesystem and logging are ready
// but MySandboxGame has not started its startup preloading yet, which is what the patches in the
// "Early" category need. See ClientPlugin.Plugin.InstallEarlyBootstrap.
//
// ReSharper disable once UnusedType.Global
public class Preloader
{
    // ReSharper disable once UnusedMember.Global
    public static void Finish()
    {
        ClientPlugin.Plugin.InstallEarlyBootstrap();
    }
}
