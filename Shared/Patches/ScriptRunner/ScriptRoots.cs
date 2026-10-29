using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using HarmonyLib;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI.Ingame;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;

namespace Shared.Patches
{
    // Hands scripts that can run off the main thread proxies instead of the game objects behind
    // their roots: GridTerminalSystem, Me, Runtime and IGC (FixScriptProxies, SE-626 stage 3).
    //
    // Everything a script reaches in the game hangs off these roots, so through the proxies every
    // game call of the script goes through its ScriptPolicy. On the main thread the proxies just
    // call the game; they matter once the parallel phase runs scripts on workers.
    //
    // Whether a script gets proxies is decided once per script instance, when the PB creates it,
    // from the option and from ScriptEligibility. A script never mixes proxies and game objects,
    // which would break comparisons like block == Me. So a change of the option applies to
    // scripts instantiated afterwards (after a recompile or a world load).
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public static class ScriptRoots
    {
        private static IPluginConfig Config => Common.Config;

        private static readonly string[] ScriptNamespaces =
        {
            "Sandbox.ModAPI.Ingame",
            "SpaceEngineers.Game.ModAPI.Ingame",
            "VRage.Game.ModAPI.Ingame",
            "Sandbox.ModAPI.Interfaces",
        };

        private sealed class Decision
        {
            public bool Proxied;
            public bool UsesIgc;

            // Why not proxied: null if the proxies were off when the script was created
            public string[] Blocking;

            // The mechanisms a proxied script needs off the main thread, see ScriptEligibility
            public StandIn Needs;
            public string[] Needing;

            // The proxied IGC getter, see IgcPrefix
            public Func<IMyIntergridCommunicationSystem> Igc;

            public bool TypesReady;

            // The game made the PB's IGC context, see Prepare
            public bool IgcReady;
        }

        private static readonly ConditionalWeakTable<MyGridProgram, Decision> Decisions =
            new ConditionalWeakTable<MyGridProgram, Decision>();

        public static bool Enabled => Config.Enabled && Config.FixScriptProxies;

        private static readonly OptionalPatch DisplayName = new OptionalPatch(
            typeof(DisplayNamePatch),
            () => Enabled
        );

        public static void Update()
        {
            DisplayName.Update();
        }

        public static void Configure()
        {
            ScriptProxies.IsScriptInterface = t => ScriptNamespaces.Contains(t.Namespace);
            ScriptPolicy.LoadDefault();
            Common.Logger.Debug($"PB script policy: {ScriptPolicy.Count} members");
        }

        // True if this script instance runs with proxies behind its roots
        public static bool IsProxied(Sandbox.ModAPI.IMyGridProgram program) =>
            program is MyGridProgram p && Decisions.TryGetValue(p, out var d) && d.Proxied;

        // Where a script may run now: -1 on the main thread for good (no proxies), otherwise the
        // mechanisms it needs that are off (StandIn), 0 if none, when it may run off the main thread
        public static int State(Sandbox.ModAPI.IMyGridProgram program)
        {
            if (!(program is MyGridProgram p) || !Decisions.TryGetValue(p, out var d) || !d.Proxied)
                return -1;
            return (int)(d.Needs & ~ScriptStandIns.Active);
        }

        // Whether a script's first use of IGC off the main thread is safe: its context was made
        // on the main thread, or the IGC stand-in serializes making it
        public static bool Prepared(Sandbox.ModAPI.IMyGridProgram program) =>
            !(program is MyGridProgram p)
            || !Decisions.TryGetValue(p, out var d)
            || !d.UsesIgc
            || d.IgcReady
            || (ScriptStandIns.Active & StandIn.Igc) != 0;

        // The reason behind State, for ScriptLogThreadDecisions
        public static string Explain(Sandbox.ModAPI.IMyGridProgram program)
        {
            if (!(program is MyGridProgram p) || !Decisions.TryGetValue(p, out var d))
                return "main thread: created before PB proxies were on";
            if (!d.Proxied)
                return d.Blocking == null
                    ? "main thread: created while PB proxies were off"
                    : $"main thread: calls {string.Join(", ", d.Blocking)}";
            var uses = d.Needing.Length == 0 ? "" : $" ({string.Join("; ", d.Needing)})";
            var missing = d.Needs & ~ScriptStandIns.Active;
            if (missing != StandIn.None)
                return $"main thread: needs {ScriptStandIns.Names(missing)}, which are off{uses}";
            return d.Needs == StandIn.None
                ? "off the main thread"
                : $"off the main thread, with {ScriptStandIns.Names(d.Needs)}{uses}";
        }

        // On the main thread, before a proxied script runs off it. The game creates the IGC
        // context of a PB on first use, in a dictionary shared by all PBs. The proxy types of the
        // classes the script will most likely meet are made here once, as making one takes long
        // enough to push a script's first runs on a worker over the run limits.
        public static void Prepare(Sandbox.ModAPI.IMyGridProgram program, MyProgrammableBlock pb)
        {
            if (!(program is MyGridProgram p) || !Decisions.TryGetValue(p, out var d))
                return;
            if (d.UsesIgc && d.Igc != null)
            {
                d.Igc();
                d.IgcReady = true;
            }
            if (d.TypesReady)
                return;
            d.TypesReady = true;

            var types = new HashSet<Type> { pb.CubeGrid.GetType(), pb.SlimBlock.GetType() };
            var blocks = MyCubeGridGroups
                .Static.Logical.GetGroup(pb.CubeGrid)
                ?.GroupData.TerminalSystem?.Blocks;
            if (blocks != null)
            {
                foreach (var block in blocks)
                {
                    types.Add(block.GetType());
                    for (var i = 0; i < block.InventoryCount; i++)
                        types.Add(MyEntityExtensions.GetInventory(block, i)?.GetType());
                    if (block is IMyTextSurfaceProvider surfaces)
                        for (var i = 0; i < surfaces.SurfaceCount; i++)
                            types.Add(surfaces.GetSurface(i)?.GetType());
                }
            }
            foreach (var type in types)
                if (type != null)
                    ScriptProxies.Prepare(type);
        }

        private static bool Decide(MyGridProgram program) => DecisionOf(program).Proxied;

        private static Decision DecisionOf(MyGridProgram program) =>
            Decisions.GetValue(
                program,
                p =>
                {
                    if (!Enabled)
                        return new Decision();
                    var result = ScriptEligibility.Of(p.GetType().Assembly);
                    if (!result.OffMain)
                        Common.Logger.Debug(
                            $"PB script stays on the main thread: {string.Join(", ", result.Blocking)}"
                        );
                    return new Decision
                    {
                        Proxied = result.OffMain,
                        UsesIgc = result.UsesIgc,
                        Blocking = result.Blocking,
                        Needs = result.Needs,
                        Needing = result.Needing,
                    };
                }
            );

        // The display name of a block definition with a variant is built in a StringBuilder of the
        // definition, shared by every block of that type. Scripts read it through CustomName and
        // DefinitionDisplayNameText, from workers of different grid groups at once.
        [HarmonyPatch(
            typeof(MyCubeBlockDefinition),
            nameof(MyCubeBlockDefinition.DisplayNameText),
            MethodType.Getter
        )]
        private static class DisplayNamePatch
        {
            private static bool Prepare() => DisplayName.Prepare();

            private static void Prefix(MyCubeBlockDefinition __instance) =>
                Monitor.Enter(__instance);

            private static void Finalizer(MyCubeBlockDefinition __instance) =>
                Monitor.Exit(__instance);
        }

        // Applied always: the setters run when a script is created and twice per run, and a
        // script given proxies needs them on every later run, whatever the option says by then
        [HarmonyPatch]
        private static class RootsPatch
        {
            private const string Prefix = "Sandbox.ModAPI.IMyGridProgram.set_";

            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyGridProgram), Prefix + "Runtime")]
            private static void RuntimePrefix(
                MyGridProgram __instance,
                ref IMyGridProgramRuntimeInfo value
            )
            {
                if (Decide(__instance))
                    value = ScriptProxies.WrapAs<IMyGridProgramRuntimeInfo>(value);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyGridProgram), Prefix + "Me")]
            private static void MePrefix(MyGridProgram __instance, ref IMyProgrammableBlock value)
            {
                if (Decide(__instance))
                    value = ScriptProxies.WrapAs<IMyProgrammableBlock>(value);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyGridProgram), Prefix + "GridTerminalSystem")]
            private static void GridTerminalSystemPrefix(
                MyGridProgram __instance,
                ref IMyGridTerminalSystem value
            )
            {
                if (value != null && Decide(__instance))
                    value = ScriptProxies.WrapAs<IMyGridTerminalSystem>(value);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(MyGridProgram), Prefix + "IGC_ContextGetter")]
            private static void IgcPrefix(
                MyGridProgram __instance,
                ref Func<IMyIntergridCommunicationSystem> value
            )
            {
                var decision = DecisionOf(__instance);
                if (value == null || !decision.Proxied)
                    return;
                var getter = value;
                IMyIntergridCommunicationSystem proxy = null;
                value = () =>
                    proxy ??= ScriptProxies.WrapAs<IMyIntergridCommunicationSystem>(getter());
                decision.Igc = value;
            }
        }
    }
}
