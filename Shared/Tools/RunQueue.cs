using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Shared.Tools;

// The runs waiting for a thread's frame budget, used by ScriptScheduler. Kept free of game
// types so it can be tested on its own.
//
// Add queues an item with flags; adding it again before it ran ORs the flags into the waiting
// entry, so an item runs at most once per drain. Drain runs the waiting items, those passed over
// the most times first and in random order among equal waits, so no item waits longer than
// about demand/capacity drains. Before each run it reserves the longest of the item's last
// HistoryLength run times: the item starts only if the time used so far plus that reservation
// fits the budget. Items that do not fit are skipped, and smaller ones behind them may still run.
// The first item of a drain always runs, so the queue makes progress even when other work used
// up the budget. An item with no run history reserves nothing.
public sealed class RunQueue<T>
    where T : class
{
    public const int HistoryLength = 10;

    public struct DrainResult
    {
        // Items waiting when the drain started
        public int Due;

        // Items run by this drain
        public int Ran;

        // The most times a still waiting item has been passed over, 0 if none waits
        public int MaxWait;

        // Time used in this drain, by the runs and by the time passed in as already used
        public long UsedTicks;
    }

    private sealed class Entry
    {
        public T Item;
        public int Flags;
        public int Wait;
        public int Order;
    }

    // The last run times of an item, kept as long as the item lives
    private sealed class History
    {
        public readonly long[] Ticks = new long[HistoryLength];
        public int Next;

        public long Longest()
        {
            long longest = 0;
            foreach (var ticks in Ticks)
                longest = Math.Max(longest, ticks);
            return longest;
        }
    }

    private static readonly Comparison<Entry> OldestFirst = (a, b) =>
        a.Wait != b.Wait ? b.Wait.CompareTo(a.Wait) : a.Order.CompareTo(b.Order);

    private readonly Dictionary<T, Entry> waiting = new Dictionary<T, Entry>();
    private readonly List<Entry> entries = new List<Entry>();
    private readonly ConditionalWeakTable<T, History> history =
        new ConditionalWeakTable<T, History>();
    private readonly Random random;

    public RunQueue(Random random)
    {
        this.random = random;
    }

    public int Count => entries.Count;

    public void Add(T item, int flags)
    {
        if (waiting.TryGetValue(item, out var entry))
        {
            entry.Flags |= flags;
            return;
        }

        entry = new Entry { Item = item, Flags = flags };
        waiting.Add(item, entry);
        entries.Add(entry);
    }

    // The longest of the item's recent run times, the reservation Drain makes for it
    public long Reservation(T item) => history.TryGetValue(item, out var h) ? h.Longest() : 0;

    // Runs the waiting items that fit the budget. usedTicks is the time already used in this
    // frame by runs outside the queue; clock gives timestamps in the same unit as the budget.
    public DrainResult Drain(long usedTicks, long budgetTicks, Action<T, int> run, Func<long> clock)
    {
        var result = new DrainResult { Due = entries.Count, UsedTicks = usedTicks };
        if (entries.Count == 0)
            return result;

        foreach (var entry in entries)
            entry.Order = random.Next();
        entries.Sort(OldestFirst);

        var kept = 0;
        try
        {
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (result.Ran > 0 && result.UsedTicks + Reservation(entry.Item) > budgetTicks)
                {
                    entry.Wait++;
                    result.MaxWait = Math.Max(result.MaxWait, entry.Wait);
                    entries[kept++] = entry;
                    continue;
                }

                // Out of the queue before it runs, so a run that throws is not retried
                waiting.Remove(entry.Item);
                result.Ran++;
                var start = clock();
                try
                {
                    run(entry.Item, entry.Flags);
                }
                finally
                {
                    var elapsed = clock() - start;
                    result.UsedTicks += elapsed;
                    var h = history.GetOrCreateValue(entry.Item);
                    h.Ticks[h.Next] = elapsed;
                    h.Next = (h.Next + 1) % HistoryLength;
                }
            }
        }
        finally
        {
            // After a throw, the entries not reached yet stay in the queue unchanged
            var reached = kept + result.Ran;
            for (var i = reached; i < entries.Count; i++)
                entries[kept++] = entries[i];
            entries.RemoveRange(kept, entries.Count - kept);
        }

        return result;
    }
}
