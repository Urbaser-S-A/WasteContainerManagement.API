using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypesV2;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypeByIdV2;

public class GetWasteTypeByIdV2Handler(IWasteTypeRepository wasteTypeRepository)
    : IRequestHandler<GetWasteTypeByIdV2Query, Result<WasteTypeV2Dto>>
{
    public async Task<Result<WasteTypeV2Dto>> Handle(
        GetWasteTypeByIdV2Query request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<WasteTypeWithContainerCount?> result =
            await wasteTypeRepository.GetByIdWithContainerCountAsync(request.Id, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<WasteTypeV2Dto>(result.Error);
        }

        if (result.Value is null)
        {
            return Result.Failure<WasteTypeV2Dto>(DomainErrors.WasteTypes.NotFound(request.Id));
        }

        WasteTypeWithContainerCount wasteType = result.Value;

        WasteTypeV2Dto dto = new()
        {
            Id = wasteType.Id,
            Name = wasteType.Name,
            Description = wasteType.Description,
            ColorCode = wasteType.ColorCode,
            IsActive = wasteType.IsActive,
            ActiveContainerCount = wasteType.ActiveContainerCount,
            CreatedAt = wasteType.CreatedAt,
            UpdatedAt = wasteType.UpdatedAt
        };

        return Result.Success(dto);
    }
}
