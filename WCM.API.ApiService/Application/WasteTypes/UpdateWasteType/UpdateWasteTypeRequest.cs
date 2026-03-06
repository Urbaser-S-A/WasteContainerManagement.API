namespace WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;

public class UpdateWasteTypeRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? ColorCode { get; init; }
    public bool IsActive { get; init; }
}
