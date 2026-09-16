using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Domain;
using SmartSpace.Api.Features.Rooms;
using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Rooms;

[Collection("SQL Server")]
public sealed class AvailabilityEndpointTests(SqlServerFixture fixture)
{
    [Fact]
    public async Task Availability_returns_only_active_rooms_matching_location_and_capacity()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var (location, _) = await SeedRoomsAsync();
        await using var db = fixture.CreateDbContext();
        var service = new RoomAvailabilityService(db);

        var result = await service.GetAvailableRoomsAsync(new RoomAvailabilityRequest(
            location.Id,
            Start,
            End,
            6));

        Assert.Equal(2, result.Count);
        Assert.Contains(result, room => room.Name == "Berk" && room.Capacity == 12);
        Assert.Contains(result, room => room.Name == "Orchidee" && room.Capacity == 8);
    }

    [Fact]
    public async Task Availability_excludes_active_overlaps_but_keeps_cancelled_and_adjacent_rooms_available()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var (location, rooms) = await SeedRoomsAsync();
        await using var db = fixture.CreateDbContext();
        db.Reservations.AddRange(
            new Reservation
            {
                ResourceId = rooms[0].Id,
                OwnerSubjectId = "employee-1",
                StartUtc = Start.AddMinutes(15),
                EndUtc = End.AddMinutes(-15),
                Status = ReservationStatus.Active
            },
            new Reservation
            {
                ResourceId = rooms[1].Id,
                OwnerSubjectId = "employee-2",
                StartUtc = End,
                EndUtc = End.AddHours(1),
                Status = ReservationStatus.Active
            },
            new Reservation
            {
                ResourceId = rooms[2].Id,
                OwnerSubjectId = "employee-3",
                StartUtc = Start,
                EndUtc = End,
                Status = ReservationStatus.Cancelled
            });
        await db.SaveChangesAsync();

        var service = new RoomAvailabilityService(db);
        var result = await service.GetAvailableRoomsAsync(new RoomAvailabilityRequest(
            location.Id,
            Start,
            End,
            1));

        Assert.DoesNotContain(result, room => room.Id == rooms[0].Id);
        Assert.Contains(result, room => room.Id == rooms[1].Id);
        Assert.Contains(result, room => room.Id == rooms[2].Id);
    }

    [Fact]
    public async Task Availability_returns_empty_when_no_room_matches()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var (location, _) = await SeedRoomsAsync();
        await using var db = fixture.CreateDbContext();
        var service = new RoomAvailabilityService(db);

        var result = await service.GetAvailableRoomsAsync(new RoomAvailabilityRequest(
            location.Id,
            Start,
            End,
            50));

        Assert.Empty(result);
    }

    [Fact]
    public async Task Availability_rejects_invalid_intervals_and_capacity()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        await SeedRoomsAsync();
        await using var db = fixture.CreateDbContext();
        var service = new RoomAvailabilityService(db);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetAvailableRoomsAsync(
            new RoomAvailabilityRequest(null, End, Start, 1)));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetAvailableRoomsAsync(
            new RoomAvailabilityRequest(null, Start, End, 0)));
    }

    private static readonly DateTimeOffset Start = new(2026, 9, 16, 9, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = Start.AddHours(1);

    private async Task<(Location Location, List<Resource> Rooms)> SeedRoomsAsync()
    {
        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();

        var location = new Location { Name = "BIDN HQ", Building = "A", Floor = "2" };
        var rooms = new List<Resource>
        {
            new() { Name = "Acacia", Capacity = 4, Location = location, IsActive = true },
            new() { Name = "Berk", Capacity = 12, Location = location, IsActive = true },
            new() { Name = "Orchidee", Capacity = 8, Location = location, IsActive = true },
            new() { Name = "Gesloten", Capacity = 20, Location = location, IsActive = false }
        };

        db.Locations.Add(location);
        db.Resources.AddRange(rooms);
        await db.SaveChangesAsync();
        return (location, rooms);
    }
}
