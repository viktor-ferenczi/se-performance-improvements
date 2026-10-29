using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

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
//
// With Parallel, the items given a group run after the others on several threads, each thread
// with a budget of its own, see DrainParallel. UsedTicks is then the time of all threads.
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

        // Per drain: the group from Parallel.GroupOf, whether it ran or was passed over
        public object Group;
        public bool Ran;
        public bool Skipped;
    }

    // How Drain runs items in parallel
    public sealed class Parallel
    {
        public WorkerPool Pool;
        public int Threads;

        // Called on the calling thread before the runs: the group of an item that may run on
        // any thread, or null for one that runs on the calling thread. Items of one group never
        // run at the same time.
        public Func<T, object> GroupOf;

        // Runs an item of a group, on any of the threads
        public Action<T, int> Run;
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
    // With parallel, the items it gives a group run after the others, on Threads threads.
    public DrainResult Drain(
        long usedTicks,
        long budgetTicks,
        Action<T, int> run,
        Func<long> clock,
        Parallel parallel = null
    )
    {
        var result = new DrainResult { Due = entries.Count, UsedTicks = usedTicks };
        if (entries.Count == 0)
            return result;

        var anyGroup = false;
        foreach (var entry in entries)
        {
            entry.Order = random.Next();
            entry.Ran = false;
            entry.Skipped = false;
            entry.Group = parallel?.GroupOf(entry.Item);
            anyGroup |= entry.Group != null;
        }
        entries.Sort(OldestFirst);

        try
        {
            foreach (var entry in entries)
            {
                if (entry.Group != null)
                    continue;
                if (result.Ran > 0 && result.UsedTicks + Reservation(entry.Item) > budgetTicks)
                {
                    entry.Skipped = true;
                    continue;
                }

                // Out of the queue before it runs, so a run that throws is not retried
                waiting.Remove(entry.Item);
                entry.Ran = true;
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
                    Record(entry.Item, elapsed);
                }
            }

            if (anyGroup)
                DrainParallel(parallel, budgetTicks, clock, ref result);
        }
        finally
        {
            // After a throw, the entries not reached yet stay in the queue unchanged
            var kept = 0;
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Ran)
                    continue;
                if (entry.Skipped)
                {
                    entry.Wait++;
                    result.MaxWait = Math.Max(result.MaxWait, entry.Wait);
                }
                entries[kept++] = entry;
            }
            entries.RemoveRange(kept, entries.Count - kept);
        }

        return result;
    }

    // Runs the entries with a group on parallel.Threads workers, the calling thread being the
    // first, which has already used result.UsedTicks. Each worker takes the oldest entry that
    // fits its own budget and whose group no other worker is running, so the items of a group
    // never run at the same time; it waits while the only entries that fit are in busy groups.
    // As on one thread, the first run of a worker always starts.
    private void DrainParallel(
        Parallel parallel,
        long budgetTicks,
        Func<long> clock,
        ref DrainResult result
    )
    {
        var threads = Math.Max(1, parallel.Threads);
        var used = new long[threads];
        var ran = new bool[threads];
        used[0] = result.UsedTicks;
        ran[0] = result.Ran > 0;
        var busy = new HashSet<object>();
        var sync = new object();
        var ranCount = 0;

        parallel.Pool.Run(
            threads,
            worker =>
            {
                while (true)
                {
                    Entry entry = null;
                    lock (sync)
                    {
                        while (true)
                        {
                            var blocked = false;
                            foreach (var e in entries)
                            {
                                if (e.Group == null || e.Ran)
                                    continue;
                                if (ran[worker] && used[worker] + Reservation(e.Item) > budgetTicks)
                                    continue;
                                if (busy.Contains(e.Group))
                                {
                                    blocked = true;
                                    continue;
                                }
                                entry = e;
                                break;
                            }
                            if (entry != null || !blocked)
                                break;
                            Monitor.Wait(sync);
                        }
                        if (entry == null)
                            return;

                        busy.Add(entry.Group);
                        waiting.Remove(entry.Item);
                        entry.Ran = true;
                        ran[worker] = true;
                        ranCount++;
                    }

                    var start = clock();
                    try
                    {
                        parallel.Run(entry.Item, entry.Flags);
                    }
                    finally
                    {
                        var elapsed = clock() - start;
                        lock (sync)
                        {
                            used[worker] += elapsed;
                            Record(entry.Item, elapsed);
                            busy.Remove(entry.Group);
                            Monitor.PulseAll(sync);
                        }
                    }
                }
            }
        );

        result.Ran += ranCount;
        for (var i = 1; i < threads; i++)
            used[0] += used[i];
        result.UsedTicks = used[0];
        foreach (var entry in entries)
            entry.Skipped |= entry.Group != null && !entry.Ran;
    }

    private void Record(T item, long elapsed)
    {
        var h = history.GetOrCreateValue(item);
        h.Ticks[h.Next] = elapsed;
        h.Next = (h.Next + 1) % HistoryLength;
    }
}
