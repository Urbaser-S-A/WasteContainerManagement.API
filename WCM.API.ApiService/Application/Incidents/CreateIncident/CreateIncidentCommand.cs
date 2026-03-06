using MediatR;
using WCM.API.ApiService.Application.Incidents.GetIncidents;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.CreateIncident;

public record CreateIncidentCommand(
    Guid ContainerId,
    IncidentType Type,
    string? Description,
    IncidentPriority Priority
) : IRequest<Result<IncidentDto>>;
