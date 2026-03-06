namespace WCM.API.ApiService.Application.Zones.UpdateZone;

public class UpdateZoneRequest
{
    public required string Name { get; init; }
    public string? District { get; init; }
    public string? City { get; init; }
    public bool IsActive { get; init; }
}
