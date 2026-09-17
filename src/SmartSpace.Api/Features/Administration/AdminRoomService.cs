using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Contracts;
using SmartSpace.Api.Data;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Features.Administration;

public sealed record AdminRoomResult(
    Guid Id,
    string Name,
    int Capacity,
    Guid LocationId,
    string LocationName,
    bool IsActive,
    Guid Version);

public sealed class AdminRoomValidationException(string message) : Exception(message);

public sealed class AdminRoomNotFoundException : Exception;

public sealed class AdminRoomConflictException(string message) : Exception(message);

public sealed class AdminRoomStaleVersionException : Exception;

public sealed class AdminRoomService(SmartSpaceDbContext db)
{
    public async Task<IReadOnlyList<AdminRoomResult>> ListAsync(CancellationToken cancellationToken = default) =>
        await db.Resources
            .AsNoTracking()
            .Where(room => room.ResourceType == ResourceType.MeetingRoom && room.Location != null)
            .OrderBy(room => room.Location!.Name)
            .ThenBy(room => room.Name)
            .Select(room => new AdminRoomResult(
                room.Id, room.Name, room.Capacity, room.LocationId, room.Location!.Name, room.IsActive, room.Version))
            .ToListAsync(cancellationToken);

    public async Task<AdminRoomResult> CreateAsync(
        RoomCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Capacity);
        var location = await GetActiveLocationAsync(request.LocationId, cancellationToken);

        var room = new Resource
        {
            Name = request.Name.Trim(),
            Capacity = request.Capacity,
            LocationId = location.Id,
            Location = location,
            IsActive = true
        };

        db.Resources.Add(room);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraint(exception))
        {
            throw new AdminRoomConflictException("A room with this name already exists at the selected location.");
        }

        return ToResult(room, location);
    }

    public async Task<AdminRoomResult> UpdateAsync(
        Guid id,
        RoomUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Capacity);
        var room = await GetRoomAsync(id, cancellationToken);
        if (room.Version != request.Version)
        {
            throw new AdminRoomStaleVersionException();
        }

        var location = await GetActiveLocationAsync(request.LocationId, cancellationToken);
        room.Name = request.Name.Trim();
        room.Capacity = request.Capacity;
        room.LocationId = location.Id;
        room.Location = location;
        room.Version = Guid.NewGuid();

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new AdminRoomStaleVersionException();
        }
        catch (DbUpdateException exception) when (IsUniqueConstraint(exception))
        {
            throw new AdminRoomConflictException("A room with this name already exists at the selected location.");
        }

        return ToResult(room, location);
    }

    public async Task<AdminRoomResult> DeactivateAsync(
        Guid id,
        Guid version,
        CancellationToken cancellationToken = default)
    {
        var room = await GetRoomAsync(id, cancellationToken);
        if (room.Version != version)
        {
            throw new AdminRoomStaleVersionException();
        }

        var now = DateTimeOffset.UtcNow;
        var hasRunningReservation = await db.Reservations.AnyAsync(reservation =>
            reservation.ResourceId == id
            && reservation.Status == ReservationStatus.Active
            && reservation.StartUtc <= now
            && reservation.EndUtc > now, cancellationToken);
        if (hasRunningReservation)
        {
            throw new AdminRoomConflictException("A room with a running reservation cannot be deactivated.");
        }

        room.IsActive = false;
        room.Version = Guid.NewGuid();
        await db.SaveChangesAsync(cancellationToken);
        return ToResult(room, room.Location!);
    }

    public async Task<AdminRoomResult> ReactivateAsync(
        Guid id,
        Guid version,
        CancellationToken cancellationToken = default)
    {
        var room = await GetRoomAsync(id, cancellationToken);
        if (room.Version != version)
        {
            throw new AdminRoomStaleVersionException();
        }

        if (room.Location is null || !room.Location.IsActive)
        {
            throw new AdminRoomConflictException("A room can only be reactivated on an active location.");
        }

        room.IsActive = true;
        room.Version = Guid.NewGuid();
        await db.SaveChangesAsync(cancellationToken);
        return ToResult(room, room.Location);
    }

    private async Task<Resource> GetRoomAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Resources
            .Include(room => room.Location)
            .SingleOrDefaultAsync(room => room.Id == id, cancellationToken)
        ?? throw new AdminRoomNotFoundException();

    private async Task<Location> GetActiveLocationAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Locations
            .SingleOrDefaultAsync(location => location.Id == id && location.IsActive, cancellationToken)
        ?? throw new AdminRoomValidationException("The selected location is not active.");

    private static void Validate(string name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new AdminRoomValidationException("A room name is required.");
        }

        if (capacity < 1)
        {
            throw new AdminRoomValidationException("Capacity must be at least 1.");
        }
    }

    private static bool IsUniqueConstraint(DbUpdateException exception) =>
        exception.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true
        || exception.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true;

    private static AdminRoomResult ToResult(Resource room, Location location) =>
        new(room.Id, room.Name, room.Capacity, location.Id, location.Name, room.IsActive, room.Version);
}