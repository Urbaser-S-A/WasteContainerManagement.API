namespace WCM.API.ApiService.Application.Interfaces;

/// <summary>
/// Projection model for waste types with their active container count.
/// Used by v2 endpoints to return enriched data.
/// </summary>
public class WasteTypeWithContainerCount
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
