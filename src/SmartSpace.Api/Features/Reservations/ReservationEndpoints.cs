using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartSpace.Api.Contracts;

namespace SmartSpace.Api.Features.Reservations;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(this WebApplication app)
    {
        app.MapGet("/api/reservations/mine", async (
            string? view,
            int? page,
            int? pageSize,
            ClaimsPrincipal user,
            OwnReservationQueryService queries,
            CancellationToken cancellationToken) =>
        {
            var subjectId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                return Results.Unauthorized();
            }

            if (!TryParseView(view, out var reservationView))
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: "Ongeldige reserveringsweergave",
                    detail: "Gebruik upcoming, past of cancelled.",
                    extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
            }

            try
            {
                return Results.Ok(await queries.GetAsync(subjectId, reservationView, page ?? 1,
                    pageSize ?? 20, cancellationToken));
            }
            catch (ReservationValidationException exception)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: "Ongeldige paginering", detail: exception.Message,
                    extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
            }
        }).RequireAuthorization();

        app.MapGet("/api/reservations/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            OwnReservationQueryService queries,
            CancellationToken cancellationToken) =>
        {
            var subjectId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                return Results.Unauthorized();
            }

            var result = await queries.GetByIdAsync(id, subjectId, IsAdministrator(user), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization();

        app.MapPost("/api/reservations", async (
            [FromBody] CreateReservationRequest request,
            ClaimsPrincipal user,
            ReservationService service,
            CancellationToken cancellationToken) =>
        {
            var ownerSubjectId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(ownerSubjectId))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Authenticatie vereist",
                    detail: "Log in om een reservering te maken.",
                    extensions: new Dictionary<string, object?> { ["code"] = "unauthorized" });
            }

            try
            {
                var result = await service.CreateAsync(
                    new CreateReservationCommand(request.ResourceId, request.Start, request.End, ownerSubjectId),
                    cancellationToken);

                return Results.Created($"/api/reservations/{result.Id}", result);
            }
            catch (ReservationValidationException exception)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Ongeldige reservering",
                    detail: exception.Message,
                    extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
            }
            catch (ReservationConflictException exception)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Ruimte niet beschikbaar",
                    detail: exception.Message,
                    extensions: new Dictionary<string, object?> { ["code"] = ApiErrorCodes.BookingOverlap });
            }
        })
        .RequireAuthorization();

        app.MapPut("/api/reservations/{id:guid}", async (
            Guid id,
            [FromBody] UpdateReservationRequest request,
            ClaimsPrincipal user,
            ReservationService service,
            CancellationToken cancellationToken) =>
        {
            var subjectId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var result = await service.UpdateAsync(id,
                    new UpdateReservationCommand(request.ResourceId, request.Start, request.End, request.Version),
                    subjectId, IsAdministrator(user), cancellationToken);
                return Results.Ok(result);
            }
            catch (ReservationNotFoundException)
            {
                return Results.NotFound();
            }
            catch (ReservationStaleVersionException)
            {
                return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                    title: "Reservering gewijzigd", detail: "Laad de reservering opnieuw.",
                    extensions: new Dictionary<string, object?> { ["code"] = "stale_version" });
            }
            catch (ReservationValidationException exception)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: "Ongeldige reservering", detail: exception.Message,
                    extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
            }
            catch (ReservationConflictException exception)
            {
                return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                    title: "Ruimte niet beschikbaar", detail: exception.Message,
                    extensions: new Dictionary<string, object?> { ["code"] = ApiErrorCodes.BookingOverlap });
            }
        }).RequireAuthorization();

        app.MapPost("/api/reservations/{id:guid}/cancel", async (
            Guid id,
            [FromBody] CancelReservationRequest request,
            ClaimsPrincipal user,
            ReservationService service,
            CancellationToken cancellationToken) =>
        {
            var subjectId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var changed = await service.CancelAsync(id, request.Version, subjectId, IsAdministrator(user), cancellationToken);
                return changed ? Results.Ok() : Results.NoContent();
            }
            catch (ReservationNotFoundException)
            {
                return Results.NotFound();
            }
            catch (ReservationStaleVersionException)
            {
                return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                    title: "Reservering gewijzigd", detail: "Laad de reservering opnieuw.",
                    extensions: new Dictionary<string, object?> { ["code"] = "stale_version" });
            }
            catch (ReservationValidationException exception)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: "Ongeldige annulering", detail: exception.Message,
                    extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
            }
        }).RequireAuthorization();
    }

    private static bool IsAdministrator(ClaimsPrincipal user) => user.IsInRole("Administrator");

    private static bool TryParseView(string? value, out ReservationView view)
    {
        view = value?.ToLowerInvariant() switch
        {
            null or "" or "upcoming" => ReservationView.Upcoming,
            "past" => ReservationView.Past,
            "cancelled" => ReservationView.Cancelled,
            _ => (ReservationView)(-1)
        };
        return Enum.IsDefined(view);
    }
}
