using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.UpdateZone;

public record UpdateZoneCommand(
    Guid Id,
    string Name,
    string? District,
    string? City,
    bool IsActive
) : IRequest<Result>;
