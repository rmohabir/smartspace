namespace SmartSpace.Api.Contracts;

public static class ApiErrorCodes
{
    public const string BookingOverlap = "booking_overlap";
    public const string StaleVersion = "stale_version";
    public const string PolicyConflict = "policy_conflict";
    public const string InvalidLocalTime = "invalid_local_time";
    public const string AmbiguousLocalTime = "ambiguous_local_time";
    public const string PersistenceBusy = "persistence_busy";
}
