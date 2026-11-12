using System;
using System.Collections.Generic;

namespace Shared.Tools;

// The per-player time quota of programmable blocks, used by ScriptQuota. Kept free of game types
// so it can be tested on its own.
//
// Add records the time of one run of an item, on any thread. EndFrame adds up the frame's runs
// by owner and finds each owner's most expensive item. An owner over the hard limit, or over the
// soft limit in SoftLimitFrames frames in a row, gets an offence naming that item, at most one
// per owner and frame. A frame in which none of an owner's items ran leaves the soft limit
// count alone.
public sealed class PlayerQuota<T>
    where T : class
{
    public const int SoftLimitFrames = 3;

    // An owner not seen for this many frames is forgotten
    public const int ForgetFrames = 10 * 60 * 60;

    public struct Offence
    {
        public long Owner;
        public T Item;

        // Time of all the owner's runs in the frame, and of the item's
        public long OwnerTicks;
        public long ItemTicks;

        // Over the hard limit, otherwise over the soft limit in SoftLimitFrames frames in a row
        public bool Hard;
    }

    private sealed class Owner
    {
        public long Id;
        public long Ticks;
        public T Top;
        public long TopTicks;
        public int SoftFrames;
        public long Frame;
    }

    private readonly Dictionary<T, long> runs = new Dictionary<T, long>();
    private readonly Dictionary<long, Owner> owners = new Dictionary<long, Owner>();
    private readonly List<Owner> touched = new List<Owner>();
    private readonly List<long> forgotten = new List<long>();
    private long frame;

    // Owners with a state kept
    public int OwnerCount => owners.Count;

    public void Add(T item, long ticks)
    {
        lock (runs)
        {
            runs.TryGetValue(item, out var sum);
            runs[item] = sum + ticks;
        }
    }

    // Called between frames, while no item runs. ownerOf gives an item's owner, exempt tells
    // which owners are not limited. With checkHard false the hard limit is not checked.
    public void EndFrame(
        Func<T, long> ownerOf,
        Func<long, bool> exempt,
        long softTicks,
        long hardTicks,
        bool checkHard,
        List<Offence> offences
    )
    {
        frame++;
        foreach (var run in runs)
        {
            var id = ownerOf(run.Key);
            if (exempt(id))
                continue;

            if (!owners.TryGetValue(id, out var owner))
                owners[id] = owner = new Owner { Id = id };
            if (owner.Frame != frame)
            {
                owner.Frame = frame;
                owner.Ticks = 0;
                owner.TopTicks = -1;
                touched.Add(owner);
            }

            owner.Ticks += run.Value;
            if (run.Value > owner.TopTicks)
            {
                owner.Top = run.Key;
                owner.TopTicks = run.Value;
            }
        }
        runs.Clear();

        foreach (var owner in touched)
        {
            var hard = checkHard && owner.Ticks > hardTicks;
            if (hard || owner.Ticks > softTicks && ++owner.SoftFrames >= SoftLimitFrames)
            {
                owner.SoftFrames = 0;
                offences.Add(
                    new Offence
                    {
                        Owner = owner.Id,
                        Item = owner.Top,
                        OwnerTicks = owner.Ticks,
                        ItemTicks = owner.TopTicks,
                        Hard = hard,
                    }
                );
            }
            else if (owner.Ticks <= softTicks)
                owner.SoftFrames = 0;

            owner.Top = null;
        }
        touched.Clear();

        if (frame % ForgetFrames == 0)
            Forget();
    }

    public void Clear()
    {
        lock (runs)
            runs.Clear();
        owners.Clear();
    }

    private void Forget()
    {
        foreach (var pair in owners)
            if (frame - pair.Value.Frame >= ForgetFrames)
                forgotten.Add(pair.Key);
        foreach (var id in forgotten)
            owners.Remove(id);
        forgotten.Clear();
    }
}
