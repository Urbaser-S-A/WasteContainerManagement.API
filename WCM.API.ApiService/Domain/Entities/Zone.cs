namespace WCM.API.ApiService.Domain.Entities;

public class Zone
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Container> Containers { get; set; } = [];
}
