using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;
using VRage.Platform.Windows.Sys;

namespace Shared.Patches
{
    // Decides how many worker threads the Havok physics job pool gets.
    //
    // MyPhysics.LoadData sizes the pool from the platform's advice:
    //
    //     int? optimalHavokThreadCount = MyVRage.Platform.System.OptimalHavokThreadCount;
    //     m_threadPool = optimalHavokThreadCount.HasValue
    //         ? new HkJobThreadPool(optimalHavokThreadCount.Value)
    //         : new HkJobThreadPool();
    //     m_jobQueue = new HkJobQueue(m_threadPool.ThreadCount + 1);
    //
    // MyWindowsSystem answers a hard `=> null`, so the parameterless constructor runs and
    // Havok sizes the pool from the machine on its own terms. That property is the game's
    // own extension point for the answer and MyPhysics.LoadData is the only place it is
    // read, so a postfix on it is the whole fix. The job queue follows, because its size is
    // derived from the pool's.
    //
    // The thread count is taken at world load, so a change needs a restart (or at least
    // reloading the world) to take effect.
    //
    // MINIMUM OF 2, NOT 1. A pool of one is not single threaded physics: the Havok worlds
    // are still created and initialized for multithreading (MyPhysics.CreateHkWorld passes
    // MyFakes.ENABLE_HAVOK_MULTITHREADING, HkWorld.InitMultithreading(m_threadPool,
    // m_jobQueue)) and still stepped through the multithreaded path (StepSimulation with
    // multithreaded: true, or InitMtStep / ProcessAllJobs / FinishMtStep on the job queue),
    // so one worker keeps every bit of the job dispatch and waiting and has nothing to
    // overlap it with. Physics without threading is a different switch entirely:
    // MyFakes.ENABLE_HAVOK_MULTITHREADING, which the game exposes to admins through
    // MyPhysics.SetScheduling and which also decides how the worlds themselves are created.
    // Havok's own way to run the multithreaded code path on the calling thread alone would
    // be a pool of zero, but HkJobThreadPool hands the count straight to the native library
    // (HkJobThreadPool_CreateWithNumThreads), so nothing on this side can establish what the
    // shipped build does with 0 or 1 without guessing. Two is the smallest count which is
    // certainly a working pool.
    //
    // THE UPPER END IS CAPPED BY HAVOK, NOT HERE. Measured on a 16 logical processor host,
    // asking for 16 and asking for 24 both produced 11 worker threads (HkThread_1..11 in a
    // thread dump), while the game's own sizing produced 7 and an explicit 3 produced 3. So
    // the shipped library has an internal maximum of its own - 11 workers plus the calling
    // thread - and a bigger number is neither an error nor an improvement, it simply stops
    // making a difference. The configuration allows up to 64 because that is a sanity limit
    // on the option, not a claim that the pool will be that size.
    [HarmonyPatch(typeof(MyWindowsSystem))]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class MyWindowsSystemPatch
    {
        private static IPluginConfig Config => Common.Config;

        // Zero leaves the game's own sizing alone
        private static int threadCount;

        private static bool subscribed;

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

            // The Game mode is the hands-off one: the getter keeps answering what the game
            // answers, so Havok sizes the pool exactly as it does without the plugin.
            if (!config.Enabled || config.HavokThreadCountMode == HavokThreadCountMode.Game)
            {
                threadCount = 0;
                return;
            }

            threadCount = config.HavokThreadCountMode == HavokThreadCountMode.Auto
                ? HavokThreads.Auto
                : Math.Max(HavokThreads.Min, Math.Min(HavokThreads.Max, config.HavokThreadCount));

            // Keep the configured number showing the effective one, so the in-game dialog and
            // the server's web UI display the count the game is going to get. In Auto that is
            // this machine's number; in Manual it only corrects a value outside the range.
            // The write raises PropertyChanged, which calls back into here once with the two
            // already equal.
            if (config.HavokThreadCount != threadCount)
                config.HavokThreadCount = threadCount;
        }

        private static void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            Configure();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyWindowsSystem.OptimalHavokThreadCount), MethodType.Getter)]
        [EnsureCode("fb445ec2")]
        private static void OptimalHavokThreadCountGetterPostfix(ref int? __result)
        {
            if (threadCount <= 0)
                return;

            __result = threadCount;

            // The property is read once per world load, so this is not a hot path.
            Common.Logger.Debug($"Havok physics thread count: {threadCount} ({Config.HavokThreadCountMode}); the pool may end up smaller, the shipped Havok build caps it");
        }
    }
}
