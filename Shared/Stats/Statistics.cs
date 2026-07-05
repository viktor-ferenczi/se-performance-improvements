using System;
using System.ComponentModel;
using Shared.Patches;
using Shared.Plugin;

namespace Shared.Stats
{
    // Central switch and driver for the plugin's runtime statistics.
    //
    // The counters live in the patch classes and their caches; this class decides
    // whether they are collected at all (Enabled), periodically reads them into a
    // host-agnostic StatisticsSnapshot (Capture), and hands each snapshot to the
    // host's consumer (Publisher). PatchHelpers.PatchUpdates drives the period.
    //
    // Kept free of any PluginSdk dependency: the dedicated server plugs in a
    // Publisher that forwards to the PluginSdk statistics API; the client leaves it
    // null for now (it collects nothing in a release build until a consumer exists).
    public static class Statistics
    {
        // How often (in simulation ticks) statistics are captured and delivered.
        public const int PeriodTicks = 10 * 60;

        // Runtime collection gate, read on the cache lookup hot path, so it is a plain
        // static field rather than a config lookup. Kept in sync with the
        // CollectStatistics config option (and with whether a consumer exists) by
        // RefreshEnabled.
        public static bool Enabled;

        // Optional consumer of each captured snapshot. Set by the host (the dedicated
        // server sets it to PerformanceStats.Publish); null on the client for now.
        public static Action<StatisticsSnapshot> Publisher
        {
            get;
            set
            {
                field = value;
                RefreshEnabled();
            }
        }

        // Wires up config-change tracking and computes the initial Enabled state.
        // Called from PatchHelpers.Configure once the shared config is available.
        public static void Configure()
        {
            var config = Common.Config;
            if (config != null)
            {
                config.PropertyChanged -= OnConfigChanged;
                config.PropertyChanged += OnConfigChanged;
            }

            RefreshEnabled();
        }

        private static void OnConfigChanged(object sender, PropertyChangedEventArgs e) => RefreshEnabled();

        private static void RefreshEnabled()
        {
            var config = Common.Config;
            var enabled = config != null && config.CollectStatistics;

#if !DEBUG
            // In a release build there is no log consumer, so only collect when a host
            // actually publishes the snapshots — the client then pays nothing until it
            // ships a PluginSdk consumer. In a debug build the stats are always logged.
            enabled = enabled && Publisher != null;
#endif

            Enabled = enabled;
        }

        // Reads the live counters into a self-contained snapshot, resetting them. Called
        // once per period so a single reader owns the reset (the DEBUG log and the
        // Publisher both consume the one returned snapshot — no double read).
        public static StatisticsSnapshot Capture()
        {
            var snapshot = new StatisticsSnapshot();

            MySafeZonePatch.CaptureStatistics(snapshot);
            MySessionComponentSafeZonesPatch.CaptureStatistics(snapshot);
            MyWindTurbinePatch.CaptureStatistics(snapshot);
            MyGridConveyorSystemPatch.CaptureStatistics(snapshot);

            return snapshot;
        }
    }
}
