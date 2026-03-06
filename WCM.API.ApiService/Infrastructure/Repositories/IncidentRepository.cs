using Microsoft.EntityFrameworkCore;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Persistence;

namespace WCM.API.ApiService.Infrastructure.Repositories;

public class IncidentRepository(IApplicationDbContext context, ILogger<IncidentRepository> logger)
    : BaseRepository(context, logger), IIncidentRepository
{
    public async Task<Result<IReadOnlyList<Incident>>> GetAllAsync(
        Guid? containerId,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<Incident> query = Context.Incidents
                    .AsNoTracking()
                    .Include(i => i.Container);

                query = ApplyFilters(query, containerId, type, status, priority, fromDate, toDate);

                return (IReadOnlyList<Incident>)await query
                    .OrderByDescending(i => i.ReportedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
            },
            "retrieving all incidents");
    }

    public async Task<Result<int>> GetTotalCountAsync(
        Guid? containerId,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<Incident> query = Context.Incidents.AsNoTracking();

                query = ApplyFilters(query, containerId, type, status, priority, fromDate, toDate);

                return await query.CountAsync(cancellationToken);
            },
            "counting incidents");
    }

    public async Task<Result<Incident?>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () => await Context.Incidents
                .AsNoTracking()
                .Include(i => i.Container)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken),
            "retrieving incident by ID",
            logParameters: [id]);
    }

    public async Task<Result> CreateAsync(Incident incident, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Incidents.Add(incident);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "creating incident",
            logParameters: [incident.Id, incident.ContainerId]);
    }

    public async Task<Result> UpdateAsync(Incident incident, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Incidents.Update(incident);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "updating incident",
            logParameters: [incident.Id]);
    }

    public async Task<Result> DeleteAsync(Incident incident, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Incidents.Remove(incident);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "deleting incident",
            logParameters: [incident.Id]);
    }

    private static IQueryable<Incident> ApplyFilters(
        IQueryable<Incident> query,
        Guid? containerId,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (containerId.HasValue)
        {
            query = query.Where(i => i.ContainerId == containerId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(i => i.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(i => i.Priority == priority.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(i => i.ReportedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(i => i.ReportedAt <= toDate.Value);
        }

        return query;
    }
}
