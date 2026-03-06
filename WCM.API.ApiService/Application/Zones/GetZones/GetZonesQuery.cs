using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.GetZones;

public record GetZonesQuery(
    string? District,
    string? City,
    bool? IsActive
) : IRequest<Result<IReadOnlyList<ZoneDto>>>;
