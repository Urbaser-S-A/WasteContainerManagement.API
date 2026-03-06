using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.UpdateContainer;

public class UpdateContainerHandler(IContainerRepository containerRepository)
    : IRequestHandler<UpdateContainerCommand, Result>
{
    public async Task<Result> Handle(
        UpdateContainerCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Container?> getResult = await containerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (getResult.IsFailure)
        {
            return Result.Failure(getResult.Error);
        }

        if (getResult.Value is null)
        {
            return Result.Failure(DomainErrors.Containers.NotFound(request.Id));
        }

        Result<bool> existsResult = await containerRepository.ExistsByCodeAsync(
            request.Code, excludeId: request.Id, cancellationToken);

        if (existsResult.IsFailure)
        {
            return Result.Failure(existsResult.Error);
        }

        if (existsResult.Value)
        {
            return Result.Failure(DomainErrors.Containers.DuplicateCode(request.Code));
        }

        Container container = getResult.Value;
        container.Code = request.Code;
        container.WasteTypeId = request.WasteTypeId;
        container.ZoneId = request.ZoneId;
        container.Latitude = request.Latitude;
        container.Longitude = request.Longitude;
        container.Address = request.Address;
        container.CapacityLiters = request.CapacityLiters;
        container.Status = request.Status;
        container.InstallationDate = request.InstallationDate;
        container.LastCollectionDate = request.LastCollectionDate;

        return await containerRepository.UpdateAsync(container, cancellationToken);
    }
}
