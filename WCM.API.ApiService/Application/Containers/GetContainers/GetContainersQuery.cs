using MediatR;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.GetContainers;

public record GetContainersQuery(
    Guid? ZoneId,
    Guid? WasteTypeId,
    ContainerStatus? Status,
    int Page,
    int PageSize
) : IRequest<Result<ContainerListResponse>>;
