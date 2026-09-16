using SmartSpace.Api.Domain;

namespace SmartSpace.UnitTests;

public sealed class BookingIntervalRulesTests
{
    private static readonly DateTimeOffset Base = new(2026, 10, 1, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Rejects_end_before_or_equal_to_start()
    {
        Assert.False(BookingIntervalRules.IsValid(Base, Base.AddMinutes(-1)));
        Assert.False(BookingIntervalRules.IsValid(Base, Base));
    }

    [Fact]
    public void Detects_partial_containment_and_identical_overlap()
    {
        var existingStart = Base;
        var existingEnd = Base.AddHours(1);

        Assert.True(BookingIntervalRules.Overlaps(existingStart, existingEnd, Base.AddMinutes(30), Base.AddHours(2)));
        Assert.True(BookingIntervalRules.Overlaps(existingStart, existingEnd, Base.AddMinutes(-1), Base.AddMinutes(30)));
        Assert.True(BookingIntervalRules.Overlaps(existingStart, existingEnd, existingStart, existingEnd));
    }

    [Fact]
    public void Allows_adjacent_half_open_intervals()
    {
        Assert.False(BookingIntervalRules.Overlaps(Base, Base.AddHours(1), Base.AddHours(1), Base.AddHours(2)));
    }
}
