using System.Net;
using System.Net.Http.Json;

namespace SmartSpace.UI.Components.Reservations;

public sealed class MyReservationsClient(HttpClient http)
{
    public async Task<IReadOnlyList<MyReservation>> GetAsync(string view, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/reservations/mine?view={view}");
        AddDevelopmentIdentity(request);
        using var response = await http.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<MyReservation>>(cancellationToken) ?? [];
        }

        throw await CreateExceptionAsync(response, cancellationToken);
    }

    public async Task<MyReservation> UpdateAsync(Guid id, UpdateMyReservationRequest request, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/reservations/{id}")
        {
            Content = JsonContent.Create(request)
        };
        AddDevelopmentIdentity(message);
        using var response = await http.SendAsync(message, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MyReservation>(cancellationToken)
                ?? throw new InvalidOperationException("The updated reservation response was empty.");
        }

        throw await CreateExceptionAsync(response, cancellationToken);
    }

    public async Task CancelAsync(Guid id, Guid version, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/reservations/{id}/cancel")
        {
            Content = JsonContent.Create(new CancelMyReservationRequest(version))
        };
        AddDevelopmentIdentity(message);
        using var response = await http.SendAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateExceptionAsync(response, cancellationToken);
        }
    }

    private static async Task<MyReservationException> CreateExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(cancellationToken);
        return new MyReservationException(response.StatusCode, problem?.Detail ?? "De reservering kon niet worden verwerkt.");
    }

    private static void AddDevelopmentIdentity(HttpRequestMessage request)
    {
        request.Headers.Add("X-SmartSpace-Test-Subject", "demo-employee");
        request.Headers.Add("X-SmartSpace-Test-Role", "Employee");
    }
}

public sealed record MyReservation(
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

public sealed record UpdateMyReservationRequest(Guid ResourceId, DateTimeOffset Start, DateTimeOffset End, Guid Version);

public sealed record CancelMyReservationRequest(Guid Version);

public sealed class MyReservationException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}