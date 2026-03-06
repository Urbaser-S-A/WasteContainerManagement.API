using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Interfaces;

public interface IIncidentRepository
{
    Task<Result<IReadOnlyList<Incident>>> GetAllAsync(
        Guid? containerId,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Result<int>> GetTotalCountAsync(
        Guid? containerId,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken);

    Task<Result<Incident?>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result> CreateAsync(Incident incident, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Incident incident, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Incident incident, CancellationToken cancellationToken);
}
