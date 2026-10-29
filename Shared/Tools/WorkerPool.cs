using System;
using System.Collections.Generic;
using System.Threading;

namespace Shared.Tools;

// Persistent background threads that run one body on several threads at once, the calling thread
// being worker 0. Threads are started on first use and stay for later calls, so a frame does not
// pay for starting threads.
public sealed class WorkerPool
{
    private readonly string name;
    private readonly List<Thread> threads = new List<Thread>();
    private readonly object sync = new object();

    private Action<int> body;
    private int generation;
    private int active;
    private int running;
    private Exception error;

    public WorkerPool(string name)
    {
        this.name = name;
    }

    // Calls body(0) on the calling thread and body(1) to body(count - 1) on the pool's threads,
    // and returns once every call has returned. The first exception is thrown again here.
    public void Run(int count, Action<int> work)
    {
        if (count <= 1)
        {
            work(0);
            return;
        }

        lock (sync)
        {
            while (threads.Count < count - 1)
                Start(threads.Count + 1);

            body = work;
            active = count - 1;
            running = count - 1;
            error = null;
            generation++;
            Monitor.PulseAll(sync);
        }

        Exception own = null;
        try
        {
            work(0);
        }
        catch (Exception e)
        {
            own = e;
        }

        lock (sync)
        {
            while (running != 0)
                Monitor.Wait(sync);
            body = null;
        }

        if (own != null)
            throw own;
        if (error != null)
            throw new AggregateException(error);
    }

    private void Start(int index)
    {
        var thread = new Thread(() => Loop(index))
        {
            Name = $"{name} {index}",
            IsBackground = true,
        };
        threads.Add(thread);
        thread.Start();
    }

    private void Loop(int index)
    {
        var seen = 0;
        while (true)
        {
            Action<int> work;
            lock (sync)
            {
                while (generation == seen)
                    Monitor.Wait(sync);
                seen = generation;
                if (index > active)
                    continue;
                work = body;
            }

            try
            {
                work(index);
            }
            catch (Exception e)
            {
                lock (sync)
                    error ??= e;
            }
            finally
            {
                lock (sync)
                {
                    if (--running == 0)
                        Monitor.PulseAll(sync);
                }
            }
        }
    }
}
