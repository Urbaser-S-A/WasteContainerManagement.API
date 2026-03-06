using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.UpdateIncident;

public class UpdateIncidentHandler(IIncidentRepository incidentRepository)
    : IRequestHandler<UpdateIncidentCommand, Result>
{
    public async Task<Result> Handle(
        UpdateIncidentCommand request,
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

        Incident incident = getResult.Value;

        if (incident.Status is IncidentStatus.Resolved or IncidentStatus.Closed
            && request.Status is IncidentStatus.Open or IncidentStatus.InProgress)
        {
            return Result.Failure(DomainErrors.Incidents.AlreadyResolved(request.Id));
        }

        incident.Type = request.Type;
        incident.Description = request.Description;
        incident.Status = request.Status;
        incident.Priority = request.Priority;
        incident.ResolvedAt = request.ResolvedAt;

        if (request.Status is IncidentStatus.Resolved or IncidentStatus.Closed
            && incident.ResolvedAt is null)
        {
            incident.ResolvedAt = DateTime.UtcNow;
        }

        return await incidentRepository.UpdateAsync(incident, cancellationToken);
    }
}
