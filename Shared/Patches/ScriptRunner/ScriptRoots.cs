using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using HarmonyLib;
using Sandbox.Definitions;
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

        private static bool Decide(MyGridProgram program) =>
            Decisions
                .GetValue(
                    program,
                    p =>
                    {
                        var proxied = Enabled;
                        if (proxied)
                        {
                            var result = ScriptEligibility.Of(p.GetType().Assembly);
                            proxied = result.OffMain;
                            if (!proxied)
                                Common.Logger.Debug(
                                    $"PB script stays on the main thread: {string.Join(", ", result.Blocking)}"
                                );
                        }
                        return new Decision { Proxied = proxied };
                    }
                )
                .Proxied;

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
                if (value == null || !Decide(__instance))
                    return;
                var getter = value;
                value = () => ScriptProxies.WrapAs<IMyIntergridCommunicationSystem>(getter());
            }
        }
    }
}
