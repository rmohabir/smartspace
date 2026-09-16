using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Data;

namespace SmartSpace.IntegrationTests.Infrastructure;

public sealed class SqlServerFixture : IAsyncLifetime
{
    public string? ConnectionString => Environment.GetEnvironmentVariable("SMARTSPACE_SQLSERVER_CONNECTION");
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

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}
