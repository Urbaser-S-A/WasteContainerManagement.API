using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.DeleteIncident;

public class DeleteIncidentHandler(IIncidentRepository incidentRepository)
    : IRequestHandler<DeleteIncidentCommand, Result>
{
    public async Task<Result> Handle(
        DeleteIncidentCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Incident?> getResult = await incidentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (getResult.IsFailure)
        {
            return Result.Failure(getResult.Error);
        }

        if (getResult.Value is null)
        {
            return Result.Failure(DomainErrors.Incidents.NotFound(request.Id));
        }

        return await incidentRepository.DeleteAsync(getResult.Value, cancellationToken);
    }
}
