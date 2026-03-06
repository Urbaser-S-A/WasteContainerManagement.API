using MediatR;
using WCM.API.ApiService.Application.Zones.GetZones;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.GetZoneById;

public record GetZoneByIdQuery(Guid Id) : IRequest<Result<ZoneDto>>;
