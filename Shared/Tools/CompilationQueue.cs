using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Tools;

// The active count includes asynchronous compiler continuations until their result is ready.
// Waiting requests use no worker thread.
internal sealed class CompilationQueue<T>(Func<int> concurrency)
{
    private readonly Queue<(Func<Task> Run, Action Cancel)> pending = new();
    private int active;

    internal Task<T> Submit(Func<Task<T>> compile)
    {
        var result = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        lock (pending)
        {
            pending.Enqueue(
                (
                    async () =>
                    {
                        try
                        {
                            result.SetResult(await compile().ConfigureAwait(false));
                        }
                        catch (OperationCanceledException)
                        {
                            result.SetCanceled();
                        }
                        catch (Exception error)
                        {
                            result.SetException(error);
                        }
                    },
                    () => result.SetCanceled()
                )
            );
            StartWorkers();
        }
        return result.Task;
    }

    // Used at session boundaries only, while the engine still owns the compiler policy.
    internal void WaitForIdle(bool cancelPending = false)
    {
        lock (pending)
        {
            if (cancelPending)
                while (pending.Count != 0)
                    pending.Dequeue().Cancel();
            while (active != 0)
                Monitor.Wait(pending);
        }
    }

    private void StartWorkers()
    {
        while (active < Math.Max(1, concurrency()) && pending.Count != 0)
        {
            var job = pending.Dequeue();
            active++;
            _ = Task.Run(async () =>
            {
                try
                {
                    await job.Run().ConfigureAwait(false);
                }
                finally
                {
                    lock (pending)
                    {
                        active--;
                        StartWorkers();
                        if (active == 0)
                            Monitor.PulseAll(pending);
                    }
                }
            });
        }
    }
}
