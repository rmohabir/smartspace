using System.Net.Http.Json;

namespace SmartSpace.UI.Components.RoomSearch;

public sealed class RoomSearchClient(HttpClient http)
{
    public async Task<IReadOnlyList<LocationOption>> GetLocationsAsync(CancellationToken cancellationToken = default) =>
        await http.GetFromJsonAsync<List<LocationOption>>("api/locations", cancellationToken) ?? [];

    public async Task<IReadOnlyList<RoomResult>> SearchAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        Guid? locationId,
        int minimumCapacity,
        CancellationToken cancellationToken = default)
    {
        var query = $"api/rooms/availability?start={Uri.EscapeDataString(start.ToString("O"))}" +
            $"&end={Uri.EscapeDataString(end.ToString("O"))}&minCapacity={minimumCapacity}";

        if (locationId.HasValue)
        {
            query += $"&locationId={locationId.Value}";
        }

        return await http.GetFromJsonAsync<List<RoomResult>>(query, cancellationToken) ?? [];
    }
}

public sealed record LocationOption(Guid Id, string Name, string? Building, string? Floor);

public sealed record RoomResult(Guid Id, string Name, Guid LocationId, int Capacity);
