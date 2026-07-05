using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Shared.Logging;
using Shared.Plugin;
using Shared.Stats;
using Shared.Tools;

namespace Shared.Patches
{
    // ReSharper disable once UnusedType.Global
    public static class PatchHelpers
    {
        // Harmony patch category for patches that must be deferred to IPlugin.Init because their
        // target type lives in an assembly that is not loaded yet at the dedicated server's early
        // bootstrap point (e.g. VRage.EOS). Every patch without this category is applied early.
        public const string LateCategory = "Late";

        public static bool HarmonyPatchAll(IPluginLogger log, Harmony harmony, bool handleExceptions = true)
        {
            return VerifyAndApply(log, harmony, handleExceptions,
                EnsureCode.Verify,
                () => harmony.PatchAll(Assembly.GetExecutingAssembly()),
                "all patches");
        }

        // Applies the uncategorized patches: everything except the deferred "Late" category. Used
        // by the dedicated server's early bootstrap, before world-load compilation. Patches whose
        // target assembly is not loaded yet carry a category and are applied later from Init.
        public static bool HarmonyPatchUncategorized(IPluginLogger log, Harmony harmony, bool handleExceptions = true)
        {
            return VerifyAndApply(log, harmony, handleExceptions,
                EnsureCode.VerifyUncategorized,
                () => harmony.PatchAllUncategorized(Assembly.GetExecutingAssembly()),
                "uncategorized patches");
        }

        // Applies only the patches in the given category, verifying only those first.
        public static bool HarmonyPatchCategory(IPluginLogger log, Harmony harmony, string category, bool handleExceptions = true)
        {
            return VerifyAndApply(log, harmony, handleExceptions,
                () => EnsureCode.VerifyCategory(category),
                () => harmony.PatchCategory(Assembly.GetExecutingAssembly(), category),
                $"category '{category}'");
        }

        // Shared scaffold: verify the targeted game methods still match, then apply the patches.
        private static bool VerifyAndApply(IPluginLogger log, Harmony harmony, bool handleExceptions, Func<IEnumerable<CodeChange>> verify, Action apply, string what)
        {
            log.Debug($"Scanning for conflicting code changes ({what})");
            var throwOnFailedVerification = !handleExceptions || Environment.GetEnvironmentVariable("SE_PLUGIN_THROW_ON_FAILED_METHOD_VERIFICATION") != null;
            try
            {
                var codeChanges = verify().ToList();
                if (codeChanges.Count != 0)
                {
                    log.Critical("Detected conflicting code changes:");
                    foreach (var codeChange in codeChanges)
                        log.Info(codeChange.ToString());

                    if (throwOnFailedVerification)
                    {
                        throw new Exception("Detected conflicting code changes");
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to scan for conflicting code changes");

                if (throwOnFailedVerification)
                {
                    throw;
                }

                return false;
            }

            log.Debug($"Applying Harmony patches ({what})");

            // Snapshot the methods this Harmony id has already patched so that, after applying, we
            // can report exactly the ones this phase added. GetPatchedMethods() is scoped to
            // harmony.Id, but the dedicated server applies patches in two phases under the same id
            // (uncategorized early, then the "Late" category from Init), so a before/after delta
            // isolates the current phase.
            var before = new HashSet<MethodBase>(harmony.GetPatchedMethods());
            try
            {
                apply();
            }
            catch (Exception ex)
            {
                log.Critical(ex, "Failed to apply Harmony patches");
                return false;
            }

            if (log.IsDebugEnabled)
            {
                LogAppliedPatches(log, harmony, before, what);
            }
            
            return true;
        }

        // Proof that the patches were applied: debug-log every game method this phase patched (each
        // with the plugin patch class targeting it) with a running count, then an info line with
        // the total. A test run can be verified from the log — enable all fixes and confirm every
        // expected patch appears. Note the applied set is fixed at build time, not by config: the
        // Fix* flags gate behavior inside the patch bodies, not whether a patch is applied, so this
        // count is the same regardless of which fixes are enabled.
        private static void LogAppliedPatches(IPluginLogger log, Harmony harmony, HashSet<MethodBase> before, string what)
        {
            var applied = harmony.GetPatchedMethods()
                .Where(method => !before.Contains(method))
                .OrderBy(method => method.DeclaringType?.FullName, StringComparer.Ordinal)
                .ThenBy(method => method.ToString(), StringComparer.Ordinal)
                .ToList();

            var count = 0;
            foreach (var method in applied)
                log.Debug($"Patch applied #{++count}: {DescribePatchedMethod(harmony, method)}");

            log.Debug($"Applied {count} {(count == 1 ? "patch" : "patches")} ({what})");
        }

        // Renders a patched game method as "Namespace.Type.Method(argTypes) <- PatchClass[, ...]",
        // naming the plugin patch classes (filtered to this Harmony id) whose prefix/postfix/
        // transpiler/finalizer targets it.
        private static string DescribePatchedMethod(Harmony harmony, MethodBase method)
        {
            var parameters = string.Join(", ", method.GetParameters().Select(parameter => parameter.ParameterType.Name));
            var target = $"{method.DeclaringType?.FullName}.{method.Name}({parameters})";

            var info = Harmony.GetPatchInfo(method);
            if (info == null)
                return target;

            var patchClasses = info.Prefixes
                .Concat(info.Postfixes)
                .Concat(info.Transpilers)
                .Concat(info.Finalizers)
                .Where(patch => patch.owner == harmony.Id)
                .Select(patch => patch.PatchMethod.DeclaringType?.Name)
                .Distinct()
                .ToList();

            return patchClasses.Count == 0 ? target : $"{target} <- {string.Join(", ", patchClasses)}";
        }

        // Called after loading configuration, but before patching
        public static void Configure()
        {
            MyScriptCompilerPatch.Configure();
            MySafeZonePatch.Configure();
            MySessionComponentSafeZonesPatch.Configure();
            MyPhysicsPatch.Configure();
            MyPhysicsBodyPatch.Configure();
            MyClusterTreePatch.Configure();
            MyCharacterPatch.Configure();
            MyStorageExtensionsPatch.Configure();
            MyWindTurbinePatch.Configure();
            MyDefinitionIdToStringPatch.Configure();
            // MyCubeBlockPatch.Configure();
            // MyTerminalBlockPatch.Configure();
            // MyGridTerminalSystemPatch.Configure();

            Statistics.Configure();

            // FIXME: Make this configurable!
            // PhysicsFixes.SetClusterSize(3000f);
        }

        // Called on every update
        public static void PatchUpdates()
        {
            MyDefinitionIdToStringPatch.Update();
            MySafeZonePatch.Update();
            MySessionComponentSafeZonesPatch.Update();
            MyWindTurbinePatch.Update();
            MyGridConveyorSystemPatch.Update();
            // MyCubeBlockPatch.Update();
            // MyTerminalBlockPatch.Update();
            // MyGridTerminalSystemPatch.Update();

            // Capture and deliver the runtime statistics once per period. This runs
            // regardless of the log level (formerly a DEBUG-only log); the dedicated
            // server forwards each snapshot to the PluginSdk statistics API through the
            // Publisher, while a debug build additionally logs it. Gated by the
            // CollectStatistics config option via Statistics.Enabled.
            if (Statistics.Enabled && Common.Plugin.Tick % Statistics.PeriodTicks == 0)
            {
                var snapshot = Statistics.Capture();
                Statistics.Publisher?.Invoke(snapshot);
#if DEBUG
                LogStatistics(snapshot);
#endif
            }
        }

#if DEBUG
        // Logs the captured statistics, preserving the former DEBUG cache-hit-rate dump.
        private static void LogStatistics(StatisticsSnapshot snapshot)
        {
            var log = Common.Plugin.Log;
            log.Info("Cache hit rates:");
            foreach (var cache in snapshot.Caches)
                log.Info($"- {cache.Name}: HitRate = {cache.HitRatePercent:0.000}% = {cache.Hits}/{cache.Lookups}; ItemCount = {cache.Size}");
            log.Info($"- Conveyor pull calls: PullItem {snapshot.PullItem}; PullItems {snapshot.PullItems}");
        }
#endif
    }
}