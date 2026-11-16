using System;
using System.Runtime.CompilerServices;

namespace Shared.Patches;

// Whether a script instance gets proxies behind its roots, see ScriptRoots (SE-1588).
//
// The decision is made only where the game creates the instance and sets Runtime and Me. The
// setters of each run (GridTerminalSystem, IGC) only look it up. An instance the game made
// before the patches were in place has a raw Runtime and Me and no decision, so it keeps the
// game objects for good. Deciding there would hand it proxied blocks next to a raw Me, and a
// raw block or grid called with a proxy argument throws InvalidCastException in the game.
public sealed class RootDecisions<TProgram, TDecision>
    where TProgram : class
    where TDecision : class
{
    private readonly ConditionalWeakTable<TProgram, TDecision> decisions =
        new ConditionalWeakTable<TProgram, TDecision>();
    private readonly ConditionalWeakTable<TProgram, TDecision>.CreateValueCallback decide;

    public RootDecisions(Func<TProgram, TDecision> decide)
    {
        this.decide = p => decide(p);
    }

    // While the game creates the instance: decides once
    public TDecision AtCreation(TProgram program) => decisions.GetValue(program, decide);

    // Anywhere else: null if the instance was not seen at creation
    public TDecision Of(TProgram program) =>
        decisions.TryGetValue(program, out var decision) ? decision : null;
}
