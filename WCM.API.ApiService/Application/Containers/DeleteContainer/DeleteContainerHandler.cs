using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Containers.DeleteContainer;

public class DeleteContainerHandler(IContainerRepository containerRepository)
    : IRequestHandler<DeleteContainerCommand, Result>
{
    public async Task<Result> Handle(
        DeleteContainerCommand request,
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

        Result<bool> hasIncidentsResult = await containerRepository.HasOpenIncidentsAsync(request.Id, cancellationToken);

        if (hasIncidentsResult.IsFailure)
        {
            return Result.Failure(hasIncidentsResult.Error);
        }

        if (hasIncidentsResult.Value)
        {
            return Result.Failure(DomainErrors.Containers.HasOpenIncidents(request.Id));
        }

        return await containerRepository.DeleteAsync(getResult.Value, cancellationToken);
    }
}
