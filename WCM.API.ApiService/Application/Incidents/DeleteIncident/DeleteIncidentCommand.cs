using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.DeleteIncident;

public record DeleteIncidentCommand(Guid Id) : IRequest<Result>;
