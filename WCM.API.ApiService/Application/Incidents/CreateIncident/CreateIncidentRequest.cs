using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Application.Incidents.CreateIncident;

public class CreateIncidentRequest
{
    public Guid ContainerId { get; init; }
    public IncidentType Type { get; init; }
    public string? Description { get; init; }
    public IncidentPriority Priority { get; init; } = IncidentPriority.Medium;
}
