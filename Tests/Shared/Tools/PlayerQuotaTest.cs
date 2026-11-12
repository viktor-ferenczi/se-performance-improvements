using System.Collections.Generic;
using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools;

public class PlayerQuotaTest
{
    private sealed class Pb
    {
        public string Name;
        public long Owner;

        public override string ToString() => Name;
    }

    private const long Soft = 10;
    private const long Hard = 25;

    private readonly PlayerQuota<Pb> quota = new PlayerQuota<Pb>();
    private readonly HashSet<long> exempt = new HashSet<long>();

    private List<PlayerQuota<Pb>.Offence> EndFrame(bool checkHard = true)
    {
        var offences = new List<PlayerQuota<Pb>.Offence>();
        quota.EndFrame(pb => pb.Owner, exempt.Contains, Soft, Hard, checkHard, offences);
        return offences;
    }

    private static Pb Make(string name, long owner) => new Pb { Name = name, Owner = owner };

    [Fact]
    public void UnderTheLimitsNothingHappens()
    {
        var a = Make("a", 1);
        for (var i = 0; i < 10; i++)
        {
            quota.Add(a, Soft);
            Assert.Empty(EndFrame());
        }
    }

    [Fact]
    public void HardLimitTurnsOffTheMostExpensiveAtOnce()
    {
        var a = Make("a", 1);
        var b = Make("b", 1);
        quota.Add(a, 10);
        quota.Add(b, 12);
        quota.Add(a, 5);

        var offence = Assert.Single(EndFrame());

        Assert.True(offence.Hard);
        Assert.Same(a, offence.Item);
        Assert.Equal(1, offence.Owner);
        Assert.Equal(27, offence.OwnerTicks);
        Assert.Equal(15, offence.ItemTicks);
    }

    [Fact]
    public void SoftLimitNeedsThreeFramesInARow()
    {
        var a = Make("a", 1);
        var b = Make("b", 1);
        for (var i = 0; i < PlayerQuota<Pb>.SoftLimitFrames - 1; i++)
        {
            quota.Add(a, 6);
            quota.Add(b, 5);
            Assert.Empty(EndFrame());
        }

        quota.Add(a, 4);
        quota.Add(b, 7);
        var offence = Assert.Single(EndFrame());

        Assert.False(offence.Hard);
        Assert.Same(b, offence.Item);

        // The count starts over after an offence
        quota.Add(a, 11);
        Assert.Empty(EndFrame());
    }

    [Fact]
    public void FrameUnderTheSoftLimitResetsTheCount()
    {
        var a = Make("a", 1);
        quota.Add(a, 11);
        EndFrame();
        quota.Add(a, 11);
        EndFrame();
        quota.Add(a, 9);
        EndFrame();
        quota.Add(a, 11);
        Assert.Empty(EndFrame());
        quota.Add(a, 11);
        Assert.Empty(EndFrame());
        quota.Add(a, 11);
        Assert.Single(EndFrame());
    }

    [Fact]
    public void FrameWithoutRunsKeepsTheCount()
    {
        var a = Make("a", 1);
        quota.Add(a, 11);
        EndFrame();
        quota.Add(a, 11);
        EndFrame();
        Assert.Empty(EndFrame());
        Assert.Empty(EndFrame());
        quota.Add(a, 11);
        Assert.Single(EndFrame());
    }

    [Fact]
    public void PlayersAreCountedApart()
    {
        quota.Add(Make("a", 1), 20);
        quota.Add(Make("b", 2), 20);
        Assert.Empty(EndFrame());

        // Together they would be over the hard limit, each alone is not
        quota.Add(Make("c", 3), 30);
        var offence = Assert.Single(EndFrame());
        Assert.Equal(3, offence.Owner);
    }

    [Fact]
    public void AtMostOnePbPerPlayerAndFrame()
    {
        quota.Add(Make("a", 1), 100);
        quota.Add(Make("b", 1), 90);
        Assert.Single(EndFrame());
    }

    [Fact]
    public void ExemptOwnersAreNotLimited()
    {
        exempt.Add(1);
        quota.Add(Make("a", 1), 100);
        Assert.Empty(EndFrame());
        Assert.Equal(0, quota.OwnerCount);
    }

    [Fact]
    public void HardLimitSkippedStillCountsSoft()
    {
        var a = Make("a", 1);
        quota.Add(a, 100);
        Assert.Empty(EndFrame(checkHard: false));
        quota.Add(a, 100);
        Assert.Empty(EndFrame(checkHard: false));
        quota.Add(a, 100);
        var offence = Assert.Single(EndFrame(checkHard: false));
        Assert.False(offence.Hard);
    }

    [Fact]
    public void IdleOwnersAreForgotten()
    {
        quota.Add(Make("a", 1), 1);
        EndFrame();
        Assert.Equal(1, quota.OwnerCount);
        // Checked once per ForgetFrames, so it takes up to twice that
        for (var i = 0; i < 2 * PlayerQuota<Pb>.ForgetFrames; i++)
            EndFrame();
        Assert.Equal(0, quota.OwnerCount);
    }
}
