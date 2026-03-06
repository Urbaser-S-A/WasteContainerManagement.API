using Microsoft.EntityFrameworkCore;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Persistence;

namespace WCM.API.ApiService.Infrastructure.Repositories;

public class WasteTypeRepository(IApplicationDbContext context, ILogger<WasteTypeRepository> logger)
    : BaseRepository(context, logger), IWasteTypeRepository
{
    public async Task<Result<IReadOnlyList<WasteType>>> GetAllAsync(
        bool? isActive,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () =>
            {
                IQueryable<WasteType> query = Context.WasteTypes.AsNoTracking();

                if (isActive.HasValue)
                {
                    query = query.Where(wt => wt.IsActive == isActive.Value);
                }

                return (IReadOnlyList<WasteType>)await query
                    .OrderBy(wt => wt.Name)
                    .ToListAsync(cancellationToken);
            },
            "retrieving all waste types",
            logParameters: isActive.HasValue ? [isActive.Value] : []);
    }

    public async Task<Result<WasteType?>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () => await Context.WasteTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(wt => wt.Id == id, cancellationToken),
            "retrieving waste type by ID",
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
                IQueryable<WasteType> query = Context.WasteTypes
                    .AsNoTracking()
                    .Where(wt => wt.Name == name);

                if (excludeId.HasValue)
                {
                    query = query.Where(wt => wt.Id != excludeId.Value);
                }

                return await query.AnyAsync(cancellationToken);
            },
            "checking waste type name existence",
            logParameters: [name]);
    }

    public async Task<Result<bool>> HasActiveContainersAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(
            async () => await Context.Containers
                .AsNoTracking()
                .AnyAsync(c => c.WasteTypeId == id, cancellationToken),
            "checking active containers for waste type",
            logParameters: [id]);
    }

    public async Task<Result> CreateAsync(
        WasteType wasteType,
        CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.WasteTypes.Add(wasteType);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "creating waste type",
            logParameters: [wasteType.Id, wasteType.Name]);
    }

    public async Task<Result> UpdateAsync(
        WasteType wasteType,
        CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.WasteTypes.Update(wasteType);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "updating waste type",
            logParameters: [wasteType.Id, wasteType.Name]);
    }

    public async Task<Result> DeleteAsync(
        WasteType wasteType,
        CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(
            async () =>
            {
                Context.WasteTypes.Remove(wasteType);
                await Context.SaveChangesAsync(cancellationToken);
            },
            "deleting waste type",
            logParameters: [wasteType.Id]);
    }
}
