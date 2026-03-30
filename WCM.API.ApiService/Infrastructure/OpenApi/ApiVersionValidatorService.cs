using Asp.Versioning.ApiExplorer;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

/// <summary>
/// Background service that validates API version synchronization between Asp.Versioning and OpenAPI document registration.
/// </summary>
/// <remarks>
/// CONTEXT: Microsoft.AspNetCore.OpenApi requires explicit document registration BEFORE IServiceProvider is built.
/// This prevents dynamic discovery of API versions via IApiVersionDescriptionProvider (unlike Swashbuckle).
///
/// WORKAROUND: API versions must be manually registered in AddOpenApiWithVersioning() using a static array.
/// This service validates at startup that:
/// - All discovered API versions have corresponding OpenAPI documents registered
/// - No extra OpenAPI documents are registered without actual API versions
///
/// When to update knownVersions array in ServiceCollectionExtensions.cs:
/// - Adding a new API version (e.g., v2): Add "v2" to knownVersions array
/// - Deprecating an old API version: Remove from knownVersions array (or keep if still serving deprecated endpoints)
///
/// Usage: Registered automatically via AddOpenApiWithVersioning() in ServiceCollectionExtensions.
/// </remarks>
internal sealed class ApiVersionValidatorService(
    IApiVersionDescriptionProvider provider,
    ILogger<ApiVersionValidatorService> logger) : IHostedService
{
    private static readonly string[] KnownVersions = ApiVersionConstants.KnownVersions;

    /// <summary>
    /// Validates API version synchronization when the application starts.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        string[] discoveredVersions = provider.ApiVersionDescriptions
            .Select(d => d.GroupName)
            .ToArray();

        string[] missingVersions = discoveredVersions.Except(KnownVersions).ToArray();
        string[] extraVersions = KnownVersions.Except(discoveredVersions).ToArray();

        if (missingVersions.Length > 0)
        {
            logger.LogWarning(
                "OPENAPI CONFIGURATION WARNING: API versions {MissingVersions} are defined but not registered in AddOpenApiWithVersioning. " +
                "Update ServiceCollectionExtensions.cs to add them to the knownVersions array.",
                string.Join(", ", missingVersions));
        }

        if (extraVersions.Length > 0)
        {
            logger.LogWarning(
                "OPENAPI CONFIGURATION WARNING: OpenAPI documents {ExtraVersions} are registered but no API version exists. " +
                "Remove them from knownVersions array in ServiceCollectionExtensions.cs or add corresponding API version.",
                string.Join(", ", extraVersions));
        }

        if (missingVersions.Length == 0 && extraVersions.Length == 0)
        {
            logger.LogInformation(
                "OpenAPI version validation: All API versions {Versions} are correctly registered",
                string.Join(", ", discoveredVersions));
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// No-op stop implementation.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
