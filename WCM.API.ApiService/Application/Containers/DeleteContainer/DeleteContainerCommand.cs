using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.DeleteContainer;

public record DeleteContainerCommand(Guid Id) : IRequest<Result>;
