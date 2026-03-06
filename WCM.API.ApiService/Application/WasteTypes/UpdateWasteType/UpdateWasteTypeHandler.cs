using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;

public class UpdateWasteTypeHandler(IWasteTypeRepository wasteTypeRepository)
    : IRequestHandler<UpdateWasteTypeCommand, Result>
{
    public async Task<Result> Handle(
        UpdateWasteTypeCommand request,
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

        // Check for duplicate name (excluding self)
        Result<bool> existsResult = await wasteTypeRepository.ExistsByNameAsync(
            request.Name, excludeId: request.Id, cancellationToken);

        if (existsResult.IsFailure)
        {
            return Result.Failure(existsResult.Error);
        }

        if (existsResult.Value)
        {
            return Result.Failure(DomainErrors.WasteTypes.DuplicateName(request.Name));
        }

        // Update entity
        WasteType wasteType = getResult.Value;
        wasteType.Name = request.Name;
        wasteType.Description = request.Description;
        wasteType.ColorCode = request.ColorCode;
        wasteType.IsActive = request.IsActive;

        return await wasteTypeRepository.UpdateAsync(wasteType, cancellationToken);
    }
}
