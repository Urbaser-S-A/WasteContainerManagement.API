namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypesV2;

/// <summary>
/// Enriched waste type DTO for API v2. Includes active container count.
/// </summary>
public class WasteTypeV2Dto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? ColorCode { get; init; }
    public bool IsActive { get; init; }
    public int ActiveContainerCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
