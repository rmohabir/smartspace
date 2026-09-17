using System.Net;
using System.Net.Http.Json;

namespace SmartSpace.UI.Components.Admin;

public sealed class AdminRoomClient(HttpClient http)
{
    public async Task<IReadOnlyList<AdminRoom>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(HttpMethod.Get, "api/admin/rooms", null, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<AdminRoom>>(cancellationToken) ?? [];
    }

    public Task<AdminRoom> CreateAsync(AdminRoomWriteRequest request, CancellationToken cancellationToken = default) =>
        SendRoomAsync(HttpMethod.Post, "api/admin/rooms", request, cancellationToken);

    public Task<AdminRoom> UpdateAsync(Guid id, AdminRoomWriteRequest request, CancellationToken cancellationToken = default) =>
        SendRoomAsync(HttpMethod.Put, $"api/admin/rooms/{id}", request, cancellationToken);

    public Task<AdminRoom> SetActiveAsync(Guid id, Guid version, bool active, CancellationToken cancellationToken = default) =>
        SendRoomAsync(HttpMethod.Post, $"api/admin/rooms/{id}/{(active ? "reactivate" : "deactivate")}",
            new AdminRoomVersionRequest(version), cancellationToken);

    private async Task<AdminRoom> SendRoomAsync(HttpMethod method, string uri, object body, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(method, uri, body, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<AdminRoom>(cancellationToken)
            ?? throw new InvalidOperationException("The room response was empty.");
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, object? body, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(method, uri);
        if (body is not null) request.Content = JsonContent.Create(body);
        request.Headers.Add("X-SmartSpace-Test-Subject", "demo-administrator");
        request.Headers.Add("X-SmartSpace-Test-Role", "Administrator");
        return await http.SendAsync(request, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        var problem = await response.Content.ReadFromJsonAsync<AdminProblemResponse>(cancellationToken);
        throw new AdminRoomException(response.StatusCode, problem?.Detail ?? "De ruimte kon niet worden verwerkt.");
    }
}

public sealed record AdminRoom(Guid Id, string Name, int Capacity, Guid LocationId, string LocationName, bool IsActive, Guid Version);
public sealed record AdminRoomWriteRequest(string Name, int Capacity, Guid LocationId, Guid? Version = null);
public sealed record AdminRoomVersionRequest(Guid Version);
public sealed record AdminProblemResponse(string? Detail, string? Code);

public sealed class AdminRoomException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}