using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.Engine.Voxels;
using Shared.Tools;

namespace Shared.Patches
{
    // The other half of MySandboxGamePatchForVoxelPreload: drops the voxel storage loads the
    // startup preload asks for, and only those.
    //
    // Returning null is a value the preload already handles: LoadFromFile returns null for a
    // definition whose file is missing, and the preload's only use of the result is the
    // ResetDataCache() call on the IsMemoryLimited path, which the window excludes.
    //
    // The call shape is part of the condition. Every other caller in the game leaves logInfo at
    // its default, so a load which happens to run on another thread while the preload window is
    // open is left alone instead of relying on the window being exclusive.
    [HarmonyPatchCategory(PatchHelpers.EarlyCategory)]
    [HarmonyPatch(typeof(MyStorageBase))]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class MyStorageBasePatchForVoxelPreload
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MyStorageBase.LoadFromFile), typeof(string), typeof(Dictionary<byte, byte>), typeof(bool), typeof(bool))]
        [EnsureCode("425dfca1")]
        private static bool LoadFromFilePrefix(bool cache, bool logInfo, ref MyStorageBase __result)
        {
            if (logInfo || !cache || !MySandboxGamePatchForVoxelPreload.ShouldSkipStorageLoad())
                return true;

            __result = null;
            return false;
        }
    }
}
