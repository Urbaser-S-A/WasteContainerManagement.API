using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;

public record GetWasteTypesQuery(bool? IsActive) : IRequest<Result<IReadOnlyList<WasteTypeDto>>>;
