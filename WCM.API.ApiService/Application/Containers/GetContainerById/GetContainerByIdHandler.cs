using MediatR;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.GetContainerById;

public class GetContainerByIdHandler(IContainerRepository containerRepository)
    : IRequestHandler<GetContainerByIdQuery, Result<ContainerDto>>
{
    public async Task<Result<ContainerDto>> Handle(
        GetContainerByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Container?> result = await containerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<ContainerDto>(result.Error);
        }

        if (result.Value is null)
        {
            return Result.Failure<ContainerDto>(DomainErrors.Containers.NotFound(request.Id));
        }

        Container container = result.Value;

        ContainerDto dto = new ContainerDto
        {
            Id = container.Id,
            Code = container.Code,
            WasteTypeId = container.WasteTypeId,
            WasteTypeName = container.WasteType?.Name,
            ZoneId = container.ZoneId,
            ZoneName = container.Zone?.Name,
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
