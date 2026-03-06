using MediatR;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.GetContainerById;

public record GetContainerByIdQuery(Guid Id) : IRequest<Result<ContainerDto>>;
