using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.GetContainers;

public class GetContainersHandler(IContainerRepository containerRepository)
    : IRequestHandler<GetContainersQuery, Result<ContainerListResponse>>
{
    public async Task<Result<ContainerListResponse>> Handle(
        GetContainersQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<int> countResult = await containerRepository.GetTotalCountAsync(
            request.ZoneId, request.WasteTypeId, request.Status, cancellationToken);

        if (countResult.IsFailure)
        {
            return Result.Failure<ContainerListResponse>(countResult.Error);
        }

        Result<IReadOnlyList<Container>> result = await containerRepository.GetAllAsync(
            request.ZoneId, request.WasteTypeId, request.Status,
            request.Page, request.PageSize, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<ContainerListResponse>(result.Error);
        }

        ContainerListResponse response = new ContainerListResponse
        {
            Items = result.Value.Select(c => new ContainerDto
            {
                Id = c.Id,
                Code = c.Code,
                WasteTypeId = c.WasteTypeId,
                WasteTypeName = c.WasteType?.Name,
                ZoneId = c.ZoneId,
                ZoneName = c.Zone?.Name,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                Address = c.Address,
                CapacityLiters = c.CapacityLiters,
                Status = c.Status,
                InstallationDate = c.InstallationDate,
                LastCollectionDate = c.LastCollectionDate,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList(),
            TotalCount = countResult.Value,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(response);
    }
}
