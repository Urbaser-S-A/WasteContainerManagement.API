namespace WCM.API.ApiService.Application.WasteTypes.CreateWasteType;

public class CreateWasteTypeRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? ColorCode { get; init; }
    public bool IsActive { get; init; } = true;
}
