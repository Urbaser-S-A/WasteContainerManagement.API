using MediatR;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypeById;

public record GetWasteTypeByIdQuery(Guid Id) : IRequest<Result<WasteTypeDto>>;
