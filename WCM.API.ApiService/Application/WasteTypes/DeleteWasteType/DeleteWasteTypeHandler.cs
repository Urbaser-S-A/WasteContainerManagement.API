using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.DeleteWasteType;

public class DeleteWasteTypeHandler(IWasteTypeRepository wasteTypeRepository)
    : IRequestHandler<DeleteWasteTypeCommand, Result>
{
    public async Task<Result> Handle(
        DeleteWasteTypeCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Verify existence
        Result<WasteType?> getResult = await wasteTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (getResult.IsFailure)
        {
            return Result.Failure(getResult.Error);
        }

        if (getResult.Value is null)
        {
            return Result.Failure(DomainErrors.WasteTypes.NotFound(request.Id));
        }

        // Check for active containers
        Result<bool> hasContainersResult = await wasteTypeRepository.HasActiveContainersAsync(request.Id, cancellationToken);

        if (hasContainersResult.IsFailure)
        {
            return Result.Failure(hasContainersResult.Error);
        }

        if (hasContainersResult.Value)
        {
            return Result.Failure(DomainErrors.WasteTypes.HasActiveContainers(request.Id));
        }

        return await wasteTypeRepository.DeleteAsync(getResult.Value, cancellationToken);
    }
}
