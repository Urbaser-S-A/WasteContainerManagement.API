using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.ApiService.Application.Interfaces;

public interface IContainerRepository
{
    Task<Result<IReadOnlyList<Container>>> GetAllAsync(
        Guid? zoneId,
        Guid? wasteTypeId,
        ContainerStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Result<int>> GetTotalCountAsync(
        Guid? zoneId,
        Guid? wasteTypeId,
        ContainerStatus? status,
        CancellationToken cancellationToken);

    Task<Result<Container?>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<bool>> ExistsByCodeAsync(string code, Guid? excludeId, CancellationToken cancellationToken);

    Task<Result<bool>> HasOpenIncidentsAsync(Guid id, CancellationToken cancellationToken);

    Task<Result> CreateAsync(Container container, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Container container, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Container container, CancellationToken cancellationToken);
}
