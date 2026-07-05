using System.Runtime.CompilerServices;

namespace Shared.Patches
{
    public class PullItemStats
    {
        public long PullItemCount;
        public long PullItemsCount;

        // Reads the accumulated call counts into an immutable sample and resets them.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PullItemStatsSample Sample()
        {
            var pullItemCount = PullItemCount;
            var pullItemsCount = PullItemsCount;

            Reset();

            return new PullItemStatsSample(pullItemCount, pullItemsCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Reset()
        {
            PullItemCount = 0;
            PullItemsCount = 0;
        }
    }

    // Immutable snapshot of PullItemStats, taken by PullItemStats.Sample.
    public readonly struct PullItemStatsSample
    {
        public readonly long PullItem;
        public readonly long PullItems;

        public PullItemStatsSample(long pullItem, long pullItems)
        {
            PullItem = pullItem;
            PullItems = pullItems;
        }
    }
}
