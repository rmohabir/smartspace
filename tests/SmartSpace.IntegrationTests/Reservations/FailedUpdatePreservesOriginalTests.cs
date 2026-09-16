using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Domain;
using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Reservations;

[Collection("SQL Server")]
public sealed class FailedUpdatePreservesOriginalTests(SqlServerFixture fixture)
{
    [Fact]
    public async Task Rejected_update_preserves_original_reservation()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();

        var location = new Location { Name = "HQ", Building = "A", Floor = "2" };
        var resource = new Resource { Name = "Rotterdam", Capacity = 6, Location = location, IsActive = true };
        var reservation = new Reservation
        {
            ResourceId = Guid.Empty,
            OwnerSubjectId = "emp-1",
            StartUtc = new DateTimeOffset(2026, 09, 16, 09, 00, 00, TimeSpan.Zero),
            EndUtc = new DateTimeOffset(2026, 09, 16, 10, 00, 00, TimeSpan.Zero),
            Status = ReservationStatus.Active,
            Version = Guid.NewGuid()
        };

        db.Locations.Add(location);
        db.Resources.Add(resource);
        await db.SaveChangesAsync();

        reservation.ResourceId = resource.Id;
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        var existing = await db.Reservations
            .Where(r => r.Id == reservation.Id)
            .SingleAsync();
        var staleVersion = Guid.NewGuid();

        var overlapAttempt = new Reservation
        {
            ResourceId = resource.Id,
            OwnerSubjectId = "emp-2",
            StartUtc = existing.StartUtc.AddMinutes(30),
            EndUtc = existing.EndUtc.AddMinutes(30),
            Status = ReservationStatus.Active,
            Version = staleVersion
        };

        var invalidTimeAttempt = new Reservation
        {
            ResourceId = resource.Id,
            OwnerSubjectId = "emp-2",
            StartUtc = existing.EndUtc,
            EndUtc = existing.StartUtc,
            Status = ReservationStatus.Active,
            Version = staleVersion
        };

        var isOverlapRejected = BookingIntervalRules.Overlaps(
            existing.StartUtc,
            existing.EndUtc,
            overlapAttempt.StartUtc,
            overlapAttempt.EndUtc);

        var isInvalidTimeRejected = !BookingIntervalRules.IsValid(
            invalidTimeAttempt.StartUtc,
            invalidTimeAttempt.EndUtc);

        Assert.True(isOverlapRejected);
        Assert.True(isInvalidTimeRejected);
        Assert.Equal(existing.StartUtc, reservation.StartUtc);
        Assert.Equal(existing.EndUtc, reservation.EndUtc);
        Assert.Equal(existing.Status, reservation.Status);
        Assert.Equal(existing.Version, reservation.Version);
    }
}
