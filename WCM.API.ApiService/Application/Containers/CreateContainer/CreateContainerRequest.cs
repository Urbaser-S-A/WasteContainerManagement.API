using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Application.Containers.CreateContainer;

public class CreateContainerRequest
{
    public required string Code { get; init; }
    public Guid WasteTypeId { get; init; }
    public Guid ZoneId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }
    public int CapacityLiters { get; init; }
    public ContainerStatus Status { get; init; } = ContainerStatus.Active;
    public DateTime? InstallationDate { get; init; }
}
