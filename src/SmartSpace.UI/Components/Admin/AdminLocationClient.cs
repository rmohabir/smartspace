using System.Net;
using System.Net.Http.Json;

namespace SmartSpace.UI.Components.Admin;

public sealed class AdminLocationClient(HttpClient http)
{
    public async Task<IReadOnlyList<AdminLocation>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(HttpMethod.Get, "api/admin/locations", null, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<AdminLocation>>(cancellationToken) ?? [];
    }

    public Task<AdminLocation> CreateAsync(AdminLocationWriteRequest request, CancellationToken cancellationToken = default) =>
        SendLocationAsync(HttpMethod.Post, "api/admin/locations", request, cancellationToken);

    public Task<AdminLocation> UpdateAsync(Guid id, AdminLocationWriteRequest request, CancellationToken cancellationToken = default) =>
        SendLocationAsync(HttpMethod.Put, $"api/admin/locations/{id}", request, cancellationToken);

    private async Task<AdminLocation> SendLocationAsync(HttpMethod method, string uri, object body, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(method, uri, body, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<AdminLocation>(cancellationToken)
            ?? throw new InvalidOperationException("The location response was empty.");
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
        throw new AdminLocationException(response.StatusCode, problem?.Detail ?? "De locatie kon niet worden verwerkt.");
    }
}

public sealed record AdminLocation(Guid Id, string Name, string? Building, string? Floor, bool IsActive, Guid Version);
public sealed record AdminLocationWriteRequest(string Name, string? Building, string? Floor, Guid? Version = null);

public sealed class AdminLocationException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}