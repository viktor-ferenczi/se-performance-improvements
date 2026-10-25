using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Multiplayer;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;

namespace Shared.Patches
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyWheel))]
    public static class MyWheelPatch
    {
        private static IPluginConfig Config => Common.Config;

        // Applied only while the option is on, see OptionalPatch
        private static readonly OptionalPatch Optional = new OptionalPatch(
            typeof(MyWheelPatch),
            () => Config.FixWheelTrail
        );

        private static bool Prepare() => Optional.Prepare();

        public static void Update() => Optional.Update();

        [HarmonyPrefix]
        [HarmonyPatch("CheckTrail")]
        [EnsureCode("b3d278e9")]
        private static bool CheckTrailPrefix()
        {
            if (!Sync.IsDedicated)
                return true;

            return !Config.FixWheelTrail;
        }
    }
}