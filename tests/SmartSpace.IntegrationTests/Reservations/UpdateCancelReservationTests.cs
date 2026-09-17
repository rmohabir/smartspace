using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Domain;
using SmartSpace.Api.Features.Reservations;
using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Reservations;

[Collection("SQL Server")]
public sealed class UpdateCancelReservationTests(SqlServerFixture fixture)
{
    [Fact]
    public async Task Own_queries_are_scoped_and_stably_filtered()
    {
        if (!fixture.IsAvailable) return;
        var resource = await SeedRoomAsync();
        var now = DateTimeOffset.UtcNow;
        await AddReservationAsync(resource, "owner-1", now.AddHours(2), now.AddHours(3));
        await AddReservationAsync(resource, "owner-1", now.AddHours(-3), now.AddHours(-2));
        await AddReservationAsync(resource, "owner-2", now.AddHours(4), now.AddHours(5));
        var cancelled = await AddReservationAsync(resource, "owner-1", now.AddHours(6), now.AddHours(7), ReservationStatus.Cancelled);

        await using var db = fixture.CreateDbContext();
        var queries = new OwnReservationQueryService(db);

        Assert.Single(await queries.GetAsync("owner-1", ReservationView.Upcoming, 1, 20));
        Assert.Single(await queries.GetAsync("owner-1", ReservationView.Past, 1, 20));
        Assert.Equal(cancelled.Id, Assert.Single(await queries.GetAsync("owner-1", ReservationView.Cancelled, 1, 20)).Id);
        Assert.Null(await queries.GetByIdAsync(cancelled.Id, "owner-2", false));
    }

    [Fact]
    public async Task Update_changes_future_reservation_and_replaces_version()
    {
        if (!fixture.IsAvailable) return;
        var resource = await SeedRoomAsync();
        var reservation = await AddReservationAsync(resource, "owner-1", DateTimeOffset.UtcNow.AddHours(2), DateTimeOffset.UtcNow.AddHours(3));
        var oldVersion = reservation.Version;
        await using var db = fixture.CreateDbContext();

        var result = await new ReservationService(db).UpdateAsync(reservation.Id,
            new UpdateReservationCommand(resource.Id, reservation.StartUtc.AddHours(1), reservation.EndUtc.AddHours(1), oldVersion),
            "owner-1", false);

        Assert.NotEqual(oldVersion, result.Version);
        Assert.Equal(reservation.StartUtc.AddHours(1), result.Start);
    }

    [Fact]
    public async Task Stale_update_preserves_original_and_cancel_is_idempotent()
    {
        if (!fixture.IsAvailable) return;
        var resource = await SeedRoomAsync();
        var reservation = await AddReservationAsync(resource, "owner-1", DateTimeOffset.UtcNow.AddHours(2), DateTimeOffset.UtcNow.AddHours(3));
        await using var db = fixture.CreateDbContext();
        var service = new ReservationService(db);

        await Assert.ThrowsAsync<ReservationStaleVersionException>(() => service.UpdateAsync(reservation.Id,
            new UpdateReservationCommand(resource.Id, reservation.StartUtc.AddHours(1), reservation.EndUtc.AddHours(1), Guid.NewGuid()),
            "owner-1", false));

        var unchanged = await db.Reservations.SingleAsync(item => item.Id == reservation.Id);
        Assert.Equal(reservation.StartUtc, unchanged.StartUtc);
        Assert.Equal(reservation.Version, unchanged.Version);

        Assert.True(await service.CancelAsync(reservation.Id, reservation.Version, "owner-1", false));
        Assert.False(await service.CancelAsync(reservation.Id, Guid.NewGuid(), "owner-1", false));
        Assert.Equal(ReservationStatus.Cancelled, await db.Reservations.Where(item => item.Id == reservation.Id).Select(item => item.Status).SingleAsync());
    }

    [Fact]
    public async Task Another_owner_is_hidden_but_administrator_can_manage_history()
    {
        if (!fixture.IsAvailable) return;
        var resource = await SeedRoomAsync();
        var reservation = await AddReservationAsync(resource, "owner-1", DateTimeOffset.UtcNow.AddHours(2), DateTimeOffset.UtcNow.AddHours(3));
        await using var db = fixture.CreateDbContext();
        var service = new ReservationService(db);

        await Assert.ThrowsAsync<ReservationNotFoundException>(() => service.UpdateAsync(reservation.Id,
            new UpdateReservationCommand(resource.Id, reservation.StartUtc.AddHours(1), reservation.EndUtc.AddHours(1), reservation.Version),
            "owner-2", false));

        var updated = await service.UpdateAsync(reservation.Id,
            new UpdateReservationCommand(resource.Id, reservation.StartUtc.AddHours(1), reservation.EndUtc.AddHours(1), reservation.Version),
            "administrator", true);
        Assert.Equal(resource.Name, updated.ResourceName);
        Assert.Equal("US3 HQ", updated.LocationName);
        Assert.Equal(resource.Capacity, updated.Capacity);
    }

    private async Task<Resource> SeedRoomAsync()
    {
        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();
        var location = new Location { Name = "US3 HQ", Building = "A", Floor = "2" };
        var resource = new Resource { Name = "US3 room", Capacity = 8, Location = location, IsActive = true };
        db.Locations.Add(location);
        db.Resources.Add(resource);
        await db.SaveChangesAsync();
        return resource;
    }

    private async Task<Reservation> AddReservationAsync(Resource resource, string owner, DateTimeOffset start,
        DateTimeOffset end, ReservationStatus status = ReservationStatus.Active)
    {
        await using var db = fixture.CreateDbContext();
        var reservation = new Reservation
        {
            ResourceId = resource.Id,
            OwnerSubjectId = owner,
            StartUtc = start,
            EndUtc = end,
            Status = status,
            ResourceNameAtBooking = resource.Name,
            LocationNameAtBooking = "US3 HQ",
            CapacityAtBooking = resource.Capacity,
            CancelledAtUtc = status == ReservationStatus.Cancelled ? DateTimeOffset.UtcNow : null
        };
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();
        return reservation;
    }
}