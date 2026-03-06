using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.DeleteZone;

public record DeleteZoneCommand(Guid Id) : IRequest<Result>;
