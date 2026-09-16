namespace SmartSpace.Api.Domain;

public static class LocalTimeRules
{
    public static TimeZoneInfo AmsterdamTimeZone =>
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "W. Europe Standard Time" : "Europe/Amsterdam");

    public static bool IsInvalid(DateTime localTime) => AmsterdamTimeZone.IsInvalidTime(localTime);

    public static bool IsAmbiguous(DateTime localTime) => AmsterdamTimeZone.IsAmbiguousTime(localTime);
}
