using System;
using System.Collections.Generic;
using System.Linq;
using PluginSdk.Stats;
using Shared.Stats;

namespace ServerPlugin.Stats;

// Publishes the plugin's runtime statistics through the Magnetar PluginSdk
// statistics API, so a consumer (the Quasar Agent) can collect and chart them.
//
// It consumes only the host-agnostic Shared.Stats.StatisticsSnapshot and depends
// on nothing else in ServerPlugin, so once the client also ships the PluginSdk
// this file can be moved into Shared verbatim and wired up from the client too.
// For now it lives in ServerPlugin and is invoked only from the server
// (Plugin.EarlyStartup sets Statistics.Publisher = PerformanceStats.Publish).
public static class PerformanceStats
{
    // Provider name the snapshot is published under; a consumer pulls it by this name.
    private const string ProviderName = "Performance";

    // Schemas are cached per POCO type by StatsSchema.Build; hold the references so
    // each publish only walks the cached property list.
    private static readonly StatsSchema CacheSchema = StatsSchema.Build(typeof(CacheStatsRow));
    private static readonly StatsSchema PullSchema = StatsSchema.Build(typeof(ConveyorPullRow));

    // Maps a captured snapshot onto the self-describing PluginSdk shapes and publishes
    // it. Registered as Statistics.Publisher, so it is called once per period on the
    // main thread with the snapshot the shared code just captured.
    public static void Publish(StatisticsSnapshot snapshot)
    {
        var cacheRows = snapshot.Caches.Select(cache => new CacheStatsRow
        {
            Name = cache.Name,
            HitRate = cache.HitRatePercent,
            Lookups = cache.Lookups,
            Hits = cache.Hits,
            ItemCount = cache.Size,
        });

        var pullRow = new ConveyorPullRow
        {
            PullItem = snapshot.PullItem,
            PullItems = snapshot.PullItems,
        };

        var stats = new StatsSnapshot
        {
            UtcTimestamp = DateTime.UtcNow,
            Groups =
            {
                CacheSchema.CaptureGroup(cacheRows),
                PullSchema.CaptureGroup(new[] { pullRow }),
            },
        };

        PluginStats.Publish(ProviderName, stats);
    }

    // One row per instrumented cache.
    public sealed class CacheStatsRow
    {
        [StatLabel("Cache")]
        public string Name { get; set; }

        [Gauge("Cache hit rate", Unit = "%", AcrossInstances = StatAggregation.Mean)]
        public double HitRate { get; set; }

        [Counter("Cache lookups")]
        public long Lookups { get; set; }

        [Counter("Cache hits")]
        public long Hits { get; set; }

        [Gauge("Cached item count")]
        public double ItemCount { get; set; }
    }

    // Conveyor item-pull call counts (a single, unlabelled row).
    public sealed class ConveyorPullRow
    {
        [Counter("MyGridConveyorSystem.PullItem calls")]
        public long PullItem { get; set; }

        [Counter("MyGridConveyorSystem.PullItems calls")]
        public long PullItems { get; set; }
    }
}
