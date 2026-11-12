using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.Game;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;

namespace Shared.Patches
{
    // Limits the programmable block time of each player per frame (ScriptPlayerQuota).
    //
    // Every run of a PB is timed, the whole ExecuteCode including the game's ownership walk
    // before the script, on whatever thread it runs: update frequency runs, timers, buttons, IGC,
    // TryRun (counted within the outer run) and the PB worker threads of the frame budget.
    // Between frames the runs are added up by the PB's owner, or its builder if it has no owner;
    // PBs with neither share one bucket. A player over ScriptPlayerHardLimitMs in a frame, or
    // over ScriptPlayerSoftLimitMs in PlayerQuota.SoftLimitFrames frames in a row, gets their
    // most expensive PB of that frame turned off, at most one per frame. Turning it off keeps the
    // script and its state, which survive a save and a cluster handover; the player turns it on
    // again. The PB's DetailedInfo says why, and the owner gets a chat message at most once a
    // minute. The Report mode only logs what Enforce would turn off.
    //
    // NPC identities are not limited, admins only with ScriptPlayerQuotaExemptAdmins off. The
    // hard limit is not checked in a frame with a gen 2 garbage collection, which stalls every
    // run in it. Each server or cluster node counts on its own.
    //
    // Needs the per-thread counter (FixScriptCounter), like the other PB limits.
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public static class ScriptQuota
    {
        private static IPluginConfig Config => Common.Config;

        private const int MessagePeriodTicks = 60 * 60;

        private static readonly PlayerQuota<MyProgrammableBlock> Quota =
            new PlayerQuota<MyProgrammableBlock>();

        private static readonly List<PlayerQuota<MyProgrammableBlock>.Offence> Offences =
            new List<PlayerQuota<MyProgrammableBlock>.Offence>();

        private static readonly Func<MyProgrammableBlock, long> OwnerOf = pb =>
            pb.OwnerId != 0 ? pb.OwnerId : pb.BuiltBy;

        private static readonly Func<long, bool> Exempt = IsExempt;

        private static readonly Action<MyProgrammableBlock, string> SetDetailedInfo =
            AccessTools.MethodDelegate<Action<MyProgrammableBlock, string>>(
                AccessTools.Method(typeof(MyProgrammableBlock), "SetDetailedInfo")
            );

        private static readonly OptionalPatch Patch = new OptionalPatch(
            typeof(QuotaPatch),
            () => Enabled
        );

        // Per owner: tick of the last chat message or log line, and the offences since
        private static readonly Dictionary<long, (long Tick, int Count)> Told =
            new Dictionary<long, (long, int)>();

        [ThreadStatic]
        private static int depth;

        private static int gen2Collections;

        public static bool Enabled =>
            ScriptRunner.Enabled && Config.ScriptPlayerQuota != ScriptPlayerQuotaMode.Off;

        // Called on every update, on the main thread between frames
        public static void Update()
        {
            Patch.Update();

            var gen2 = GC.CollectionCount(2);
            var checkHard = gen2 == gen2Collections;
            gen2Collections = gen2;

            if (!Enabled || !Sync.IsServer)
            {
                Quota.Clear();
                Told.Clear();
                return;
            }

            Quota.EndFrame(
                OwnerOf,
                Exempt,
                Ticks(Config.ScriptPlayerSoftLimitMs),
                Ticks(Config.ScriptPlayerHardLimitMs),
                checkHard,
                Offences
            );
            foreach (var offence in Offences)
                Act(offence);
            Offences.Clear();
        }

        private static long Ticks(float ms) =>
            (long)(Math.Max(ScriptRunner.MinLimitMs, ms) * Stopwatch.Frequency / 1000.0);

        private static string Ms(long ticks) => $"{1000.0 * ticks / Stopwatch.Frequency:0.0}";

        private static bool IsExempt(long identityId)
        {
            var players = MySession.Static?.Players;
            if (players == null || identityId == 0)
                return false;
            if (players.IdentityIsNpc(identityId))
                return true;
            if (!Config.ScriptPlayerQuotaExemptAdmins)
                return false;
            var steamId = players.TryGetSteamId(identityId);
            return steamId != 0 && MySession.Static.IsUserAdmin(steamId);
        }

        private static void Act(PlayerQuota<MyProgrammableBlock>.Offence offence)
        {
            var pb = offence.Item;
            if (pb.Closed || pb.MarkedForClose || !pb.Enabled)
                return;

            var limit = offence.Hard
                ? $"this server's {ScriptRunner.Ms(Config.ScriptPlayerHardLimitMs)} ms limit per player"
                : $"this server's {ScriptRunner.Ms(Config.ScriptPlayerSoftLimitMs)} ms limit per player in {PlayerQuota<MyProgrammableBlock>.SoftLimitFrames} frames in a row";
            var what =
                $"PB \"{pb.CustomName}\" ({pb.EntityId}) on \"{pb.CubeGrid?.DisplayName}\" of player {offence.Owner}: "
                + $"the player's PBs ran {Ms(offence.OwnerTicks)} ms in a frame, over {limit}; this PB {Ms(offence.ItemTicks)} ms";

            var enforce = Config.ScriptPlayerQuota == ScriptPlayerQuotaMode.Enforce;
            var tell = Tell(offence.Owner, out var count);
            if (!enforce)
            {
                if (tell)
                    Common.Logger.Info(
                        $"PB player quota would turn off {what} ({count} times in the last minute)"
                    );
                return;
            }

            pb.Enabled = false;
            Common.Logger.Info($"PB player quota turned off {what} (tick {Common.Plugin.Tick})");
            SetDetailedInfo(
                pb,
                $"Turned off by the server: the programmable blocks of this block's owner ran {Ms(offence.OwnerTicks)} ms in one frame, "
                    + $"over {limit}. This one ran the longest, {Ms(offence.ItemTicks)} ms. Make the scripts do less per run, then turn the block on again."
            );

            if (tell && offence.Owner != 0)
                MyVisualScriptLogicProvider.SendChatMessage(
                    $"Your programmable block \"{pb.CustomName}\" on \"{pb.CubeGrid?.DisplayName}\" was turned off: "
                        + $"your programmable blocks ran {Ms(offence.OwnerTicks)} ms in one frame, over {limit}.",
                    "Server",
                    offence.Owner,
                    "Red"
                );
        }

        // Whether the owner may be told again, at most once a minute; count is the offences since
        private static bool Tell(long owner, out int count)
        {
            var tick = Common.Plugin.Tick;
            Told.TryGetValue(owner, out var told);
            count = told.Count + 1;
            if (told.Tick != 0 && tick - told.Tick < MessagePeriodTicks)
            {
                Told[owner] = (told.Tick, count);
                return false;
            }

            Told[owner] = (tick, 0);
            return true;
        }

        // Applied only while the quota is on, see OptionalPatch
        [HarmonyPatch]
        private static class QuotaPatch
        {
            private static bool Prepare() => Patch.Prepare();

            // A run nested in another (TryRun) is part of the outer run's time
            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyProgrammableBlock), "ExecuteCode")]
            private static void ExecuteCodePrefix(out long __state)
            {
                __state = depth++ == 0 ? Stopwatch.GetTimestamp() : 0;
            }

            [HarmonyFinalizer]
            [HarmonyPatch(typeof(MyProgrammableBlock), "ExecuteCode")]
            private static void ExecuteCodeFinalizer(MyProgrammableBlock __instance, long __state)
            {
                depth--;
                if (__state != 0)
                    Quota.Add(__instance, Stopwatch.GetTimestamp() - __state);
            }
        }
    }
}
