using MediatR;
using WCM.API.ApiService.Application.Incidents.GetIncidents;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.GetIncidentById;

public record GetIncidentByIdQuery(Guid Id) : IRequest<Result<IncidentDto>>;
