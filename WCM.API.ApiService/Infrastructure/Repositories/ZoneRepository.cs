using Microsoft.EntityFrameworkCore;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Persistence;

namespace WCM.API.ApiService.Infrastructure.Repositories;

public class ZoneRepository(IApplicationDbContext context, ILogger<ZoneRepository> logger)
    : BaseRepository(context, logger), IZoneRepository
{
    public async Task<Result<IReadOnlyList<Zone>>> GetAllAsync(
        string? district,
        string? city,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<Zone> query = Context.Zones.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(district))
                {
                    query = query.Where(z => z.District != null && z.District.Contains(district));
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    query = query.Where(z => z.City != null && z.City.Contains(city));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(z => z.IsActive == isActive.Value);
                }

                return (IReadOnlyList<Zone>)await query
                    .OrderBy(z => z.Name)
                    .ToListAsync(cancellationToken);
            },
            "retrieving all zones");
    }

    public async Task<Result<Zone?>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () => await Context.Zones
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == id, cancellationToken),
            "retrieving zone by ID",
            logParameters: [id]);
    }

    public async Task<Result<bool>> ExistsByNameAsync(
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<Zone> query = Context.Zones
                    .AsNoTracking()
                    .Where(z => z.Name == name);

                if (excludeId.HasValue)
                {
                    query = query.Where(z => z.Id != excludeId.Value);
                }

                return await query.AnyAsync(cancellationToken);
            },
            "checking zone name existence",
            logParameters: [name]);
    }

    public async Task<Result<bool>> HasActiveContainersAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () => await Context.Containers
                .AsNoTracking()
                .AnyAsync(c => c.ZoneId == id, cancellationToken),
            "checking active containers for zone",
            logParameters: [id]);
    }

    public async Task<Result> CreateAsync(Zone zone, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Zones.Add(zone);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "creating zone",
            logParameters: [zone.Id, zone.Name]);
    }

    public async Task<Result> UpdateAsync(Zone zone, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Zones.Update(zone);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "updating zone",
            logParameters: [zone.Id, zone.Name]);
    }

    public async Task<Result> DeleteAsync(Zone zone, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.Zones.Remove(zone);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "deleting zone",
            logParameters: [zone.Id]);
    }
}
