using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Reservations;

[Collection("SQL Server")]
public sealed class ConcurrentReservationTests(SqlServerFixture fixture)
{
    [Fact(Skip = "Requires a running SQL Server container and implemented reservation mutation service.")]
    public Task Concurrent_overlapping_creates_leave_at_most_one_active_row()
    {
        Assert.True(fixture.IsAvailable);
        return Task.CompletedTask;
    }
}
