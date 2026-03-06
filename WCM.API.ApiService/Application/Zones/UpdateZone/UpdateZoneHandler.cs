using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.UpdateZone;

public class UpdateZoneHandler(IZoneRepository zoneRepository)
    : IRequestHandler<UpdateZoneCommand, Result>
{
    public async Task<Result> Handle(
        UpdateZoneCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Zone?> getResult = await zoneRepository.GetByIdAsync(request.Id, cancellationToken);

        if (getResult.IsFailure)
        {
            return Result.Failure(getResult.Error);
        }

        if (getResult.Value is null)
        {
            return Result.Failure(DomainErrors.Zones.NotFound(request.Id));
        }

        Result<bool> existsResult = await zoneRepository.ExistsByNameAsync(
            request.Name, excludeId: request.Id, cancellationToken);

        if (existsResult.IsFailure)
        {
            return Result.Failure(existsResult.Error);
        }

        if (existsResult.Value)
        {
            return Result.Failure(DomainErrors.Zones.DuplicateName(request.Name));
        }

        Zone zone = getResult.Value;
        zone.Name = request.Name;
        zone.District = request.District;
        zone.City = request.City;
        zone.IsActive = request.IsActive;

        return await zoneRepository.UpdateAsync(zone, cancellationToken);
    }
}
