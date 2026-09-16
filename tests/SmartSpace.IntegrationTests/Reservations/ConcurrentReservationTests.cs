using SmartSpace.Api.Domain;
using SmartSpace.IntegrationTests.Infrastructure;

namespace SmartSpace.IntegrationTests.Reservations;

[Collection("SQL Server")]
public sealed class ConcurrentReservationTests(SqlServerFixture fixture)
{
    [Fact]
    public async Task Concurrent_overlapping_creates_leave_at_most_one_active_row()
    {
        if (!fixture.IsAvailable)
        {
            return;
        }

        await fixture.ResetAsync();
        await using var db = fixture.CreateDbContext();

        var location = new Location { Name = "HQ", Building = "A", Floor = "2" };
        var resource = new Resource { Name = "Den Haag", Capacity = 8, Location = location, IsActive = true };

        db.Locations.Add(location);
        db.Resources.Add(resource);
        await db.SaveChangesAsync();

        var start = new DateTimeOffset(2026, 09, 16, 09, 00, 00, TimeSpan.Zero);
        var end = start.AddHours(1);

        var candidateReservations = new[]
        {
            new Reservation { ResourceId = resource.Id, OwnerSubjectId = "emp-1", StartUtc = start, EndUtc = end, Status = ReservationStatus.Active },
            new Reservation { ResourceId = resource.Id, OwnerSubjectId = "emp-2", StartUtc = start, EndUtc = end, Status = ReservationStatus.Active },
            new Reservation { ResourceId = resource.Id, OwnerSubjectId = "emp-3", StartUtc = start.AddMinutes(30), EndUtc = end.AddMinutes(30), Status = ReservationStatus.Active }
        };

        var successfulCandidates = candidateReservations
            .Where(candidate =>
            {
                var existing = db.Reservations
                    .AsEnumerable()
                    .Where(existingReservation => existingReservation.ResourceId == candidate.ResourceId
                        && existingReservation.Status == ReservationStatus.Active)
                    .ToList();

                return !existing.Any(existingReservation =>
                    BookingIntervalRules.Overlaps(
                        existingReservation.StartUtc,
                        existingReservation.EndUtc,
                        candidate.StartUtc,
                        candidate.EndUtc));
            })
            .ToList();

        Assert.InRange(successfulCandidates.Count, 0, 1);
        Assert.Equal(1, db.Reservations.Count(r => r.ResourceId == resource.Id && r.Status == ReservationStatus.Active));
    }
}
