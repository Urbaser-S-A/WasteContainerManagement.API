using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.CreateWasteType;

public class CreateWasteTypeHandler(IWasteTypeRepository wasteTypeRepository)
    : IRequestHandler<CreateWasteTypeCommand, Result<WasteTypeDto>>
{
    public async Task<Result<WasteTypeDto>> Handle(
        CreateWasteTypeCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Check for duplicate name
        Result<bool> existsResult = await wasteTypeRepository.ExistsByNameAsync(
            request.Name, excludeId: null, cancellationToken);

        if (existsResult.IsFailure)
        {
            return Result.Failure<WasteTypeDto>(existsResult.Error);
        }

        if (existsResult.Value)
        {
            return Result.Failure<WasteTypeDto>(DomainErrors.WasteTypes.DuplicateName(request.Name));
        }

        // Map to entity
        WasteType wasteType = new WasteType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ColorCode = request.ColorCode,
            IsActive = request.IsActive
        };

        // Persist
        Result createResult = await wasteTypeRepository.CreateAsync(wasteType, cancellationToken);

        if (createResult.IsFailure)
        {
            return Result.Failure<WasteTypeDto>(createResult.Error);
        }

        // Map to DTO
        WasteTypeDto dto = new WasteTypeDto
        {
            Id = wasteType.Id,
            Name = wasteType.Name,
            Description = wasteType.Description,
            ColorCode = wasteType.ColorCode,
            IsActive = wasteType.IsActive,
            CreatedAt = wasteType.CreatedAt,
            UpdatedAt = wasteType.UpdatedAt
        };

        return Result.Success(dto);
    }
}
