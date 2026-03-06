using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypeById;

public class GetWasteTypeByIdHandler(IWasteTypeRepository wasteTypeRepository)
    : IRequestHandler<GetWasteTypeByIdQuery, Result<WasteTypeDto>>
{
    public async Task<Result<WasteTypeDto>> Handle(
        GetWasteTypeByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<WasteType?> result = await wasteTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<WasteTypeDto>(result.Error);
        }

        if (result.Value is null)
        {
            return Result.Failure<WasteTypeDto>(DomainErrors.WasteTypes.NotFound(request.Id));
        }

        WasteType wasteType = result.Value;

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
