using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Shared.Tools
{
    // Not thread safe, since we don't care about super exact results
    public class CacheStat
    {
        private long Lookups { get; set; }
        private long Hits { get; set; }
        private int Size { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Reset(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset(int size)
        {
            Lookups = 0;
            Hits = 0;
            Size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncreaseSize(int size)
        {
            Size = Math.Max(Size, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CountLookup(int size)
        {
            Size = size;
            Lookups++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CountHit()
        {
            Hits++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(CacheStat source)
        {
            Lookups += source.Lookups;
            Hits += source.Hits;
            Size += source.Size;

            source.Reset(source.Size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<CacheStat> stats)
        {
            foreach (var stat in stats)
                Add(stat);
        }

        // Reads the accumulated counters into an immutable sample and resets them,
        // clamping hits to lookups (the counters are updated without locking, so a
        // race can momentarily overshoot). This is the single numeric accessor both
        // the human-readable Report and the statistics publishing build on, so the
        // counters are read and reset exactly once per period.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CacheStatSample Sample()
        {
            var lookups = Lookups;
            var hits = Hits;
            var size = Size;

            if (hits > lookups)
                hits = lookups;

            Reset(size);

            return new CacheStatSample(lookups, hits, size);
        }

        public string Report => Sample().ToString();
    }

    // Immutable snapshot of a CacheStat, taken by CacheStat.Sample. Carries the raw
    // counters so a consumer (the statistics publisher, or the Report formatter) can
    // compute the hit rate however it needs.
    public readonly struct CacheStatSample
    {
        public readonly long Lookups;
        public readonly long Hits;
        public readonly int Size;

        public CacheStatSample(long lookups, long hits, int size)
        {
            Lookups = lookups;
            Hits = hits;
            Size = size;
        }

        public double HitRatePercent => Lookups > 0 ? 100.0 * Hits / Lookups : 100.0;

        public override string ToString() => $"HitRate = {HitRatePercent:0.000}% = {Hits}/{Lookups}; ItemCount = {Size}";
    }
}