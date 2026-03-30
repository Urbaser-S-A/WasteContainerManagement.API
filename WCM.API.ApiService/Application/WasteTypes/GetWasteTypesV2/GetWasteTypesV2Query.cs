using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypesV2;

public record GetWasteTypesV2Query(bool? IsActive) : IRequest<Result<IReadOnlyList<WasteTypeV2Dto>>>;
