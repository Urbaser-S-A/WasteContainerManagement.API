using MediatR;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypesV2;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypeByIdV2;

public record GetWasteTypeByIdV2Query(Guid Id) : IRequest<Result<WasteTypeV2Dto>>;
