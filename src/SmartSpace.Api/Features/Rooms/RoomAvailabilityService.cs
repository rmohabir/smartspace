using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Data;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Features.Rooms;

public sealed record RoomAvailabilityRequest(
    Guid? LocationId,
    DateTimeOffset Start,
    DateTimeOffset End,
    int MinimumCapacity);

public sealed record RoomAvailabilityResult(
    Guid Id,
    string Name,
    Guid LocationId,
    int Capacity);

public sealed class RoomAvailabilityService(SmartSpaceDbContext db)
{
    public async Task<IReadOnlyList<RoomAvailabilityResult>> GetAvailableRoomsAsync(RoomAvailabilityRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.End <= request.Start)
        {
            throw new ArgumentException("End must be after Start.", nameof(request));
        }

        if (request.MinimumCapacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Minimum capacity must be at least 1.");
        }

        var overlappingResourceIds = await db.Reservations
            .Where(reservation => reservation.Status == ReservationStatus.Active)
            .Where(reservation => reservation.StartUtc < request.End && reservation.EndUtc > request.Start)
            .Select(reservation => reservation.ResourceId)
            .Distinct()
            .ToListAsync();

        var availableRooms = await db.Resources
            .AsNoTracking()
            .Include(resource => resource.Location)
            .Where(resource => resource.IsActive)
            .Where(resource => resource.Location != null && resource.Location.IsActive)
            .Where(resource => request.LocationId == null || resource.LocationId == request.LocationId)
            .Where(resource => resource.ResourceType == ResourceType.MeetingRoom)
            .Where(resource => resource.Capacity >= request.MinimumCapacity)
            .Where(resource => !overlappingResourceIds.Contains(resource.Id))
            .OrderBy(resource => resource.Name)
            .Select(resource => new RoomAvailabilityResult(
                resource.Id,
                resource.Name,
                resource.LocationId,
                resource.Capacity))
            .ToListAsync();

        return availableRooms;
    }
}
