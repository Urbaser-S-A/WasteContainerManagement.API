using Microsoft.EntityFrameworkCore;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Persistence;

namespace WCM.API.ApiService.Infrastructure.Repositories;

public class ContainerRepository(IApplicationDbContext context, ILogger<ContainerRepository> logger)
    : BaseRepository(context, logger), IContainerRepository
{
    public async Task<Result<IReadOnlyList<Container>>> GetAllAsync(
        Guid? zoneId,
        Guid? wasteTypeId,
        ContainerStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<Container> query = Context.Containers
                    .AsNoTracking()
                    .Include(c => c.WasteType)
                    .Include(c => c.Zone);

                if (zoneId.HasValue)
                {
                    query = query.Where(c => c.ZoneId == zoneId.Value);
                }

                if (wasteTypeId.HasValue)
                {
                    query = query.Where(c => c.WasteTypeId == wasteTypeId.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(c => c.Status == status.Value);
                }

                return (IReadOnlyList<Container>)await query
                    .OrderBy(c => c.Code)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
            },
            "retrieving all containers");
    }

    public async Task<Result<int>> GetTotalCountAsync(
        Guid? zoneId,
        Guid? wasteTypeId,
        ContainerStatus? status,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<Container> query = Context.Containers.AsNoTracking();

                if (zoneId.HasValue)
                {
                    query = query.Where(c => c.ZoneId == zoneId.Value);
                }

                if (wasteTypeId.HasValue)
                {
                    query = query.Where(c => c.WasteTypeId == wasteTypeId.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(c => c.Status == status.Value);
                }

                return await query.CountAsync(cancellationToken);
            },
            "counting containers");
    }

    public async Task<Result<Container?>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () => await Context.Containers
                .AsNoTracking()
                .Include(c => c.WasteType)
                .Include(c => c.Zone)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken),
            "retrieving container by ID",
            logParameters: [id]);
    }

    public async Task<Result<bool>> ExistsByCodeAsync(
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<Container> query = Context.Containers
                    .AsNoTracking()
                    .Where(c => c.Code == code);

                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeId.Value);
                }

                return await query.AnyAsync(cancellationToken);
            },
            "checking container code existence",
            logParameters: [code]);
    }

    public async Task<Result<bool>> HasOpenIncidentsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () => await Context.Incidents
                .AsNoTracking()
                .AnyAsync(i => i.ContainerId == id &&
                    (i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress),
                    cancellationToken),
            "checking open incidents for container",
            logParameters: [id]);
    }

    public async Task<Result> CreateAsync(Container container, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Containers.Add(container);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "creating container",
            logParameters: [container.Id, container.Code]);
    }

    public async Task<Result> UpdateAsync(Container container, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Containers.Update(container);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "updating container",
            logParameters: [container.Id, container.Code]);
    }

    public async Task<Result> DeleteAsync(Container container, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Containers.Remove(container);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "deleting container",
            logParameters: [container.Id]);
    }
}
