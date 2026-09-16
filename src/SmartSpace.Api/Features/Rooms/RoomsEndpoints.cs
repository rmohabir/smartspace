using Microsoft.AspNetCore.Mvc;
using SmartSpace.Api.Features.Rooms;

namespace SmartSpace.Api;

public static class RoomsEndpoints
{
    public static void MapRoomsEndpoints(this WebApplication app)
    {
        app.MapGet("/rooms/availability", async (
            [FromQuery] Guid locationId,
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end,
            [FromQuery] int minimumCapacity,
            RoomAvailabilityService service) =>
        {
            if (end <= start)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["end"] = ["End must be after Start."]
                });
            }

            if (minimumCapacity < 1)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["minimumCapacity"] = ["Minimum capacity must be at least 1."]
                });
            }

            var request = new RoomAvailabilityRequest(locationId, start, end, minimumCapacity);
            var result = await service.GetAvailableRoomsAsync(request);
            return Results.Ok(result);
        });
    }
}
