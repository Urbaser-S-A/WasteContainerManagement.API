using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Application.Containers.GetContainers;

public class ContainerDto
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public Guid WasteTypeId { get; init; }
    public string? WasteTypeName { get; init; }
    public Guid ZoneId { get; init; }
    public string? ZoneName { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }
    public int CapacityLiters { get; init; }
    public ContainerStatus Status { get; init; }
    public DateTime? InstallationDate { get; init; }
    public DateTime? LastCollectionDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
