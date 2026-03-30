using Asp.Versioning;
using Asp.Versioning.Builder;

namespace WCM.API.ApiService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring Minimal API endpoint routing.
/// </summary>
internal static class EndpointExtensions
{
    /// <summary>
    /// Creates the default API version set with version 1.0 and version reporting enabled.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A configured <see cref="ApiVersionSet"/>.</returns>
    internal static ApiVersionSet CreateDefaultApiVersionSet(this IEndpointRouteBuilder app)
    {
        return app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();
    }
}
