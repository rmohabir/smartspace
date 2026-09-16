using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Reservations;

[Collection("SQL Server")]
public sealed class FailedUpdatePreservesOriginalTests(SqlServerFixture fixture)
{
    [Fact(Skip = "Requires a running SQL Server container and implemented reservation mutation service.")]
    public Task Rejected_update_preserves_original_reservation()
    {
        Assert.True(fixture.IsAvailable);
        return Task.CompletedTask;
    }
}
