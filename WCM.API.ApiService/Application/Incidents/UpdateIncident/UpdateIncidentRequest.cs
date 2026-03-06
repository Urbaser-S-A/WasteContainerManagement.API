using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Application.Incidents.UpdateIncident;

public class UpdateIncidentRequest
{
    public IncidentType Type { get; init; }
    public string? Description { get; init; }
    public IncidentStatus Status { get; init; }
    public IncidentPriority Priority { get; init; }
    public DateTime? ResolvedAt { get; init; }
}
