using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools;

public class RunQueueTest
{
    private sealed class Pb
    {
        public string Name;
        public long Cost;
        public readonly List<int> Runs = new List<int>();

        public override string ToString() => Name;
    }

    private long now;
    private readonly RunQueue<Pb> queue = new RunQueue<Pb>(new Random(1));

    private void Run(Pb pb, int flags)
    {
        pb.Runs.Add(flags);
        now += pb.Cost;
    }

    private RunQueue<Pb>.DrainResult Drain(long budget, long used = 0) =>
        queue.Drain(used, budget, Run, () => now);

    private static List<Pb> Make(int count, long cost, string prefix = "pb") =>
        Enumerable.Range(0, count).Select(i => new Pb { Name = prefix + i, Cost = cost }).ToList();

    private void AddAll(IEnumerable<Pb> pbs, int flags = 1)
    {
        foreach (var pb in pbs)
            queue.Add(pb, flags);
    }

    [Fact]
    public void DueTwiceRunsOnceWithBothFlags()
    {
        var pb = Make(1, 1)[0];
        queue.Add(pb, 1);
        queue.Add(pb, 2);

        var result = Drain(100);

        Assert.Equal(new[] { 3 }, pb.Runs);
        Assert.Equal(1, result.Due);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void UnknownRunsReserveNothing()
    {
        var pbs = Make(4, 3);
        AddAll(pbs);

        // No history yet: runs start while the budget is not used up
        var result = Drain(5);

        Assert.Equal(2, result.Ran);
        Assert.Equal(6, result.UsedTicks);
        Assert.Equal(1, result.MaxWait);
    }

    [Fact]
    public void ReservesTheLongestRecentRun()
    {
        var pbs = Make(3, 3);
        AddAll(pbs);
        Drain(100);
        Assert.All(pbs, pb => Assert.Equal(3, queue.Reservation(pb)));

        // 3 + 3 does not fit 5: one run per drain
        AddAll(pbs);
        Assert.Equal(1, Drain(5).Ran);
        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void ReservationForgetsOldRuns()
    {
        var pb = Make(1, 50)[0];
        queue.Add(pb, 1);
        Drain(100);

        pb.Cost = 1;
        for (var i = 0; i < RunQueue<Pb>.HistoryLength - 1; i++)
        {
            queue.Add(pb, 1);
            Drain(100);
            Assert.Equal(50, queue.Reservation(pb));
        }

        queue.Add(pb, 1);
        Drain(100);
        Assert.Equal(1, queue.Reservation(pb));
    }

    [Fact]
    public void FirstRunAlwaysStarts()
    {
        var pbs = Make(2, 1);
        AddAll(pbs);

        // Runs outside the queue used more than the budget
        var result = Drain(5, used: 10);

        Assert.Equal(1, result.Ran);
        Assert.Equal(11, result.UsedTicks);
    }

    // Hands out increasing numbers, so equal waits keep the order the runs were added in
    private sealed class InOrder : Random
    {
        private int next;

        public override int Next() => next++;
    }

    [Fact]
    public void SmallRunsFillWhatABigOneLeaves()
    {
        var ordered = new RunQueue<Pb>(new InOrder());
        RunQueue<Pb>.DrainResult Drain(long budget) => ordered.Drain(0, budget, Run, () => now);

        var big = Make(1, 4, "big")[0];
        var small = Make(3, 1, "small");
        ordered.Add(big, 1);
        foreach (var pb in small)
            ordered.Add(pb, 1);
        Drain(100);

        // The big run does not fit behind a small one and waits
        ordered.Add(small[0], 1);
        ordered.Add(big, 1);
        Assert.Equal(1, Drain(2).Ran);

        // So it goes first next time, and the small ones fill what it leaves
        foreach (var pb in small)
            ordered.Add(pb, 1);
        var result = Drain(5);

        Assert.Equal(2, big.Runs.Count);
        Assert.Equal(2, result.Ran);
        Assert.Equal(5, result.UsedTicks);
        Assert.Equal(1, result.MaxWait);
    }

    [Fact]
    public void NoRunWaitsLongerThanDemandOverCapacity()
    {
        // 20 runs of 2 due every frame, 2 fit a budget of 5: each runs every 10th frame
        var pbs = Make(20, 2);
        var longest = 0;
        for (var frame = 0; frame < 200; frame++)
        {
            AddAll(pbs);
            longest = Math.Max(longest, Drain(5).MaxWait);
        }

        Assert.InRange(longest, 9, 10);
        Assert.All(pbs, pb => Assert.InRange(pb.Runs.Count, 19, 21));
    }

    [Fact]
    public void ThrowingRunIsDroppedOthersStay()
    {
        var pbs = Make(3, 1);
        AddAll(pbs);
        var calls = 0;

        Assert.Throws<InvalidOperationException>(() =>
            queue.Drain(
                0,
                100,
                (pb, flags) =>
                {
                    if (++calls == 2)
                        throw new InvalidOperationException();
                },
                () => 0
            )
        );

        Assert.Equal(1, queue.Count);
        Assert.Equal(1, Drain(100).Ran);
    }

    // Parallel drains: each thread has a clock of its own, advanced by the cost of its runs
    private readonly System.Threading.ThreadLocal<long> threadNow = new(() => 0);
    private static readonly WorkerPool Pool = new WorkerPool("RunQueueTest");

    private RunQueue<Pb>.Parallel Parallel(
        int threads,
        Func<Pb, object> groupOf,
        Action<Pb> during = null
    ) =>
        new RunQueue<Pb>.Parallel
        {
            Pool = Pool,
            Threads = threads,
            GroupOf = groupOf,
            Run = (pb, flags) =>
            {
                lock (pb.Runs)
                    pb.Runs.Add(flags);
                during?.Invoke(pb);
                threadNow.Value += pb.Cost;
            },
        };

    private RunQueue<Pb>.DrainResult DrainParallel(long budget, RunQueue<Pb>.Parallel parallel) =>
        queue.Drain(0, budget, Run, () => threadNow.Value, parallel);

    [Fact]
    public void ParallelRunsEveryItemOnce()
    {
        var pbs = Make(40, 1);
        AddAll(pbs);

        var result = DrainParallel(1000, Parallel(4, pb => pb.Name));

        Assert.Equal(40, result.Ran);
        Assert.All(pbs, pb => Assert.Single(pb.Runs));
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void ItemsOfAGroupNeverOverlap()
    {
        var pbs = Make(24, 1);
        AddAll(pbs);
        var running = new int[3];
        var overlaps = 0;

        DrainParallel(
            1000,
            Parallel(
                4,
                pb => int.Parse(pb.Name.Substring(2)) % 3,
                pb =>
                {
                    var group = int.Parse(pb.Name.Substring(2)) % 3;
                    if (System.Threading.Interlocked.Increment(ref running[group]) > 1)
                        System.Threading.Interlocked.Increment(ref overlaps);
                    System.Threading.Thread.Sleep(1);
                    System.Threading.Interlocked.Decrement(ref running[group]);
                }
            )
        );

        Assert.Equal(0, overlaps);
        Assert.All(pbs, pb => Assert.Single(pb.Runs));
    }

    [Fact]
    public void EveryThreadHasABudget()
    {
        var pbs = Make(8, 3);
        AddAll(pbs);
        DrainParallel(1000, Parallel(1, pb => pb.Name));

        // 3 + 3 does not fit 5: one run per thread
        AddAll(pbs);
        Assert.Equal(1, DrainParallel(5, Parallel(1, pb => pb.Name)).Ran);
        Assert.Equal(4, DrainParallel(5, Parallel(4, pb => pb.Name)).Ran);
        Assert.Equal(3, queue.Count);
        Assert.Equal(3, queue.Drain(0, 5, Run, () => now).MaxWait);
    }

    [Fact]
    public void ItemsWithoutAGroupRunOnTheCallingThread()
    {
        var pbs = Make(6, 1);
        AddAll(pbs);
        var caller = System.Threading.Thread.CurrentThread;
        var mainRuns = new List<Pb>();

        queue.Drain(
            0,
            1000,
            (pb, flags) =>
            {
                Assert.Same(caller, System.Threading.Thread.CurrentThread);
                mainRuns.Add(pb);
            },
            () => 0,
            Parallel(3, pb => pb.Name.EndsWith("0") || pb.Name.EndsWith("1") ? null : pb.Name)
        );

        Assert.Equal(new[] { "pb0", "pb1" }, mainRuns.Select(pb => pb.Name).OrderBy(n => n));
        Assert.Equal(4, pbs.Count(pb => pb.Runs.Count == 1));
    }
}
