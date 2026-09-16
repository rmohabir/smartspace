using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartSpace.Api.Contracts;

namespace SmartSpace.Api.Features.Reservations;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(this WebApplication app)
    {
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
    }
}
