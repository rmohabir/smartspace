using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Contracts;
using SmartSpace.Api.Domain;
using SmartSpace.Api.Features.Administration;
using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Administration;

[Collection("SQL Server")]
public sealed class AdminLocationTests(SqlServerFixture fixture)
{
    [Fact]
    public async Task Creates_and_updates_location_without_breaking_room_relationships()
    {
        if (!fixture.IsAvailable) return;
        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();
        var service = new AdminLocationService(db);

        var created = await service.CreateAsync(new LocationCreateRequest("Amsterdam", "A", "2"));
        var room = new Resource { Name = "Amsterdam room", Capacity = 8, LocationId = created.Id };
        db.Resources.Add(room);
        await db.SaveChangesAsync();

        var updated = await service.UpdateAsync(created.Id,
            new LocationUpdateRequest("Amsterdam Centrum", "B", "3", created.Version));

        Assert.Equal("Amsterdam Centrum", updated.Name);
        Assert.NotEqual(created.Version, updated.Version);
        Assert.Equal(created.Id, await db.Resources.Where(item => item.Id == room.Id).Select(item => item.LocationId).SingleAsync());
    }

    [Fact]
    public async Task Rejects_empty_duplicate_and_stale_location_changes()
    {
        if (!fixture.IsAvailable) return;
        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();
        var service = new AdminLocationService(db);
        var created = await service.CreateAsync(new LocationCreateRequest("Utrecht", null, null));

        await Assert.ThrowsAsync<AdminLocationValidationException>(() => service.CreateAsync(
            new LocationCreateRequest(" ", null, null)));
        await Assert.ThrowsAsync<AdminLocationStaleVersionException>(() => service.UpdateAsync(created.Id,
            new LocationUpdateRequest("Utrecht gewijzigd", null, null, Guid.NewGuid())));
        await Assert.ThrowsAsync<AdminLocationConflictException>(() => service.CreateAsync(
            new LocationCreateRequest("Utrecht", "C", "1")));
    }
}