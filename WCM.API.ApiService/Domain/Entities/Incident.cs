using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Domain.Entities;

public class Incident
{
    public Guid Id { get; set; }

    public Guid ContainerId { get; set; }

    public IncidentType Type { get; set; }

    public string? Description { get; set; }

    public IncidentStatus Status { get; set; } = IncidentStatus.Open;

    public IncidentPriority Priority { get; set; } = IncidentPriority.Medium;

    public DateTime ReportedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Container Container { get; set; } = null!;
}
