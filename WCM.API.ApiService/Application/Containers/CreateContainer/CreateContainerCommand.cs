using MediatR;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.CreateContainer;

public record CreateContainerCommand(
    string Code,
    Guid WasteTypeId,
    Guid ZoneId,
    double Latitude,
    double Longitude,
    string? Address,
    int CapacityLiters,
    ContainerStatus Status,
    DateTime? InstallationDate
) : IRequest<Result<ContainerDto>>;
