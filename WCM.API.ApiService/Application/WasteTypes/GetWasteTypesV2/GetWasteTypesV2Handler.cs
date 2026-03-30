using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypesV2;

public class GetWasteTypesV2Handler(IWasteTypeRepository wasteTypeRepository)
    : IRequestHandler<GetWasteTypesV2Query, Result<IReadOnlyList<WasteTypeV2Dto>>>
{
    public async Task<Result<IReadOnlyList<WasteTypeV2Dto>>> Handle(
        GetWasteTypesV2Query request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<IReadOnlyList<WasteTypeWithContainerCount>> result =
            await wasteTypeRepository.GetAllWithContainerCountAsync(request.IsActive, cancellationToken);

        return result.Match(
            onSuccess: wasteTypes =>
            {
                IReadOnlyList<WasteTypeV2Dto> dtos = wasteTypes.Select(wt => new WasteTypeV2Dto
                {
                    Id = wt.Id,
                    Name = wt.Name,
                    Description = wt.Description,
                    ColorCode = wt.ColorCode,
                    IsActive = wt.IsActive,
                    ActiveContainerCount = wt.ActiveContainerCount,
                    CreatedAt = wt.CreatedAt,
                    UpdatedAt = wt.UpdatedAt
                }).ToList();

                return Result.Success<IReadOnlyList<WasteTypeV2Dto>>(dtos);
            },
            onFailure: error => Result.Failure<IReadOnlyList<WasteTypeV2Dto>>(error));
    }
}
