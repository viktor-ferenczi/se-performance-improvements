using Shared.Config;
using Xunit;

namespace Tests.Shared.Config;

public class HavokThreadsTests
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    [InlineData(7, 6)]
    [InlineData(16, 6)]
    public void AutoReservesOneCoreAndCapsWorkers(int cores, int expected)
    {
        Assert.Equal(expected, HavokThreads.AutoForCoreCount(cores));
    }
}
