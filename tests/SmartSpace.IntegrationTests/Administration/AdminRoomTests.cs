using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Domain;
using SmartSpace.Api.Features.Administration;
using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Administration;

[Collection("SQL Server")]
public sealed class AdminRoomTests(SqlServerFixture fixture)
{
    [Fact]
    public async Task Creates_and_updates_room_with_new_version()
    {
        if (!fixture.IsAvailable) return;
        var location = await SeedLocationAsync();
        await using var db = fixture.CreateDbContext();
        var service = new AdminRoomService(db);

        var created = await service.CreateAsync(new SmartSpace.Api.Contracts.RoomCreateRequest("Atlas", 8, location.Id));
        var updated = await service.UpdateAsync(created.Id,
            new SmartSpace.Api.Contracts.RoomUpdateRequest("Atlas Plus", 12, location.Id, created.Version));

        Assert.Equal("Atlas Plus", updated.Name);
        Assert.Equal(12, updated.Capacity);
        Assert.NotEqual(created.Version, updated.Version);
    }

    [Fact]
    public async Task Rejects_invalid_input_and_stale_versions_without_changing_room()
    {
        if (!fixture.IsAvailable) return;
        var location = await SeedLocationAsync();
        await using var db = fixture.CreateDbContext();
        var service = new AdminRoomService(db);
        var created = await service.CreateAsync(new SmartSpace.Api.Contracts.RoomCreateRequest("Borealis", 4, location.Id));

        await Assert.ThrowsAsync<AdminRoomValidationException>(() => service.CreateAsync(
            new SmartSpace.Api.Contracts.RoomCreateRequest(" ", 0, location.Id)));
        await Assert.ThrowsAsync<AdminRoomStaleVersionException>(() => service.UpdateAsync(created.Id,
            new SmartSpace.Api.Contracts.RoomUpdateRequest("Changed", 10, location.Id, Guid.NewGuid())));

        var unchanged = await db.Resources.SingleAsync(room => room.Id == created.Id);
        Assert.Equal("Borealis", unchanged.Name);
        Assert.Equal(4, unchanged.Capacity);
        Assert.Equal(created.Version, unchanged.Version);
    }

    [Fact]
    public async Task Deactivation_is_blocked_for_running_booking_and_preserves_history()
    {
        if (!fixture.IsAvailable) return;
        var location = await SeedLocationAsync();
        await using var db = fixture.CreateDbContext();
        var room = new Resource { Name = "Current", Capacity = 6, LocationId = location.Id };
        db.Resources.Add(room);
        await db.SaveChangesAsync();
        var reservation = new Reservation
        {
            ResourceId = room.Id,
            OwnerSubjectId = "employee",
            StartUtc = DateTimeOffset.UtcNow.AddMinutes(-10),
            EndUtc = DateTimeOffset.UtcNow.AddMinutes(20),
            Status = ReservationStatus.Active,
            ResourceNameAtBooking = room.Name,
            LocationNameAtBooking = location.Name,
            CapacityAtBooking = room.Capacity
        };
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        var service = new AdminRoomService(db);
        await Assert.ThrowsAsync<AdminRoomConflictException>(() => service.DeactivateAsync(room.Id, room.Version));

        var stored = await db.Resources.SingleAsync(item => item.Id == room.Id);
        Assert.True(stored.IsActive);
        Assert.Equal(reservation.Id, await db.Reservations.Select(item => item.Id).SingleAsync());
    }

    [Fact]
    public async Task Deactivate_reactivate_preserves_reservations_and_changes_versions()
    {
        if (!fixture.IsAvailable) return;
        var location = await SeedLocationAsync();
        await using var db = fixture.CreateDbContext();
        var service = new AdminRoomService(db);
        var created = await service.CreateAsync(new SmartSpace.Api.Contracts.RoomCreateRequest("Comet", 5, location.Id));

        var deactivated = await service.DeactivateAsync(created.Id, created.Version);
        var reactivated = await service.ReactivateAsync(created.Id, deactivated.Version);

        Assert.False(deactivated.IsActive);
        Assert.True(reactivated.IsActive);
        Assert.NotEqual(created.Version, reactivated.Version);
    }

    private async Task<Location> SeedLocationAsync()
    {
        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();
        var location = new Location { Name = "Admin HQ", Building = "A", Floor = "1" };
        db.Locations.Add(location);
        await db.SaveChangesAsync();
        return location;
    }
}