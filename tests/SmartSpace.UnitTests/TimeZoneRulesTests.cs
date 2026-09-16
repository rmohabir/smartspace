using SmartSpace.Api.Domain;

namespace SmartSpace.UnitTests;

public sealed class TimeZoneRulesTests
{
    [Fact]
    public void Rejects_nonexistent_amsterdam_time_during_spring_transition()
    {
        var localTime = new DateTime(2026, 3, 29, 2, 30, 0, DateTimeKind.Unspecified);

        Assert.True(LocalTimeRules.IsInvalid(localTime));
    }

    [Fact]
    public void Identifies_ambiguous_amsterdam_time_during_autumn_transition()
    {
        var localTime = new DateTime(2026, 10, 25, 2, 30, 0, DateTimeKind.Unspecified);

        Assert.True(LocalTimeRules.IsAmbiguous(localTime));
    }
}
