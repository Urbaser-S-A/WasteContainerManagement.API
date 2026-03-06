using MediatR;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.UpdateIncident;

public record UpdateIncidentCommand(
    Guid Id,
    IncidentType Type,
    string? Description,
    IncidentStatus Status,
    IncidentPriority Priority,
    DateTime? ResolvedAt
) : IRequest<Result>;
