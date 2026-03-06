namespace WCM.API.ApiService.Application.Incidents.GetIncidents;

public class IncidentListResponse
{
    public required IReadOnlyList<IncidentDto> Items { get; init; }
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}
