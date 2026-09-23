using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;
using VRage.Platform.Windows.Sys;

namespace Shared.Patches
{
    // MyGeneralStats.Update runs once per frame and reads the process' private memory size
    // through MyVRage.Platform.System.ProcessPrivateMemory, which only feeds the statistics
    // log lines and the replication statistics. On Windows that is one GetProcessMemoryInfo
    // call; on Linux the compatibility layer answers it with Process.PrivateMemorySize64,
    // which creates a Process object and parses /proc/<pid>/stat and status on every call:
    // about half a millisecond per frame on the main thread, client and server alike.
    //
    // The value is refreshed at most once per second and served from the cache in between.
    [HarmonyPatch(typeof(MyWindowsSystem))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class MyWindowsSystemPatchForStats
    {
        private static IPluginConfig Config => Common.Config;

        private const long RefreshTicks = 60;

        private static long cached;
        private static long refreshAt = long.MinValue;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MyWindowsSystem.ProcessPrivateMemory), MethodType.Getter)]
        [EnsureCode("4d18b11a|4a3cb737")]
        private static bool ProcessPrivateMemoryGetterPrefix(ref long __result, ref bool __state)
        {
            if (!Config.FixMemoryStats)
            {
                return true;
            }

            if (Common.Plugin.Tick < refreshAt)
            {
                __result = cached;
                return false;
            }

            __state = true;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyWindowsSystem.ProcessPrivateMemory), MethodType.Getter)]
        [EnsureCode("4d18b11a|4a3cb737")]
        private static void ProcessPrivateMemoryGetterPostfix(long __result, bool __state)
        {
            if (!__state)
            {
                return;
            }

            cached = __result;
            refreshAt = Common.Plugin.Tick + RefreshTicks;
        }
    }
}
