using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Shared.Config;
using Shared.Plugin;
using VRage.Game.Entity;

namespace Shared.Patches
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [HarmonyPatch(typeof(MyEntity))]
    public static class MyEntityPatchForProjection
    {
        private static IPluginConfig Config => Common.Config;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyEntity.OnAddedToScene))]
        private static void OnAddedToScenePostfix(MyEntity __instance)
        {
            /* Disables functional blocks on preview grids without physics.
             * These are the projected functional blocks (MGP subgrids included),
             * because MyGridClipboard.ChangeClipboardPreview sets IsPreview and
             * CreatePhysics = false before the preview grid is added to the scene.
             *
             * Unfortunately we cannot test functionalBlock.CubeGrid?.Projector
             * here, because that is set after the preview grid is added to the scene.
             * Setting NeedsUpdate to NONE does not work here. Solving it differently
             * would have a constant CPU overhead, which should be avoided.
             *
             * The physics check alone is not enough: mods may spawn hidden grids
             * without physics, like Scope Framework does for its camera block.
             */
            if (
                Config.FixProjection
                && __instance is MyFunctionalBlock functionalBlock
                && functionalBlock.CubeGrid is { IsPreview: true, Physics: null }
            )
            {
                functionalBlock.Enabled = false;
            }
        }
    }
}
