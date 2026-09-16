using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SmartSpace.Api.Contracts;
using SmartSpace.Api.Data;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Features.Reservations;

public sealed record CreateReservationCommand(
    Guid ResourceId,
    DateTimeOffset Start,
    DateTimeOffset End,
    string OwnerSubjectId);

public sealed record ReservationResult(
    Guid Id,
    Guid ResourceId,
    string ResourceName,
    Guid LocationId,
    DateTimeOffset Start,
    DateTimeOffset End,
    ReservationStatus Status,
    Guid Version);

public sealed class ReservationValidationException(string message) : Exception(message);

public sealed class ReservationConflictException(string message, Exception? innerException = null)
    : Exception(message, innerException);

public sealed class ReservationService(SmartSpaceDbContext db)
{
    public async Task<ReservationResult> CreateAsync(
        CreateReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerSubjectId))
        {
            throw new ReservationValidationException("An authenticated owner is required.");
        }

        var start = command.Start.ToUniversalTime();
        var end = command.End.ToUniversalTime();
        if (!BookingIntervalRules.IsValid(start, end))
        {
            throw new ReservationValidationException("End must be after Start.");
        }

        if (start <= DateTimeOffset.UtcNow)
        {
            throw new ReservationValidationException("Reservations must start in the future.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable,
            cancellationToken);

        try
        {
            var resource = await db.Resources
                .Include(item => item.Location)
                .SingleOrDefaultAsync(item => item.Id == command.ResourceId, cancellationToken);

            if (resource is null || !resource.IsActive || resource.Location is null || !resource.Location.IsActive)
            {
                throw new ReservationValidationException("The selected room is not active.");
            }

            var overlapExists = await db.Reservations
                .AsNoTracking()
                .AnyAsync(item => item.ResourceId == command.ResourceId
                    && item.Status == ReservationStatus.Active
                    && item.StartUtc < end
                    && item.EndUtc > start, cancellationToken);

            if (overlapExists)
            {
                throw new ReservationConflictException("The selected room is already reserved for this interval.");
            }

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                ResourceId = resource.Id,
                OwnerSubjectId = command.OwnerSubjectId,
                StartUtc = start,
                EndUtc = end,
                Status = ReservationStatus.Active,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            };

            db.Reservations.Add(reservation);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return ToResult(reservation, resource);
        }
        catch (ReservationValidationException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (ReservationConflictException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (DbUpdateException exception) when (IsTransientSqlConflict(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new ReservationConflictException(
                "The room became unavailable while your reservation was being saved.", exception);
        }
        catch (SqlException exception) when (exception.Number is 1205 or 1222)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new ReservationConflictException(
                "The room became unavailable while your reservation was being saved.", exception);
        }
    }

    private static bool IsTransientSqlConflict(DbUpdateException exception) =>
        exception.InnerException is SqlException sqlException && sqlException.Number is 1205 or 1222;

    private static ReservationResult ToResult(Reservation reservation, Resource resource) =>
        new(
            reservation.Id,
            reservation.ResourceId,
            resource.Name,
            resource.LocationId,
            reservation.StartUtc,
            reservation.EndUtc,
            reservation.Status,
            reservation.Version);
}
