using System.Net;
using System.Net.Http.Json;

namespace SmartSpace.UI.Components.Reservations;

public sealed class BookingClient(HttpClient http)
{
    public async Task<BookingResult> CreateAsync(
        Guid resourceId,
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/reservations")
        {
            Content = JsonContent.Create(new CreateBookingRequest(resourceId, start, end))
        };
        request.Headers.Add("X-SmartSpace-Test-Subject", "demo-employee");
        request.Headers.Add("X-SmartSpace-Test-Role", "Employee");

        using var response = await http.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<BookingResult>(cancellationToken);
            return result ?? throw new InvalidOperationException("The reservation response was empty.");
        }

        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(cancellationToken);
        throw new BookingException(response.StatusCode, problem?.Detail ?? "De reservering kon niet worden opgeslagen.");
    }
}

public sealed record CreateBookingRequest(Guid ResourceId, DateTimeOffset Start, DateTimeOffset End);

public sealed record BookingResult(
    Guid Id,
    Guid ResourceId,
    string ResourceName,
    Guid LocationId,
    DateTimeOffset Start,
    DateTimeOffset End,
    ReservationStatus Status,
    Guid Version);

public enum ReservationStatus
{
    Active = 1,
    Cancelled = 2
}

public sealed record ProblemResponse(string? Detail, string? Code);

public sealed class BookingException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
