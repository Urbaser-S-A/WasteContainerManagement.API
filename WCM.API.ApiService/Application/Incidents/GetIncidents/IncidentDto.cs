using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Application.Incidents.GetIncidents;

public class IncidentDto
{
    public Guid Id { get; init; }
    public Guid ContainerId { get; init; }
    public string? ContainerCode { get; init; }
    public IncidentType Type { get; init; }
    public string? Description { get; init; }
    public IncidentStatus Status { get; init; }
    public IncidentPriority Priority { get; init; }
    public DateTime ReportedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
