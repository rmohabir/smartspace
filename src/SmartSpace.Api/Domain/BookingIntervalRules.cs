namespace SmartSpace.Api.Domain;

public static class BookingIntervalRules
{
    public static bool IsValid(DateTimeOffset startUtc, DateTimeOffset endUtc) => endUtc > startUtc;

    public static bool Overlaps(
        DateTimeOffset existingStartUtc,
        DateTimeOffset existingEndUtc,
        DateTimeOffset requestedStartUtc,
        DateTimeOffset requestedEndUtc) =>
        existingStartUtc < requestedEndUtc && existingEndUtc > requestedStartUtc;
}
