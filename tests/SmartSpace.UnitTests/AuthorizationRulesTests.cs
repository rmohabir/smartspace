using SmartSpace.Api.Domain;

namespace SmartSpace.UnitTests;

public sealed class AuthorizationRulesTests
{
    [Fact]
    public void Requires_the_server_subject_to_match_owner()
    {
        Assert.True(AuthorizationRules.OwnsReservation("employee-1", "employee-1"));
        Assert.False(AuthorizationRules.OwnsReservation("employee-1", "employee-2"));
        Assert.False(AuthorizationRules.OwnsReservation("employee-1", null));
    }

    [Fact]
    public void Recognizes_only_the_administrator_role()
    {
        Assert.True(AuthorizationRules.IsAdministrator("Administrator"));
        Assert.False(AuthorizationRules.IsAdministrator("Employee"));
        Assert.False(AuthorizationRules.IsAdministrator(null));
    }

    [Fact]
    public void Treats_users_without_administrator_role_as_non_administrators()
    {
        Assert.False(AuthorizationRules.IsAdministrator(string.Empty));
        Assert.False(AuthorizationRules.IsAdministrator("administrator"));
    }
}
