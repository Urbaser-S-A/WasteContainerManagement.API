using MediatR;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.CreateWasteType;

public record CreateWasteTypeCommand(
    string Name,
    string? Description,
    string? ColorCode,
    bool IsActive
) : IRequest<Result<WasteTypeDto>>;
