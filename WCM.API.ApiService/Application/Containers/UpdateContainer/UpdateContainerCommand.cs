using MediatR;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.UpdateContainer;

public record UpdateContainerCommand(
    Guid Id,
    string Code,
    Guid WasteTypeId,
    Guid ZoneId,
    double Latitude,
    double Longitude,
    string? Address,
    int CapacityLiters,
    ContainerStatus Status,
    DateTime? InstallationDate,
    DateTime? LastCollectionDate
) : IRequest<Result>;
