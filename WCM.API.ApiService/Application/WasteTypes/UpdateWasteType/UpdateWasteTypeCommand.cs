using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;

public record UpdateWasteTypeCommand(
    Guid Id,
    string Name,
    string? Description,
    string? ColorCode,
    bool IsActive
) : IRequest<Result>;
