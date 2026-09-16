namespace SmartSpace.Api.Domain;

public sealed class Reservation
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public Resource? Resource { get; set; }
    public required string OwnerSubjectId { get; set; }
    public DateTimeOffset StartUtc { get; set; }
    public DateTimeOffset EndUtc { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public DateTimeOffset? CancelledAtUtc { get; set; }
    public Guid Version { get; set; } = Guid.NewGuid();
}
