namespace SmartSpace.Api.Contracts;

public sealed record CreateReservationRequest(Guid ResourceId, DateTimeOffset Start, DateTimeOffset End);

public sealed record UpdateReservationRequest(
    Guid ResourceId,
    DateTimeOffset Start,
    DateTimeOffset End,
    Guid Version);

public sealed record CancelReservationRequest(Guid Version);

public sealed record RoomCreateRequest(string Name, int Capacity, Guid LocationId);

public sealed record RoomUpdateRequest(string Name, int Capacity, Guid LocationId, Guid Version);

public sealed record VersionRequest(Guid Version);
