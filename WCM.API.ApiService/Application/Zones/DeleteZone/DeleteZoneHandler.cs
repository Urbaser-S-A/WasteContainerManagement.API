using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Zones.DeleteZone;

public class DeleteZoneHandler(IZoneRepository zoneRepository)
    : IRequestHandler<DeleteZoneCommand, Result>
{
    public async Task<Result> Handle(
        DeleteZoneCommand request,
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

        Result<bool> hasContainersResult = await zoneRepository.HasActiveContainersAsync(request.Id, cancellationToken);

        if (hasContainersResult.IsFailure)
        {
            return Result.Failure(hasContainersResult.Error);
        }

        if (hasContainersResult.Value)
        {
            return Result.Failure(DomainErrors.Zones.HasActiveContainers(request.Id));
        }

        return await zoneRepository.DeleteAsync(getResult.Value, cancellationToken);
    }
}
