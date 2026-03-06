using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

/// <summary>
/// OpenAPI document transformer that configures API metadata (title, version, description, contact, license).
/// </summary>
internal sealed class OpenApiInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        string documentName = context.DocumentName;

        document.Info = new OpenApiInfo
        {
            Title = "WCM API",
            Version = documentName,
            Description = "Waste Container Management API - POC for enterprise architecture validation with OpenChoreo.",
            Contact = new OpenApiContact
            {
                Name = "WCM API",
                Email = "wcm@urbaser.com"
            },
            License = new OpenApiLicense
            {
                Name = "Internal Use Only"
            }
        };

        return Task.CompletedTask;
    }
}
