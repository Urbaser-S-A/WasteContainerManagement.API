using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WCM.API.ApiService.Domain.Entities;

namespace WCM.API.ApiService.Infrastructure.Persistence;

public interface IApplicationDbContext : IDisposable
{
    DbSet<WasteType> WasteTypes { get; }

    DbSet<Zone> Zones { get; }

    DbSet<Container> Containers { get; }

    DbSet<Incident> Incidents { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
