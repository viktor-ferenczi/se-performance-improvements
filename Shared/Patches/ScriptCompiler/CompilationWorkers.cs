using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using Shared.Plugin;
using Shared.Tools;
using VRage.Scripting;

namespace Shared.Patches;

// Queue at the platform entry point so both cache lookups and the original (also patched by
// Cluster) MyScriptCompiler.Compile run on a worker, exactly once per request.
[HarmonyPatch(typeof(MyVRageScriptingInternal), nameof(MyVRageScriptingInternal.CompileAsync))]
internal static class CompilationWorkers
{
    private static readonly CompilationQueue<Assembly> Queue = new(() =>
        Common.Config.MaxConcurrentCompilations
    );

    internal static bool Enabled =>
        Common.Config.Enabled && Common.Config.MaxConcurrentCompilations > 0;

    internal static void WaitForIdle(bool cancelPending = false) =>
        Queue.WaitForIdle(cancelPending);

    private static bool Prefix(
        MyApiTarget target,
        string assemblyName,
        IEnumerable<Script> scripts,
        ref List<Message> diagnostics,
        string friendlyName,
        bool enableDebugInformation,
        bool trackMemoryUsage,
        ref Task<Assembly> __result
    )
    {
        if (
            !CompilationWorkers.Enabled
            || target != MyApiTarget.Ingame && target != MyApiTarget.Mod
        )
            return true;

        // The mod loader clears and reuses the file list as soon as the call returns.
        var source = scripts.ToArray();
        var messages = diagnostics = new List<Message>();
        var compiler = MyScriptCompiler.Static;
        __result = Queue.Submit(() =>
            compiler.Compile(
                target,
                assemblyName,
                source,
                messages,
                friendlyName,
                enableDebugInformation,
                trackMemoryUsage
            )
        );
        return false;
    }
}
