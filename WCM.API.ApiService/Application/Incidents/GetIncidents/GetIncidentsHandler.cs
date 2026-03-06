using MediatR;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Incidents.GetIncidents;

public class GetIncidentsHandler(IIncidentRepository incidentRepository)
    : IRequestHandler<GetIncidentsQuery, Result<IncidentListResponse>>
{
    public async Task<Result<IncidentListResponse>> Handle(
        GetIncidentsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<int> countResult = await incidentRepository.GetTotalCountAsync(
            request.ContainerId, request.Type, request.Status,
            request.Priority, request.FromDate, request.ToDate,
            cancellationToken);

        if (countResult.IsFailure)
        {
            return Result.Failure<IncidentListResponse>(countResult.Error);
        }

        Result<IReadOnlyList<Incident>> result = await incidentRepository.GetAllAsync(
            request.ContainerId, request.Type, request.Status,
            request.Priority, request.FromDate, request.ToDate,
            request.Page, request.PageSize, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<IncidentListResponse>(result.Error);
        }

        IncidentListResponse response = new IncidentListResponse
        {
            Items = result.Value.Select(i => new IncidentDto
            {
                Id = i.Id,
                ContainerId = i.ContainerId,
                ContainerCode = i.Container?.Code,
                Type = i.Type,
                Description = i.Description,
                Status = i.Status,
                Priority = i.Priority,
                ReportedAt = i.ReportedAt,
                ResolvedAt = i.ResolvedAt,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            }).ToList(),
            TotalCount = countResult.Value,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(response);
    }
}
