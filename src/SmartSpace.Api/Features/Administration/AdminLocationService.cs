using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Contracts;
using SmartSpace.Api.Data;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Features.Administration;

public sealed record AdminLocationResult(
    Guid Id,
    string Name,
    string? Building,
    string? Floor,
    bool IsActive,
    Guid Version);

public sealed class AdminLocationValidationException(string message) : Exception(message);

public sealed class AdminLocationNotFoundException : Exception;

public sealed class AdminLocationConflictException(string message) : Exception(message);

public sealed class AdminLocationStaleVersionException : Exception;

public sealed class AdminLocationService(SmartSpaceDbContext db)
{
    public async Task<IReadOnlyList<AdminLocationResult>> ListAsync(CancellationToken cancellationToken = default) =>
        await db.Locations
            .AsNoTracking()
            .OrderBy(location => location.Name)
            .Select(location => new AdminLocationResult(
                location.Id,
                location.Name,
                location.Building,
                location.Floor,
                location.IsActive,
                location.Version))
            .ToListAsync(cancellationToken);

    public async Task<AdminLocationResult> CreateAsync(
        LocationCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request.Name);
        var location = new Location
        {
            Name = request.Name.Trim(),
            Building = Normalize(request.Building),
            Floor = Normalize(request.Floor)
        };
        db.Locations.Add(location);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraint(exception))
        {
            throw new AdminLocationConflictException("A location with this name already exists.");
        }

        return ToResult(location);
    }

    public async Task<AdminLocationResult> UpdateAsync(
        Guid id,
        LocationUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request.Name);
        var location = await db.Locations.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new AdminLocationNotFoundException();
        if (location.Version != request.Version)
        {
            throw new AdminLocationStaleVersionException();
        }

        location.Name = request.Name.Trim();
        location.Building = Normalize(request.Building);
        location.Floor = Normalize(request.Floor);
        location.Version = Guid.NewGuid();
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new AdminLocationStaleVersionException();
        }
        catch (DbUpdateException exception) when (IsUniqueConstraint(exception))
        {
            throw new AdminLocationConflictException("A location with this name already exists.");
        }

        return ToResult(location);
    }

    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new AdminLocationValidationException("A location name is required.");
        }
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsUniqueConstraint(DbUpdateException exception) =>
        exception.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true
        || exception.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true;

    private static AdminLocationResult ToResult(Location location) =>
        new(location.Id, location.Name, location.Building, location.Floor, location.IsActive, location.Version);

}