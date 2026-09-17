using Microsoft.AspNetCore.Mvc;
using SmartSpace.Api.Contracts;
using SmartSpace.Api.Security;

namespace SmartSpace.Api.Features.Administration;

public static class AdminLocationEndpoints
{
    public static void MapAdminLocationEndpoints(this WebApplication app)
    {
        var admin = app.MapGroup("/api/admin/locations")
            .RequireAuthorization(AuthorizationPolicies.Administrator);

        admin.MapGet("", async (AdminLocationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListAsync(cancellationToken)));

        admin.MapPost("", async (
            [FromBody] LocationCreateRequest request,
            AdminLocationService service,
            CancellationToken cancellationToken) => await ExecuteAsync(() => service.CreateAsync(request, cancellationToken)));

        admin.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] LocationUpdateRequest request,
            AdminLocationService service,
            CancellationToken cancellationToken) => await ExecuteAsync(() => service.UpdateAsync(id, request, cancellationToken)));
    }

    private static async Task<IResult> ExecuteAsync(Func<Task<AdminLocationResult>> operation)
    {
        try
        {
            return Results.Ok(await operation());
        }
        catch (AdminLocationValidationException exception)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest,
                title: "Ongeldige locatie", detail: exception.Message,
                extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
        }
        catch (AdminLocationNotFoundException)
        {
            return Results.NotFound();
        }
        catch (AdminLocationStaleVersionException)
        {
            return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Locatie gewijzigd", detail: "Laad de locatie opnieuw.",
                extensions: new Dictionary<string, object?> { ["code"] = "stale_version" });
        }
        catch (AdminLocationConflictException exception)
        {
            return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Locatie bestaat al", detail: exception.Message,
                extensions: new Dictionary<string, object?> { ["code"] = "policy_conflict" });
        }
    }
}