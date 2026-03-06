using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Domain.Entities;

public class Container
{
    public Guid Id { get; set; }

    public required string Code { get; set; }

    public Guid WasteTypeId { get; set; }

    public Guid ZoneId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Address { get; set; }

    public int CapacityLiters { get; set; }

    public ContainerStatus Status { get; set; } = ContainerStatus.Active;

    public DateTime? InstallationDate { get; set; }

    public DateTime? LastCollectionDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public WasteType WasteType { get; set; } = null!;

    public Zone Zone { get; set; } = null!;

    public ICollection<Incident> Incidents { get; set; } = [];
}
