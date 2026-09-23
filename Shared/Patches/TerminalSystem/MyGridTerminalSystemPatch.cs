using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using Shared.Config;
using Shared.Plugin;
using Shared.Stats;
using Shared.Tools;

namespace Shared.Patches
{
    // Before every run of a programmable block the game calls
    // MyGridTerminalSystem.UpdateGridBlocksOwnership with the block's owner, which sets
    // IsAccessibleForProgrammableBlock on every terminal block of the grid group from the
    // owner's access rights. On a large grid with a script running every tick that walk is
    // most of the main thread's simulation time (a 33000 block grid: 42%).
    //
    // The flags depend on the owner asked for, on the blocks of the terminal system, on
    // block ownership and share modes, on faction relations and on the admin settings. The
    // first three are tracked exactly: an entry per terminal system remembers the owner the
    // flags were last computed for and a generation counter bumped when a block joins or
    // leaves a terminal system or a block's ownership changes. A call with the same owner
    // and the same generation is skipped. Faction relation and admin setting changes are
    // not hooked, so an entry is also dropped after two seconds.
    //
    // Two programmable blocks with different owners on one grid still get a fresh walk each
    // (the flags cannot serve both), exactly as before.
    [HarmonyPatch]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class MyGridTerminalSystemPatch
    {
        private static IPluginConfig Config => Common.Config;

        private sealed class Applied
        {
            public long OwnerId;
            public long Generation;
            public long Expires;
        }

        private const int LifetimeTicks = 2 * 60;

        // Keyed by the terminal system itself, so a hash collision can never skip a walk
        private static readonly ConditionalWeakTable<MyGridTerminalSystem, Applied> Entries =
            new ConditionalWeakTable<MyGridTerminalSystem, Applied>();

        private static long generation;
        private static readonly CacheStat Stat = new CacheStat();
        private static int size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Invalidate()
        {
            Interlocked.Increment(ref generation);
        }

        public static void CaptureStatistics(StatisticsSnapshot snapshot)
        {
            var sample = Stat.Sample();
            snapshot.Caches.Add(
                new CacheStatEntry("Terminal.PbAccess", sample.Lookups, sample.Hits, sample.Size)
            );
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MyGridTerminalSystem), "UpdateGridBlocksOwnership")]
        [EnsureCode("78ecdbe6")]
        private static bool UpdateGridBlocksOwnershipPrefix(
            MyGridTerminalSystem __instance,
            long ownerID,
            ref Applied __state
        )
        {
            if (!Config.FixTerminal)
            {
                return true;
            }

            if (Statistics.Enabled)
            {
                Stat.CountLookup(size);
            }

            if (!Entries.TryGetValue(__instance, out var applied))
            {
                applied = new Applied();
                Entries.Add(__instance, applied);
                size++;
            }

            var tick = Common.Plugin.Tick;
            if (
                applied.OwnerId == ownerID
                && applied.Generation == Interlocked.Read(ref generation)
                && applied.Expires > tick
            )
            {
                if (Statistics.Enabled)
                {
                    Stat.CountHit();
                }

                return false;
            }

            __state = applied;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MyGridTerminalSystem), "UpdateGridBlocksOwnership")]
        [EnsureCode("78ecdbe6")]
        private static void UpdateGridBlocksOwnershipPostfix(long ownerID, Applied __state)
        {
            if (__state == null)
            {
                return;
            }

            __state.OwnerId = ownerID;
            __state.Generation = Interlocked.Read(ref generation);
            __state.Expires = Common.Plugin.Tick + LifetimeTicks;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MyGridTerminalSystem), "Add")]
        [EnsureCode("1ef65f21")]
        private static void AddPostfix()
        {
            Invalidate();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MyGridTerminalSystem), "Remove")]
        [EnsureCode("108ab732")]
        private static void RemovePostfix()
        {
            Invalidate();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MyCubeGrid), "NotifyBlockOwnershipChange")]
        [EnsureCode("19a731ee")]
        private static void NotifyBlockOwnershipChangePostfix()
        {
            Invalidate();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MyCubeGrid), "ChangeGridOwnership")]
        [EnsureCode("bcc83412")]
        private static void ChangeGridOwnershipPostfix()
        {
            Invalidate();
        }
    }
}
