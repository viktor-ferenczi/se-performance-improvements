using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Shared.Config;
using Shared.Plugin;
using Shared.Stats;
using Shared.Tools;
using VRage.Utils;

namespace Shared.Patches
{
    // An assembler in cooperative mode looks for a master assembler on every production
    // tick (MyAssembler.GetMasterAssembler): a breadth-first walk over the whole conveyor
    // network under the global pathfinding lock, filtered down to the reachable assemblers
    // with a friendly owner, shuffled, then the first one with a non-empty queue wins.
    // On a large production grid the walk touches thousands of conveyor endpoints and the
    // lock is contended by the parallel item transfer computations, so the main thread
    // spends most of its time in this one method.
    //
    // The walk depends only on the conveyor network and block ownership, so its result is
    // cached per assembler and reused for a few seconds, or until anything changes a
    // conveyor network (MyGridConveyorSystemPatch.Generation). The random choice among
    // the candidates, and the checks on their queue and mode, still run on every call.
    [HarmonyPatch(typeof(MyAssembler))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class MyAssemblerPatch
    {
        private static IPluginConfig Config => Common.Config;

        // Assemblers reachable from one assembler, in the game's own filtering
        private sealed class Candidates
        {
            public long Generation;
            public readonly List<MyAssembler> Assemblers = new List<MyAssembler>();
        }

        // Five seconds of simulation, then the walk runs again
        private const int LifetimeTicks = 5 * 60;

        private static readonly Cache<long, Candidates> Cache = new Cache<long, Candidates>(
            60 * 60
        );
        private static readonly CacheStat Stat = new CacheStat();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update()
        {
            if (Config.FixConveyor)
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
                new CacheStatEntry(
                    "Conveyor.MasterAssembler",
                    sample.Lookups,
                    sample.Hits,
                    sample.Size
                )
            );
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetMasterAssembler")]
        [EnsureCode("606d2b92")]
        private static bool GetMasterAssemblerPrefix(
            MyAssembler __instance,
            ref MyAssembler __result,
            ref bool __state
        )
        {
            if (!Config.FixConveyor)
            {
                return true;
            }

            if (Statistics.Enabled)
            {
                Stat.CountLookup(Cache.Count);
            }

            if (
                !Cache.TryGetValue(__instance.EntityId, out var candidates)
                || candidates.Generation != MyGridConveyorSystemPatch.Generation
            )
            {
                // Let the game walk the network, the postfix captures the result
                __state = true;
                return true;
            }

            if (Statistics.Enabled)
            {
                Stat.CountHit();
            }

            __result = Pick(candidates.Assemblers);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetMasterAssembler")]
        [EnsureCode("606d2b92")]
        private static void GetMasterAssemblerPostfix(MyAssembler __instance, bool __state)
        {
            if (!__state)
            {
                return;
            }

            // The game leaves the reachable assemblers (all but the caller, friendly owners
            // only) in this static list. Its order is random, which does not matter here.
            var candidates = new Candidates { Generation = MyGridConveyorSystemPatch.Generation };
            foreach (var endpoint in MyAssembler.m_conveyorEndpoints)
            {
                if (endpoint.CubeBlock is MyAssembler assembler)
                {
                    candidates.Assemblers.Add(assembler);
                }
            }

            Cache.Store(__instance.EntityId, candidates, LifetimeTicks);
        }

        // Same choice as the game makes over its freshly walked list
        private static MyAssembler Pick(List<MyAssembler> assemblers)
        {
            assemblers.ShuffleList();

            foreach (var assembler in assemblers)
            {
                if (
                    !assembler.Closed
                    && !assembler.MarkedForClose
                    && !assembler.DisassembleEnabled
                    && !assembler.IsSlave
                    && !assembler.IsQueueEmpty
                )
                {
                    return assembler;
                }
            }

            return null;
        }
    }
}
