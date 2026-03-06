namespace WCM.API.ApiService.Application.Zones.GetZones;

public class ZoneDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? District { get; init; }
    public string? City { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
