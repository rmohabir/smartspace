using Xunit;

namespace SmartSpace.IntegrationTests.Infrastructure;

[CollectionDefinition("SQL Server")]
public sealed class SqlServerFixtureCollection : ICollectionFixture<SqlServerFixture>
{
}
