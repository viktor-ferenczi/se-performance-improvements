using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace Shared.Patches;

// The inventory helpers of TerminalBlockExtentions for scripts with proxies (StandIn.Inventory).
//
// They are static methods taking the block, and test it for game classes (MyEntity,
// IMyInventoryOwner), which a proxy is not. So while the proxies are on, they get the game
// object behind a proxy, and an inventory they return to a script with a proxy becomes a proxy
// too. This keeps them working on the main thread as well. On a worker thread they only read,
// except SetUseConveyorSystem, a synced value, which is set on the main thread after the phase.
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class InventoryStandIn
{
    private static readonly OptionalPatch Patch = new OptionalPatch(
        typeof(ProxyPatch),
        () => ScriptRoots.Enabled
    );

    public static void Update() => Patch.Update();

    [HarmonyPatch(typeof(TerminalBlockExtentions))]
    private static class ProxyPatch
    {
        private static bool Prepare() => Patch.Prepare();

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TerminalBlockExtentions.HasInventory))]
        private static void HasInventoryPrefix(ref IMyTerminalBlock block) => Unwrap(ref block);

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TerminalBlockExtentions.GetInventoryCount))]
        private static void GetInventoryCountPrefix(ref IMyTerminalBlock block) =>
            Unwrap(ref block);

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TerminalBlockExtentions.GetUseConveyorSystem))]
        private static void GetUseConveyorSystemPrefix(ref IMyTerminalBlock block) =>
            Unwrap(ref block);

        private static void Unwrap(ref IMyTerminalBlock block) =>
            block = (IMyTerminalBlock)ScriptProxies.Unwrap(block);

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TerminalBlockExtentions.GetInventory))]
        private static void GetInventoryPrefix(ref IMyTerminalBlock block, out bool __state)
        {
            __state = block is ProxyBase;
            block = (IMyTerminalBlock)ScriptProxies.Unwrap(block);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(TerminalBlockExtentions.GetInventory))]
        private static void GetInventoryPostfix(ref IMyInventory __result, bool __state)
        {
            if (__state)
                __result = ScriptProxies.WrapAs<IMyInventory>(__result);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TerminalBlockExtentions.SetUseConveyorSystem))]
        private static bool SetUseConveyorSystemPrefix(ref IMyTerminalBlock block, bool use)
        {
            block = (IMyTerminalBlock)ScriptProxies.Unwrap(block);
            if (!ScriptWrites.Active)
                return true;
            var target = block;
            ScriptWrites.Later(() => target.SetUseConveyorSystem(use));
            return false;
        }
    }
}
