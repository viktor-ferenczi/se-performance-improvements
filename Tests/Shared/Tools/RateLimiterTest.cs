using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools;

public class RateLimiterTests
{
    [Fact]
    public void RateLimiter_RateLimit()
    {
        var rateLimiter = new RateLimiter(3);

        for (var i = 0; i < 3; i++)
        {
            Assert.True(rateLimiter.Check());
        }

        Assert.False(rateLimiter.Check());
        Assert.Equal(1, rateLimiter.Reset());
        Assert.True(rateLimiter.Check());
    }
}
