using MediatR;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.CreateContainer;

public class CreateContainerHandler(IContainerRepository containerRepository)
    : IRequestHandler<CreateContainerCommand, Result<ContainerDto>>
{
    public async Task<Result<ContainerDto>> Handle(
        CreateContainerCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<bool> existsResult = await containerRepository.ExistsByCodeAsync(
            request.Code, excludeId: null, cancellationToken);

        if (existsResult.IsFailure)
        {
            return Result.Failure<ContainerDto>(existsResult.Error);
        }

        if (existsResult.Value)
        {
            return Result.Failure<ContainerDto>(DomainErrors.Containers.DuplicateCode(request.Code));
        }

        Container container = new Container
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            WasteTypeId = request.WasteTypeId,
            ZoneId = request.ZoneId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            CapacityLiters = request.CapacityLiters,
            Status = request.Status,
            InstallationDate = request.InstallationDate
        };

        Result createResult = await containerRepository.CreateAsync(container, cancellationToken);

        if (createResult.IsFailure)
        {
            return Result.Failure<ContainerDto>(createResult.Error);
        }

        ContainerDto dto = new ContainerDto
        {
            Id = container.Id,
            Code = container.Code,
            WasteTypeId = container.WasteTypeId,
            ZoneId = container.ZoneId,
            Latitude = container.Latitude,
            Longitude = container.Longitude,
            Address = container.Address,
            CapacityLiters = container.CapacityLiters,
            Status = container.Status,
            InstallationDate = container.InstallationDate,
            LastCollectionDate = container.LastCollectionDate,
            CreatedAt = container.CreatedAt,
            UpdatedAt = container.UpdatedAt
        };

        return Result.Success(dto);
    }
}
