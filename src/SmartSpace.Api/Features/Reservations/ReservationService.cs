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

public sealed record UpdateReservationCommand(
    Guid ResourceId,
    DateTimeOffset Start,
    DateTimeOffset End,
    Guid Version);

public sealed record ReservationResult(
    Guid Id,
    Guid ResourceId,
    string ResourceName,
    string LocationName,
    Guid LocationId,
    int Capacity,
    DateTimeOffset Start,
    DateTimeOffset End,
    ReservationStatus Status,
    Guid Version);

public sealed class ReservationValidationException(string message) : Exception(message);

public sealed class ReservationConflictException(string message, Exception? innerException = null)
    : Exception(message, innerException);

public sealed class ReservationNotFoundException : Exception;

public sealed class ReservationStaleVersionException : Exception;

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
                UpdatedAtUtc = DateTimeOffset.UtcNow,
                ResourceNameAtBooking = resource.Name,
                LocationNameAtBooking = resource.Location!.Name,
                CapacityAtBooking = resource.Capacity
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

    public async Task<ReservationResult> UpdateAsync(
        Guid id,
        UpdateReservationCommand command,
        string subjectId,
        bool isAdministrator,
        CancellationToken cancellationToken = default)
    {
        var start = command.Start.ToUniversalTime();
        var end = command.End.ToUniversalTime();
        ValidateFutureInterval(start, end);

        await using var transaction = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            var reservation = await db.Reservations
                .Include(item => item.Resource!)
                .ThenInclude(resource => resource.Location)
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

            EnsureAccessible(reservation, subjectId, isAdministrator);
            if (reservation!.Status != ReservationStatus.Active || reservation.StartUtc <= DateTimeOffset.UtcNow)
            {
                throw new ReservationValidationException("Only future active reservations can be changed.");
            }

            if (reservation.Version != command.Version)
            {
                throw new ReservationStaleVersionException();
            }

            var resource = await db.Resources
                .Include(item => item.Location)
                .SingleOrDefaultAsync(item => item.Id == command.ResourceId, cancellationToken);
            if (resource is null || !resource.IsActive || resource.Location is null || !resource.Location.IsActive)
            {
                throw new ReservationValidationException("The selected room is not active.");
            }

            var overlapExists = await db.Reservations.AnyAsync(item => item.Id != id
                && item.ResourceId == resource.Id
                && item.Status == ReservationStatus.Active
                && item.StartUtc < end
                && item.EndUtc > start, cancellationToken);
            if (overlapExists)
            {
                throw new ReservationConflictException("The selected room is already reserved for this interval.");
            }

            reservation.ResourceId = resource.Id;
            reservation.StartUtc = start;
            reservation.EndUtc = end;
            reservation.UpdatedAtUtc = DateTimeOffset.UtcNow;
            reservation.Version = Guid.NewGuid();
            reservation.ResourceNameAtBooking = resource.Name;
            reservation.LocationNameAtBooking = resource.Location!.Name;
            reservation.CapacityAtBooking = resource.Capacity;
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return ToResult(reservation, resource);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new ReservationStaleVersionException();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> CancelAsync(
        Guid id,
        Guid version,
        string subjectId,
        bool isAdministrator,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            var reservation = await db.Reservations
                .Include(item => item.Resource!)
                .ThenInclude(resource => resource.Location)
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
            EnsureAccessible(reservation, subjectId, isAdministrator);
            if (reservation!.Status == ReservationStatus.Cancelled)
            {
                await transaction.CommitAsync(cancellationToken);
                return false;
            }

            if (reservation.StartUtc <= DateTimeOffset.UtcNow)
            {
                throw new ReservationValidationException("Only future active reservations can be cancelled.");
            }

            if (reservation.Version != version)
            {
                throw new ReservationStaleVersionException();
            }

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelledAtUtc = DateTimeOffset.UtcNow;
            reservation.UpdatedAtUtc = DateTimeOffset.UtcNow;
            reservation.Version = Guid.NewGuid();
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new ReservationStaleVersionException();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void ValidateFutureInterval(DateTimeOffset start, DateTimeOffset end)
    {
        if (!BookingIntervalRules.IsValid(start, end))
        {
            throw new ReservationValidationException("End must be after Start.");
        }

        if (start <= DateTimeOffset.UtcNow)
        {
            throw new ReservationValidationException("Reservations must start in the future.");
        }
    }

    private static void EnsureAccessible(Reservation? reservation, string subjectId, bool isAdministrator)
    {
        if (reservation is null || (!isAdministrator && reservation.OwnerSubjectId != subjectId))
        {
            throw new ReservationNotFoundException();
        }
    }

    private static bool IsTransientSqlConflict(DbUpdateException exception) =>
        exception.InnerException is SqlException sqlException && sqlException.Number is 1205 or 1222;

    private static ReservationResult ToResult(Reservation reservation, Resource resource) =>
        new(
            reservation.Id,
            reservation.ResourceId,
            reservation.ResourceNameAtBooking,
            reservation.LocationNameAtBooking,
            resource.LocationId,
            reservation.CapacityAtBooking,
            reservation.StartUtc,
            reservation.EndUtc,
            reservation.Status,
            reservation.Version);
}
