using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Application.Containers.UpdateContainer;

public class UpdateContainerRequest
{
    public required string Code { get; init; }
    public Guid WasteTypeId { get; init; }
    public Guid ZoneId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }
    public int CapacityLiters { get; init; }
    public ContainerStatus Status { get; init; }
    public DateTime? InstallationDate { get; init; }
    public DateTime? LastCollectionDate { get; init; }
}
