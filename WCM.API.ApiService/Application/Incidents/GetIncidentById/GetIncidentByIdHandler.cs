using MediatR;
using WCM.API.ApiService.Application.Incidents.GetIncidents;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.GetIncidentById;

public class GetIncidentByIdHandler(IIncidentRepository incidentRepository)
    : IRequestHandler<GetIncidentByIdQuery, Result<IncidentDto>>
{
    public async Task<Result<IncidentDto>> Handle(
        GetIncidentByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Incident?> result = await incidentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<IncidentDto>(result.Error);
        }

        if (result.Value is null)
        {
            return Result.Failure<IncidentDto>(DomainErrors.Incidents.NotFound(request.Id));
        }

        Incident incident = result.Value;

        IncidentDto dto = new IncidentDto
        {
            Id = incident.Id,
            ContainerId = incident.ContainerId,
            ContainerCode = incident.Container?.Code,
            Type = incident.Type,
            Description = incident.Description,
            Status = incident.Status,
            Priority = incident.Priority,
            ReportedAt = incident.ReportedAt,
            ResolvedAt = incident.ResolvedAt,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt
        };

        return Result.Success(dto);
    }
}
