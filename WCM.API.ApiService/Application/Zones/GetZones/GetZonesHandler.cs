using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.GetZones;

public class GetZonesHandler(IZoneRepository zoneRepository)
    : IRequestHandler<GetZonesQuery, Result<IReadOnlyList<ZoneDto>>>
{
    public async Task<Result<IReadOnlyList<ZoneDto>>> Handle(
        GetZonesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<IReadOnlyList<Domain.Entities.Zone>> result =
            await zoneRepository.GetAllAsync(request.District, request.City, request.IsActive, cancellationToken);

        return result.Match(
            onSuccess: zones =>
            {
                IReadOnlyList<ZoneDto> dtos = zones.Select(z => new ZoneDto
                {
                    Id = z.Id,
                    Name = z.Name,
                    District = z.District,
                    City = z.City,
                    IsActive = z.IsActive,
                    CreatedAt = z.CreatedAt,
                    UpdatedAt = z.UpdatedAt
                }).ToList();

                return Result.Success<IReadOnlyList<ZoneDto>>(dtos);
            },
            onFailure: error => Result.Failure<IReadOnlyList<ZoneDto>>(error));
    }
}
