using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Config;
using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools;

public class CompilationQueueTest
{
    [ThreadStatic]
    private static bool submitting;

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(CompilationLimits.Default, 2)]
    [InlineData(4, 4)]
    [InlineData(16, 16)]
    [InlineData(17, 16)]
    public void ConfigurationUsesTheSameBoundsAsTheSettingsControls(int value, int expected)
    {
        Assert.Equal(expected, CompilationLimits.Clamp(value));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public async Task MixedRequestsShareOneLimit(int limit)
    {
        var queue = new CompilationQueue<string>(() => limit);
        var firstWorkers = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var release = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        int entered = 0;
        int active = 0;
        int overLimit = 0;
        string[] requests = { "mod-a", "pb-a", "mod-b", "pb-b", "mod-c", "pb-c" };
        var tasks = new List<Task<string>>();
        try
        {
            submitting = true;
            foreach (var request in requests)
                tasks.Add(
                    queue.Submit(async () =>
                    {
                        Assert.False(submitting);
                        if (Interlocked.Increment(ref active) > limit)
                            Interlocked.Increment(ref overLimit);
                        if (Interlocked.Increment(ref entered) == limit)
                            firstWorkers.TrySetResult(true);
                        try
                        {
                            await release.Task;
                            return request;
                        }
                        finally
                        {
                            Interlocked.Decrement(ref active);
                        }
                    })
                );
        }
        finally
        {
            submitting = false;
        }
        try
        {
            await firstWorkers.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(limit, Volatile.Read(ref entered));
            Assert.All(tasks, task => Assert.False(task.IsCompleted));
        }
        finally
        {
            release.TrySetResult(true);
        }
        Assert.Equal(requests, await Task.WhenAll(tasks));
        Assert.Equal(0, overLimit);
    }

    [Fact]
    public async Task SessionUnloadDropsPendingWorkAndWaitsForTheActiveCompile()
    {
        var queue = new CompilationQueue<int>(() => 1);
        var entered = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var finish = new TaskCompletionSource<int>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var active = queue.Submit(() =>
        {
            entered.SetResult(true);
            return finish.Task;
        });
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var pending = queue.Submit(() => throw new InvalidOperationException("stale request ran"));
        var unload = Task.Run(() => queue.WaitForIdle(cancelPending: true));
        try
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                pending.WaitAsync(TimeSpan.FromSeconds(5))
            );
            Assert.False(unload.IsCompleted);
        }
        finally
        {
            finish.TrySetResult(42);
        }
        await unload.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(42, await active);
        Assert.Equal(7, await queue.Submit(() => Task.FromResult(7)));
    }

    [Fact]
    public async Task FaultsAndCancellationsReleaseTheirSlot()
    {
        var queue = new CompilationQueue<int>(() => 1);
        var fault = queue.Submit(() => throw new FormatException());
        var cancelled = queue.Submit(() => Task.FromCanceled<int>(new CancellationToken(true)));
        var success = queue.Submit(() => Task.FromResult(42));
        await Assert.ThrowsAsync<FormatException>(() => fault);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => cancelled);
        Assert.Equal(42, await success.WaitAsync(TimeSpan.FromSeconds(5)));
    }
}
