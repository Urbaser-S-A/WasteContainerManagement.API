namespace WCM.API.ApiService.Infrastructure.OpenApi;

/// <summary>
/// Centralized constants for API version management.
/// Update this array when adding or removing API versions.
/// </summary>
internal static class ApiVersionConstants
{
    /// <summary>
    /// Known API versions that must be kept in sync with actual API version definitions.
    /// Used by both OpenAPI document registration and validation.
    /// </summary>
    internal static readonly string[] KnownVersions = ["v1"];
}
