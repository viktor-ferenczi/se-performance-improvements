using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Shared.Logging;
using Shared.Plugin;
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
            return VerifyAndApply(log, handleExceptions,
                EnsureCode.Verify,
                () => harmony.PatchAll(Assembly.GetExecutingAssembly()),
                "all patches");
        }

        // Applies the uncategorized patches: everything except the deferred "Late" category. Used
        // by the dedicated server's early bootstrap, before world-load compilation. Patches whose
        // target assembly is not loaded yet carry a category and are applied later from Init.
        public static bool HarmonyPatchUncategorized(IPluginLogger log, Harmony harmony, bool handleExceptions = true)
        {
            return VerifyAndApply(log, handleExceptions,
                EnsureCode.VerifyUncategorized,
                () => harmony.PatchAllUncategorized(Assembly.GetExecutingAssembly()),
                "uncategorized patches");
        }

        // Applies only the patches in the given category, verifying only those first.
        public static bool HarmonyPatchCategory(IPluginLogger log, Harmony harmony, string category, bool handleExceptions = true)
        {
            return VerifyAndApply(log, handleExceptions,
                () => EnsureCode.VerifyCategory(category),
                () => harmony.PatchCategory(Assembly.GetExecutingAssembly(), category),
                $"category '{category}'");
        }

        // Shared scaffold: verify the targeted game methods still match, then apply the patches.
        private static bool VerifyAndApply(IPluginLogger log, bool handleExceptions, Func<IEnumerable<CodeChange>> verify, Action apply, string what)
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
            try
            {
                apply();
            }
            catch (Exception ex)
            {
                log.Critical(ex, "Failed to apply Harmony patches");
                return false;
            }

            return true;
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

#if DEBUG
            const int period = 10 * 60; // Ticks
            if (Common.Plugin.Tick % period == 0)
            {
                var log = Common.Plugin.Log;
                log.Info("Cache hit rates:");
                log.Info($"- MySafeZonePatch IsSafe: {MySafeZonePatch.IsSafeCacheReport}");
                log.Info($"- MySafeZonePatch IsActionAllowed: {MySafeZonePatch.IsActionAllowedCacheReport}");
                log.Info($"- MySessionComponentSafeZonesPatch: {MySessionComponentSafeZonesPatch.CacheReport}");
                log.Info($"- MyWindTurbinePatch: {MyWindTurbinePatch.CacheReport}");
                // log.Info($"- MyPathFindingSystemPatch: {MyPathFindingSystemPatch.Report(period)}");
                // log.Info($"- MyPathFindingSystemEnumeratorPatch: {MyPathFindingSystemEnumeratorPatch.Report(period)}");
                log.Info($"- MyGridConveyorSystemPatch: {MyGridConveyorSystemPatch.PullItemReports}");
                foreach (var report in MyGridConveyorSystemPatch.CacheReports)
                {
                    log.Info($"- MyGridConveyorSystemPatch: {report}");
                }
                // log.Info($"- MyLargeTurretTargetingSystemPatch VisibilityCache: {MyLargeTurretTargetingSystemPatch.VisibilityCacheReport}");
                // log.Info($"- MyCubeBlockPatch: {MyCubeBlockPatch.CacheReport}");
                // log.Info($"- MyTerminalBlockPatch: {MyTerminalBlockPatch.CacheReport}");
                // log.Info($"- MyGridTerminalSystemPatch: {MyGridTerminalSystemPatch.InhibitorReport}");
            }
#endif
        }
    }
}