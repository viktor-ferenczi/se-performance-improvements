using System.Collections.Generic;

namespace Shared.Stats
{
    // A host-agnostic, point-in-time capture of the plugin's runtime statistics.
    //
    // This is deliberately free of any dependency on the Magnetar PluginSdk (or any
    // other host): the Shared code that collects the counters produces it, and a host
    // consumes it. On the dedicated server the consumer maps it onto the PluginSdk
    // statistics API (see ServerPlugin.Stats.PerformanceStats); the client currently
    // has no consumer. When the client also ships the PluginSdk the same consumer can
    // move into Shared unchanged, since it only ever sees this neutral shape.
    public sealed class StatisticsSnapshot
    {
        // One entry per instrumented cache (hit rate, lookups, hits, item count).
        public readonly List<CacheStatEntry> Caches = new List<CacheStatEntry>();

        // MyGridConveyorSystem.PullItem / PullItems call counts over the period.
        public long PullItem;
        public long PullItems;
    }

    // A single instrumented cache's counters over one period.
    public readonly struct CacheStatEntry
    {
        public readonly string Name;
        public readonly long Lookups;
        public readonly long Hits;
        public readonly long Size;

        public CacheStatEntry(string name, long lookups, long hits, long size)
        {
            Name = name;
            Lookups = lookups;
            Hits = hits;
            Size = size;
        }

        public double HitRatePercent => Lookups > 0 ? 100.0 * Hits / Lookups : 100.0;
    }
}
