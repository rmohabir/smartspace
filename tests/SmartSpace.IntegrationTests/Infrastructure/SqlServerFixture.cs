using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Data;

namespace SmartSpace.IntegrationTests.Infrastructure;

public sealed class SqlServerFixture : IAsyncLifetime
{
    public string? ConnectionString =>
        Environment.GetEnvironmentVariable("SMARTSPACE_SQLSERVER_CONNECTION")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__SmartSpace");

    public bool IsAvailable => !string.IsNullOrWhiteSpace(ConnectionString);

    public SmartSpaceDbContext CreateDbContext()
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException(
                "SMARTSPACE_SQLSERVER_CONNECTION is required for SQL Server integration tests.");
        }

        var options = new DbContextOptionsBuilder<SmartSpaceDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        return new SmartSpaceDbContext(options);
    }

    public async Task InitializeAsync()
    {
        if (!IsAvailable)
        {
            return;
        }

        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();
    }

    public async Task ResetAsync()
    {
        if (!IsAvailable)
        {
            return;
        }

        await using var db = CreateDbContext();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM Reservations; DELETE FROM Resources; DELETE FROM Locations;");
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
