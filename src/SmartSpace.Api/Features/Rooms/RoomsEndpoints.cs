using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Data;
using SmartSpace.Api.Domain;
using SmartSpace.Api.Features.Rooms;

namespace SmartSpace.Api;

public static class RoomsEndpoints
{
    public static void MapRoomsEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        api.MapGet("/locations", async (SmartSpaceDbContext db, CancellationToken cancellationToken) =>
            Results.Ok(await db.Locations
                .AsNoTracking()
                .Where(location => location.IsActive)
                .OrderBy(location => location.Name)
                .Select(location => new LocationResult(location.Id, location.Name, location.Building, location.Floor))
                .ToListAsync(cancellationToken)));

        api.MapGet("/rooms", async (
            [FromQuery(Name = "locationId")] Guid? locationId,
            [FromQuery(Name = "minCapacity")] int? minimumCapacity,
            SmartSpaceDbContext db,
            CancellationToken cancellationToken) =>
        {
            if (minimumCapacity is < 1)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Ongeldige ruimtefilter",
                    detail: "De minimumcapaciteit moet minimaal 1 zijn.",
                    extensions: new Dictionary<string, object?> { ["code"] = "validation_error" });
            }

            var rooms = await db.Resources
                .AsNoTracking()
                .Where(resource => resource.IsActive && resource.ResourceType == ResourceType.MeetingRoom)
                .Where(resource => resource.Location != null && resource.Location.IsActive)
                .Where(resource => locationId == null || resource.LocationId == locationId)
                .Where(resource => minimumCapacity == null || resource.Capacity >= minimumCapacity)
                .OrderBy(resource => resource.Name)
                .Select(resource => new RoomResult(
                    resource.Id,
                    resource.Name,
                    resource.Capacity,
                    resource.LocationId,
                    resource.Location!.Name,
                    resource.IsActive,
                    resource.Version))
                .ToListAsync(cancellationToken);

            return Results.Ok(rooms);
        });

        api.MapGet("/rooms/availability", async (
            [FromQuery] Guid? locationId,
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end,
            [FromQuery(Name = "minCapacity")] int? minimumCapacity,
            RoomAvailabilityService service,
            CancellationToken cancellationToken) =>
        {
            if (end <= start)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Ongeldig tijdvak",
                    detail: "De eindtijd moet na de starttijd liggen.", extensions: new Dictionary<string, object?>
                    { ["code"] = "validation_error" });
            }

            if (minimumCapacity is < 1)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Ongeldige capaciteit",
                    detail: "De minimumcapaciteit moet minimaal 1 zijn.", extensions: new Dictionary<string, object?>
                    { ["code"] = "validation_error" });
            }

            var request = new RoomAvailabilityRequest(locationId, start, end, minimumCapacity ?? 1);
            var result = await service.GetAvailableRoomsAsync(request);
            return Results.Ok(result);
        });
    }

    private sealed record LocationResult(Guid Id, string Name, string? Building, string? Floor);

    private sealed record RoomResult(
        Guid Id,
        string Name,
        int Capacity,
        Guid LocationId,
        string LocationName,
        bool IsActive,
        Guid Version);
}
