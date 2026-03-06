namespace WCM.API.ApiService.Application.Zones.CreateZone;

public class CreateZoneRequest
{
    public required string Name { get; init; }
    public string? District { get; init; }
    public string? City { get; init; }
    public bool IsActive { get; init; } = true;
}
