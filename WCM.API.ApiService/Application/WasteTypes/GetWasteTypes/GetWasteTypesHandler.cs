using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;

public class GetWasteTypesHandler(IWasteTypeRepository wasteTypeRepository)
    : IRequestHandler<GetWasteTypesQuery, Result<IReadOnlyList<WasteTypeDto>>>
{
    public async Task<Result<IReadOnlyList<WasteTypeDto>>> Handle(
        GetWasteTypesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<IReadOnlyList<Domain.Entities.WasteType>> result =
            await wasteTypeRepository.GetAllAsync(request.IsActive, cancellationToken);

        return result.Match(
            onSuccess: wasteTypes =>
            {
                IReadOnlyList<WasteTypeDto> dtos = wasteTypes.Select(wt => new WasteTypeDto
                {
                    Id = wt.Id,
                    Name = wt.Name,
                    Description = wt.Description,
                    ColorCode = wt.ColorCode,
                    IsActive = wt.IsActive,
                    CreatedAt = wt.CreatedAt,
                    UpdatedAt = wt.UpdatedAt
                }).ToList();

                return Result.Success<IReadOnlyList<WasteTypeDto>>(dtos);
            },
            onFailure: error => Result.Failure<IReadOnlyList<WasteTypeDto>>(error));
    }
}
