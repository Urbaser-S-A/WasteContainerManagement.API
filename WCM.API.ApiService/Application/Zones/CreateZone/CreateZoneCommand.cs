using MediatR;
using WCM.API.ApiService.Application.Zones.GetZones;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.CreateZone;

public record CreateZoneCommand(
    string Name,
    string? District,
    string? City,
    bool IsActive
) : IRequest<Result<ZoneDto>>;
