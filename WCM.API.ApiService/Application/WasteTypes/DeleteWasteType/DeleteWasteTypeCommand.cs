using MediatR;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.WasteTypes.DeleteWasteType;

public record DeleteWasteTypeCommand(Guid Id) : IRequest<Result>;
