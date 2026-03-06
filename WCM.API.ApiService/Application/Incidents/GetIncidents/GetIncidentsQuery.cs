using MediatR;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.GetIncidents;

public record GetIncidentsQuery(
    Guid? ContainerId,
    IncidentType? Type,
    IncidentStatus? Status,
    IncidentPriority? Priority,
    DateTime? FromDate,
    DateTime? ToDate,
    int Page,
    int PageSize
) : IRequest<Result<IncidentListResponse>>;
