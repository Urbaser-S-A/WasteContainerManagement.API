using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Interfaces;

public interface IZoneRepository
{
    Task<Result<IReadOnlyList<Zone>>> GetAllAsync(string? district, string? city, bool? isActive, CancellationToken cancellationToken);

    Task<Result<Zone?>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<bool>> ExistsByNameAsync(string name, Guid? excludeId, CancellationToken cancellationToken);

    Task<Result<bool>> HasActiveContainersAsync(Guid id, CancellationToken cancellationToken);

    Task<Result> CreateAsync(Zone zone, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Zone zone, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Zone zone, CancellationToken cancellationToken);
}
