using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Shared.Patches;

// The writes of one script run on a worker thread (SE-626).
//
// While a log is active on a thread, the proxies of ScriptProxies record the calls of queued
// members here instead of making them, and refuse main-thread members. Replay makes the
// recorded calls on the main thread afterwards, in the order the script made them. A script
// that sets a property and reads it back in the same run reads the value it set. Everything
// else it reads is the state before the phase, so effects of its own writes on other state
// (set Enabled, read IsWorking) show up a frame later.
//
// Outside an active log the proxies call the game directly, as on the main thread.
public sealed class ScriptWrites
{
    [ThreadStatic]
    private static ScriptWrites current;

    private readonly List<(int Writer, object Target, object[] Args)> calls =
        new List<(int, object, object[])>();

    private readonly Dictionary<(object, int), object> written = new Dictionary<
        (object, int),
        object
    >(new TargetComparer());

    public static bool Active => current != null;

    public int Count => calls.Count;

    // Makes this log the thread's active one until End
    public void Begin()
    {
        calls.Clear();
        written.Clear();
        current = this;
    }

    public static void End()
    {
        current = null;
    }

    // Called by a proxy for a queued member, with the arguments already converted to game objects
    public static void Add(int writer, object target, object[] args)
    {
        var log = current;
        for (var i = 0; i < args.Length; i++)
            args[i] = Snapshot(args[i]);
        log.calls.Add((writer, target, args));
        if (args.Length == 1 && ScriptProxies.IsSetter(writer))
            log.written[(target, writer)] = args[0];
    }

    // Called by a proxy for a getter whose setter is queued: the value the script set in this run
    public static bool TryRead(object target, int setter, out object value)
    {
        value = null;
        return current != null && current.written.TryGetValue((target, setter), out value);
    }

    // The script may change a text or a list it passed after the call returned, before replay
    private static object Snapshot(object arg)
    {
        switch (arg)
        {
            case StringBuilder text:
                return new StringBuilder(text.ToString());
            case Array array:
                return array.Clone();
            case IList list
                when arg.GetType().IsGenericType
                    && arg.GetType().GetGenericTypeDefinition() == typeof(List<>):
                return Activator.CreateInstance(arg.GetType(), list);
            default:
                return arg;
        }
    }

    // Called by a stand-in: makes the call on the main thread at replay, in order with the others
    public static void Later(Action call)
    {
        current.calls.Add((-1, call, null));
    }

    public static Exception Refuse(string member) =>
        new InvalidOperationException(
            $"{member} is not available to programmable block scripts running on a worker thread"
        );

    // On the main thread: makes the recorded calls. A failing call is reported and skipped.
    public void Replay(Action<Exception> report)
    {
        foreach (var (writer, target, args) in calls)
        {
            try
            {
                if (writer < 0)
                    ((Action)target)();
                else
                    ScriptProxies.Write(writer, target, args);
            }
            catch (Exception e)
            {
                report(e);
            }
        }
        calls.Clear();
        written.Clear();
    }

    // Game objects by reference, whatever their Equals does
    private sealed class TargetComparer : IEqualityComparer<(object, int)>
    {
        public bool Equals((object, int) a, (object, int) b) =>
            ReferenceEquals(a.Item1, b.Item1) && a.Item2 == b.Item2;

        public int GetHashCode((object, int) key) =>
            RuntimeHelpers.GetHashCode(key.Item1) * 31 + key.Item2;
    }
}
