using Microsoft.AspNetCore.Mvc;
using SmartSpace.Api.Contracts;
using SmartSpace.Api.Security;

namespace SmartSpace.Api.Features.Administration;

public static class AdminRoomEndpoints
{
    public static void MapAdminRoomEndpoints(this WebApplication app)
    {
        var admin = app.MapGroup("/api/admin/rooms")
            .RequireAuthorization(AuthorizationPolicies.Administrator);

        admin.MapGet("", async (AdminRoomService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListAsync(cancellationToken)));

        admin.MapPost("", async (
            [FromBody] RoomCreateRequest request,
            AdminRoomService service,
            CancellationToken cancellationToken) =>
            await ExecuteAsync(() => service.CreateAsync(request, cancellationToken)));

        admin.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] RoomUpdateRequest request,
            AdminRoomService service,
            CancellationToken cancellationToken) =>
            await ExecuteAsync(() => service.UpdateAsync(id, request, cancellationToken)));

        admin.MapPost("/{id:guid}/deactivate", async (
            Guid id,
            [FromBody] VersionRequest request,
            AdminRoomService service,
            CancellationToken cancellationToken) =>
            await ExecuteAsync(() => service.DeactivateAsync(id, request.Version, cancellationToken)));

        admin.MapPost("/{id:guid}/reactivate", async (
            Guid id,
            [FromBody] VersionRequest request,
            AdminRoomService service,
            CancellationToken cancellationToken) =>
            await ExecuteAsync(() => service.ReactivateAsync(id, request.Version, cancellationToken)));
    }

    private static async Task<IResult> ExecuteAsync(Func<Task<AdminRoomResult>> operation)
    {
        try
        {
            return Results.Ok(await operation());
        }
        catch (AdminRoomValidationException exception)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest,
                title: "Ongeldige ruimte", detail: exception.Message,
                extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
        }
        catch (AdminRoomNotFoundException)
        {
            return Results.NotFound();
        }
        catch (AdminRoomStaleVersionException)
        {
            return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Ruimte gewijzigd", detail: "Laad de ruimte opnieuw.",
                extensions: new Dictionary<string, object?> { ["code"] = "stale_version" });
        }
        catch (AdminRoomConflictException exception)
        {
            return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Ruimte kan niet worden gewijzigd", detail: exception.Message,
                extensions: new Dictionary<string, object?> { ["code"] = "policy_conflict" });
        }
    }
}