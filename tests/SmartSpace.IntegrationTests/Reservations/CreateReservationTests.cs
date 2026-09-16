using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Data;
using SmartSpace.Api.Domain;
using SmartSpace.Api.Features.Reservations;
using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Reservations;

[Collection("SQL Server")]
public sealed class CreateReservationTests(SqlServerFixture fixture)
{
    [Fact]
    public async Task Creates_future_reservation_with_server_derived_owner()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var resource = await SeedRoomAsync();
        var start = DateTimeOffset.UtcNow.AddHours(2);
        await using var db = fixture.CreateDbContext();
        var result = await new ReservationService(db).CreateAsync(
            new CreateReservationCommand(resource.Id, start, start.AddHours(1), "employee-123"));

        Assert.Equal(resource.Id, result.ResourceId);
        Assert.Equal("employee-123", await db.Reservations
            .Where(item => item.Id == result.Id)
            .Select(item => item.OwnerSubjectId)
            .SingleAsync());
    }

    [Fact]
    public async Task Rejects_invalid_time_and_non_future_start()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var resource = await SeedRoomAsync();
        await using var db = fixture.CreateDbContext();
        var service = new ReservationService(db);
        var start = DateTimeOffset.UtcNow.AddHours(2);

        await Assert.ThrowsAsync<ReservationValidationException>(() => service.CreateAsync(
            new CreateReservationCommand(resource.Id, start, start, "employee-123")));
        await Assert.ThrowsAsync<ReservationValidationException>(() => service.CreateAsync(
            new CreateReservationCommand(resource.Id, DateTimeOffset.UtcNow.AddMinutes(-1), start, "employee-123")));
    }

    [Fact]
    public async Task Rejects_inactive_room()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var resource = await SeedRoomAsync(isActive: false);
        await using var db = fixture.CreateDbContext();
        var start = DateTimeOffset.UtcNow.AddHours(2);

        await Assert.ThrowsAsync<ReservationValidationException>(() => new ReservationService(db).CreateAsync(
            new CreateReservationCommand(resource.Id, start, start.AddHours(1), "employee-123")));
    }

    [Fact]
    public async Task Rejects_active_overlap_but_allows_adjacent_interval()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var resource = await SeedRoomAsync();
        var start = DateTimeOffset.UtcNow.AddHours(2);
        await using var db = fixture.CreateDbContext();
        var service = new ReservationService(db);
        await service.CreateAsync(new CreateReservationCommand(resource.Id, start, start.AddHours(1), "employee-123"));

        await Assert.ThrowsAsync<ReservationConflictException>(() => service.CreateAsync(
            new CreateReservationCommand(resource.Id, start.AddMinutes(30), start.AddHours(1).AddMinutes(30), "employee-456")));

        var adjacent = await service.CreateAsync(
            new CreateReservationCommand(resource.Id, start.AddHours(1), start.AddHours(2), "employee-456"));
        Assert.Equal("employee-456", await db.Reservations
            .Where(item => item.Id == adjacent.Id)
            .Select(item => item.OwnerSubjectId)
            .SingleAsync());
    }

    [Fact]
    public async Task Concurrent_creates_leave_at_most_one_active_overlap()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        var resource = await SeedRoomAsync();
        var start = DateTimeOffset.UtcNow.AddHours(3);
        var commands = new[]
        {
            new CreateReservationCommand(resource.Id, start, start.AddHours(1), "employee-1"),
            new CreateReservationCommand(resource.Id, start, start.AddHours(1), "employee-2")
        };

        var outcomes = await Task.WhenAll(commands.Select(async command =>
        {
            await using var db = fixture.CreateDbContext();
            try
            {
                return (Success: true, Result: (ReservationResult?)await new ReservationService(db).CreateAsync(command));
            }
            catch (ReservationConflictException)
            {
                return (Success: false, Result: null);
            }
        }));

        Assert.Single(outcomes, item => item.Success);
        await using var verificationDb = fixture.CreateDbContext();
        Assert.Equal(1, await verificationDb.Reservations.CountAsync(item =>
            item.ResourceId == resource.Id && item.Status == ReservationStatus.Active));
    }

    private async Task<Resource> SeedRoomAsync(bool isActive = true)
    {
        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();
        var location = new Location { Name = "Booking HQ", Building = "A", Floor = "1" };
        var resource = new Resource { Name = "Booking room", Capacity = 8, Location = location, IsActive = isActive };
        db.Locations.Add(location);
        db.Resources.Add(resource);
        await db.SaveChangesAsync();
        return resource;
    }
}
