using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Interfaces;

public interface IWasteTypeRepository
{
    Task<Result<IReadOnlyList<WasteType>>> GetAllAsync(bool? isActive, CancellationToken cancellationToken);

    Task<Result<WasteType?>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<bool>> ExistsByNameAsync(string name, Guid? excludeId, CancellationToken cancellationToken);

    Task<Result<bool>> HasActiveContainersAsync(Guid id, CancellationToken cancellationToken);

    Task<Result> CreateAsync(WasteType wasteType, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(WasteType wasteType, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(WasteType wasteType, CancellationToken cancellationToken);
}
