using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Data;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Features.Reservations;

public enum ReservationView
{
    Upcoming,
    Past,
    Cancelled
}

public sealed class OwnReservationQueryService(SmartSpaceDbContext db)
{
    public async Task<IReadOnlyList<ReservationResult>> GetAsync(
        string ownerSubjectId,
        ReservationView view,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            throw new ReservationValidationException("Page must be at least 1 and pageSize must be between 1 and 100.");
        }

        var now = DateTimeOffset.UtcNow;
        var query = db.Reservations
            .AsNoTracking()
            .Where(item => item.OwnerSubjectId == ownerSubjectId);

        query = view switch
        {
            ReservationView.Upcoming => query.Where(item => item.Status == ReservationStatus.Active && item.EndUtc >= now),
            ReservationView.Past => query.Where(item => item.Status == ReservationStatus.Active && item.EndUtc < now),
            ReservationView.Cancelled => query.Where(item => item.Status == ReservationStatus.Cancelled),
            _ => query
        };

        return await query
            .OrderBy(item => item.StartUtc)
            .ThenBy(item => item.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new ReservationResult(
                item.Id,
                item.ResourceId,
                item.ResourceNameAtBooking,
                item.LocationNameAtBooking,
                item.Resource!.LocationId,
                item.CapacityAtBooking,
                item.StartUtc,
                item.EndUtc,
                item.Status,
                item.Version))
            .ToListAsync(cancellationToken);
    }

    public async Task<ReservationResult?> GetByIdAsync(
        Guid id,
        string subjectId,
        bool isAdministrator,
        CancellationToken cancellationToken = default)
    {
        return await db.Reservations
            .AsNoTracking()
            .Where(item => item.Id == id && (isAdministrator || item.OwnerSubjectId == subjectId))
            .Select(item => new ReservationResult(
                item.Id,
                item.ResourceId,
                item.ResourceNameAtBooking,
                item.LocationNameAtBooking,
                item.Resource!.LocationId,
                item.CapacityAtBooking,
                item.StartUtc,
                item.EndUtc,
                item.Status,
                item.Version))
            .SingleOrDefaultAsync(cancellationToken);
    }
}