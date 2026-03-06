using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.Zones.GetZones;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.CreateZone;

public class CreateZoneHandler(IZoneRepository zoneRepository)
    : IRequestHandler<CreateZoneCommand, Result<ZoneDto>>
{
    public async Task<Result<ZoneDto>> Handle(
        CreateZoneCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<bool> existsResult = await zoneRepository.ExistsByNameAsync(
            request.Name, excludeId: null, cancellationToken);

        if (existsResult.IsFailure)
        {
            return Result.Failure<ZoneDto>(existsResult.Error);
        }

        if (existsResult.Value)
        {
            return Result.Failure<ZoneDto>(DomainErrors.Zones.DuplicateName(request.Name));
        }

        Zone zone = new Zone
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            District = request.District,
            City = request.City,
            IsActive = request.IsActive
        };

        Result createResult = await zoneRepository.CreateAsync(zone, cancellationToken);

        if (createResult.IsFailure)
        {
            return Result.Failure<ZoneDto>(createResult.Error);
        }

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
