using Shared.Patches;
using Xunit;

namespace Tests.Shared.Patches;

// SE-1588: a script instance decides at creation whether its roots are proxies; the setters of
// a run only follow that decision, so Me and the blocks from GridTerminalSystem always agree.
public class RootDecisionsTest
{
    private sealed class Program { }

    private sealed class Decision
    {
        public bool Proxied;
    }

    private int decided;

    private RootDecisions<Program, Decision> Decisions(bool proxied) =>
        new RootDecisions<Program, Decision>(_ =>
        {
            decided++;
            return new Decision { Proxied = proxied };
        });

    [Fact]
    public void RunFollowsTheDecisionMadeAtCreation()
    {
        var decisions = Decisions(true);
        var program = new Program();

        Assert.True(decisions.AtCreation(program).Proxied); // Runtime
        Assert.True(decisions.AtCreation(program).Proxied); // Me
        Assert.True(decisions.Of(program).Proxied); // GridTerminalSystem of a run
        Assert.Equal(1, decided);
    }

    // The plugin patched the setters after the game had made the instance with a raw Me: a run
    // must not proxy GridTerminalSystem, or a raw block gets a proxy argument and the game's
    // cast to MyCubeGrid throws
    [Fact]
    public void InstanceMadeBeforeThePatchesStaysOnGameObjects()
    {
        var decisions = Decisions(true);
        var program = new Program();

        Assert.Null(decisions.Of(program));
        Assert.Null(decisions.Of(program));
        Assert.Equal(0, decided);
    }

    [Fact]
    public void DecisionIsPerInstance()
    {
        var decisions = Decisions(true);
        var created = new Program();
        decisions.AtCreation(created);

        Assert.NotNull(decisions.Of(created));
        Assert.Null(decisions.Of(new Program()));
    }
}
