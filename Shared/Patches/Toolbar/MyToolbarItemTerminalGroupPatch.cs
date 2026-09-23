using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Shared.Config;
using Shared.Plugin;
using Shared.Stats;
using Shared.Tools;
using VRage.Collections;

namespace Shared.Patches
{
    // A block group on the toolbar of a cockpit (MyToolbarItemTerminalGroup) is refreshed on
    // every frame while the player sits in it. The refresh collects the terminal actions
    // valid for the group (GetActionsWithGenericDuplicates), which walks every block in the
    // group and every component of every block. With groups of hundreds or thousands of
    // blocks that is a few milliseconds per frame for nothing, since the result only
    // changes when the group's membership changes.
    //
    // The collected actions are cached per toolbar item for a second, keyed by the block
    // list, so a change to the group is picked up at once and the rest of the refresh (the
    // enabled state, icons and the value text) still runs on every frame from the live
    // blocks. Client only: the dedicated server never updates toolbars.
    [HarmonyPatch(typeof(MyToolbarItemTerminalGroup))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class MyToolbarItemTerminalGroupPatch
    {
        private static IPluginConfig Config => Common.Config;

        private sealed class Actions
        {
            public ulong Fingerprint;
            public ListReader<ITerminalAction> List;
            public bool GenericType;
        }

        private const int LifetimeTicks = 60;

        // Keyed by the toolbar item's identity; a cockpit toolbar has at most a few dozen
        private static readonly Cache<int, Actions> Cache = new Cache<int, Actions>(10 * 60);
        private static readonly CacheStat Stat = new CacheStat();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update()
        {
            if (Config.FixToolbar)
            {
                Cache.Cleanup();
            }
            else if (Cache.Count != 0)
            {
                Cache.Clear();
            }
        }

        public static void CaptureStatistics(StatisticsSnapshot snapshot)
        {
            var sample = Stat.Sample();
            snapshot.Caches.Add(
                new CacheStatEntry("Toolbar.GroupActions", sample.Lookups, sample.Hits, sample.Size)
            );
        }

        // The block list is rebuilt by the game on every call, so the cache is keyed by its
        // content: the count and the entity ids in order
        private static ulong Fingerprint(ListReader<MyTerminalBlock> blocks)
        {
            var hash = 14695981039346656037ul ^ (ulong)blocks.Count;
            foreach (var block in blocks)
            {
                hash ^= (ulong)block.EntityId;
                hash *= 1099511628211ul;
            }

            return hash;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetActionsWithGenericDuplicates")]
        [EnsureCode("7a0b644a")]
        private static bool GetActionsWithGenericDuplicatesPrefix(
            MyToolbarItemTerminalGroup __instance,
            ListReader<MyTerminalBlock> blocks,
            ref bool genericType,
            ref ListReader<ITerminalAction> __result,
            ref (int, ulong) __state
        )
        {
            if (!Config.FixToolbar)
            {
                return true;
            }

            if (Statistics.Enabled)
            {
                Stat.CountLookup(Cache.Count);
            }

            var key = RuntimeHelpers.GetHashCode(__instance);
            var fingerprint = Fingerprint(blocks);

            if (Cache.TryGetValue(key, out var actions) && actions.Fingerprint == fingerprint)
            {
                if (Statistics.Enabled)
                {
                    Stat.CountHit();
                }

                genericType = actions.GenericType;
                __result = actions.List;
                return false;
            }

            __state = (key, fingerprint);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetActionsWithGenericDuplicates")]
        [EnsureCode("7a0b644a")]
        private static void GetActionsWithGenericDuplicatesPostfix(
            bool genericType,
            ListReader<ITerminalAction> __result,
            (int, ulong) __state
        )
        {
            if (__state.Item1 == 0)
            {
                return;
            }

            Cache.Store(
                __state.Item1,
                new Actions
                {
                    Fingerprint = __state.Item2,
                    List = __result,
                    GenericType = genericType,
                },
                LifetimeTicks
            );
        }
    }
}
