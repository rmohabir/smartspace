namespace SmartSpace.Api.Domain;

public static class AuthorizationRules
{
    public static bool OwnsReservation(string ownerSubjectId, string? currentSubjectId) =>
        !string.IsNullOrWhiteSpace(currentSubjectId) &&
        string.Equals(ownerSubjectId, currentSubjectId, StringComparison.Ordinal);

    public static bool IsAdministrator(string? role) =>
        string.Equals(role, "Administrator", StringComparison.Ordinal);
}
