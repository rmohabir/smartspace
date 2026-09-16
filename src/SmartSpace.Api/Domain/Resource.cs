namespace SmartSpace.Api.Domain;

public sealed class Resource
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ResourceType ResourceType { get; set; } = ResourceType.MeetingRoom;
    public int Capacity { get; set; }
    public Guid LocationId { get; set; }
    public Location? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid Version { get; set; } = Guid.NewGuid();

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
