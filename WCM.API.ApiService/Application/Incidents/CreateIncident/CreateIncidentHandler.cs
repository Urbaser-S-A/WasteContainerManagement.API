using MediatR;
using WCM.API.ApiService.Application.Incidents.GetIncidents;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.CreateIncident;

public class CreateIncidentHandler(
    IIncidentRepository incidentRepository,
    IContainerRepository containerRepository)
    : IRequestHandler<CreateIncidentCommand, Result<IncidentDto>>
{
    public async Task<Result<IncidentDto>> Handle(
        CreateIncidentCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Container?> containerResult = await containerRepository.GetByIdAsync(
            request.ContainerId, cancellationToken);

        if (containerResult.IsFailure)
        {
            return Result.Failure<IncidentDto>(containerResult.Error);
        }

        if (containerResult.Value is null)
        {
            return Result.Failure<IncidentDto>(DomainErrors.Containers.NotFound(request.ContainerId));
        }

        if (containerResult.Value.Status != ContainerStatus.Active)
        {
            return Result.Failure<IncidentDto>(DomainErrors.Incidents.ContainerNotActive(request.ContainerId));
        }

        Incident incident = new Incident
        {
            Id = Guid.NewGuid(),
            ContainerId = request.ContainerId,
            Type = request.Type,
            Description = request.Description,
            Status = IncidentStatus.Open,
            Priority = request.Priority
        };

        Result createResult = await incidentRepository.CreateAsync(incident, cancellationToken);

        if (createResult.IsFailure)
        {
            return Result.Failure<IncidentDto>(createResult.Error);
        }

        IncidentDto dto = new IncidentDto
        {
            Id = incident.Id,
            ContainerId = incident.ContainerId,
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
