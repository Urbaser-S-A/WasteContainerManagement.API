using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.Zones.GetZones;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.GetZoneById;

public class GetZoneByIdHandler(IZoneRepository zoneRepository)
    : IRequestHandler<GetZoneByIdQuery, Result<ZoneDto>>
{
    public async Task<Result<ZoneDto>> Handle(
        GetZoneByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Zone?> result = await zoneRepository.GetByIdAsync(request.Id, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<ZoneDto>(result.Error);
        }

        if (result.Value is null)
        {
            return Result.Failure<ZoneDto>(DomainErrors.Zones.NotFound(request.Id));
        }

        Zone zone = result.Value;

        ZoneDto dto = new ZoneDto
        {
            Id = zone.Id,
            Name = zone.Name,
            District = zone.District,
            City = zone.City,
            IsActive = zone.IsActive,
            CreatedAt = zone.CreatedAt,
            UpdatedAt = zone.UpdatedAt
        };

        return Result.Success(dto);
    }
}
