using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using HarmonyLib;
using Sandbox;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRage;

namespace Shared.Patches
{
    // Skips the eager preloading of the vanilla asteroid voxel storages on game start.
    //
    // MySandboxGame.PerformPreloading walks every VoxelMapStorage definition which can take
    // part in procedural asteroid generation and calls MyStorageBase.LoadFromFile on each,
    // purely to warm MyStorageBase's LRU cache. That loop costs roughly a gigabyte of managed
    // memory, before any world is loaded and whether or not the session will ever contain an
    // asteroid. It is paid by the client at startup and by the dedicated server as well.
    //
    // Skipping it is lazy versus eager and nothing else: LoadFromFile is itself the cache
    // lookup (a 512 entry LRU cache keyed by the file path plus the material modifiers), so a
    // caller which needs one of these storages later loads it then and gets the same shared
    // object the preload would have put there. The only cost is that the load happens on first
    // use instead of at startup. The game already has a weaker form of the same idea: the
    // preload calls ResetDataCache() on what it has just loaded when the platform reports
    // IsMemoryLimited, which is the same admission that holding all of them is optional.
    //
    // This class only marks the preload window; the loads themselves are dropped by
    // MyStorageBasePatchForVoxelPreload, so voxels a session actually needs still load
    // normally.
    [HarmonyPatchCategory(PatchHelpers.EarlyCategory)]
    [HarmonyPatch(typeof(MySandboxGame))]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public static class MySandboxGamePatchForVoxelPreload
    {
        private static IPluginLogger Log => Common.Logger;
        private static IPluginConfig Config => Common.Config;

        // Read once when the preload starts, since the preload is a startup-only event and
        // toggling the option later cannot undo or redo it
        private static bool enabled;

        // Set only while MySandboxGame.PerformPreloading is running. The preload does its
        // loading from a ParallelTasks.Parallel.For, so this is read from worker threads;
        // Parallel.For blocks until the loop is done, so the window closes only after the last
        // of them has finished.
        private static volatile bool preloading;

        // Number of storage loads dropped inside the window, logged when it closes. It is the
        // proof the fix did anything: the preload's call shape is what the prefix keys on, so a
        // zero here on a game version where Keen has changed that call is the visible symptom.
        private static int skipped;

        private static bool subscribed;

        // Runs from the early bootstrap on both sides, before the patches are applied, and again
        // from PatchHelpers.Configure once the plugin is fully initialized. Common is attached by
        // the early bootstrap, so the config is readable at both points.
        public static void Configure()
        {
            var config = Config;
            if (config == null)
                return;

            if (!subscribed)
            {
                config.PropertyChanged += OnConfigChanged;
                subscribed = true;
            }

            enabled = config.Enabled && config.SkipVoxelPreload;
        }

        private static void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            Configure();
        }

        // Called from MyStorageBasePatchForVoxelPreload for the loads the preload asks for
        internal static bool ShouldSkipStorageLoad()
        {
            if (!preloading)
                return false;

            Interlocked.Increment(ref skipped);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("PerformPreloading")]
        [EnsureCode("3c413d08")]
        private static void PerformPreloadingPrefix()
        {
            skipped = 0;

            // IsMemoryLimited is false on every platform the game ships, but if it were true
            // the preload would call ResetDataCache() on the loaded storage without a null
            // check, so a skipped load would be a NullReferenceException on a worker thread.
            // That platform is also the one where the preload already frees the voxel data it
            // has just loaded, so it has the least to gain here.
            preloading = enabled && !MyVRage.Platform.System.IsMemoryLimited;
        }

        // A finalizer, not a postfix: the preload swallows its own MyLoadingException, but
        // anything else would leave the flag set for the rest of the process, and a permanently
        // "preloading" game never loads a voxel storage again.
        [HarmonyFinalizer]
        [HarmonyPatch("PerformPreloading")]
        [EnsureCode("3c413d08")]
        private static void PerformPreloadingFinalizer()
        {
            if (preloading)
                Log.Debug($"Skipped the preloading of {skipped} vanilla voxel storages");

            preloading = false;
        }
    }
}
