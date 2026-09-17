namespace SmartSpace.Api.Domain;

public sealed class Location
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid Version { get; set; } = Guid.NewGuid();

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
