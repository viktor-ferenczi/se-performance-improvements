using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using Sandbox.Game.GameSystems.IntergridCommunication;
using Sandbox.Game.SessionComponents;

namespace Shared.Patches;

// Intergrid communication for scripts off the main thread (StandIn.Igc).
//
// Each PB has its own context, and a listener's messages are queued by the main thread before
// the simulation, so accepting a message touches only the PB's own listener. What all PBs share
// is the session component: the contexts by PB, the active broadcast listeners, the contexts
// with pending callbacks and the messages for the next tick, plain collections, so in the
// parallel phase they are used under one lock. Whether a unicast target is reachable is worked
// out over the antenna relays with scratch sets of the antenna system, under
// ScriptStandIns.AntennaLock; the result is cached per context and frame, so only a miss pays.
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class IgcStandIn
{
    private static readonly object SessionLock = new object();

    [HarmonyPatch]
    private static class SessionPatch
    {
        private static bool Prepare() => ScriptRunner.CounterPerThread;

        private static IEnumerable<MethodBase> TargetMethods()
        {
            var type = typeof(MyIGCSystemSessionComponent);
            yield return AccessTools.Method(
                type,
                nameof(MyIGCSystemSessionComponent.GetContextForPB)
            );
            yield return AccessTools.Method(
                type,
                nameof(MyIGCSystemSessionComponent.GetOrMakeContextFor)
            );
            yield return AccessTools.Method(
                type,
                nameof(MyIGCSystemSessionComponent.RegisterBroadcastListener)
            );
            yield return AccessTools.Method(
                type,
                nameof(MyIGCSystemSessionComponent.UnregisterBroadcastListener)
            );
            yield return AccessTools.Method(
                type,
                nameof(MyIGCSystemSessionComponent.RegisterContextWithPendingCallbacks)
            );
            yield return AccessTools.Method(
                type,
                nameof(MyIGCSystemSessionComponent.UnregisterContextWithPendingCallbacks)
            );
            yield return AccessTools.Method(
                type,
                nameof(MyIGCSystemSessionComponent.EnqueueMessage)
            );
        }

        private static void Prefix(out bool __state) => __state = ScriptStandIns.Enter(SessionLock);

        private static void Finalizer(bool __state) => ScriptStandIns.Exit(SessionLock, __state);
    }

    [HarmonyPatch(typeof(MyIntergridCommunicationContext), "EvaluateConnectionTo")]
    private static class ConnectionPatch
    {
        private static bool Prepare() => ScriptRunner.CounterPerThread;

        private static void Prefix(out bool __state) =>
            __state = ScriptStandIns.Enter(ScriptStandIns.AntennaLock);

        private static void Finalizer(bool __state) =>
            ScriptStandIns.Exit(ScriptStandIns.AntennaLock, __state);
    }
}
