// DO NOT USE A NAMESPACE HERE!
// CRITICAL: Pulsar locates this class via assembly.GetType("Preloader"), which only succeeds
// for a top-level type with no namespace.

// The only Cecil pre-patch is IlInjectorPrepatch (FixScriptCounter). TargetDLLs names its
// assembly only while that option is on in the config file, so with the option off the loader
// leaves VRage.Library alone and only the Finish() post-hook is used. Finish() runs after Pulsar's
// game assembly resolver is in place but before the game's Main, which is the only point early
// enough to get a Harmony hook onto MyInitializer.InvokeBeforeRun. When that hook runs, the game's filesystem and logging are ready
// but MySandboxGame has not started its startup preloading yet, which is what the patches in the
// "Early" category need. See ClientPlugin.Plugin.InstallEarlyBootstrap.
//
// ReSharper disable once UnusedType.Global
public class Preloader
{
    private static readonly bool FixScriptCounter = Shared.Tools.PreloadConfig.ReadBool(
        false,
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
        ClientPlugin.Plugin.InstallEarlyBootstrap();
    }
}
