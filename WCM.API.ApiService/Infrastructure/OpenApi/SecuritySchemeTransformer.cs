using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

/// <summary>
/// OpenAPI document transformer that adds JWT Bearer authentication security scheme.
/// </summary>
internal sealed class SecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Please enter a valid JWT token obtained from Azure Entra ID. Example: Bearer {your-token}"
        };

        foreach (OpenApiPathItem path in document.Paths.Values)
        {
            if (path.Operations == null) continue;

            foreach (OpenApiOperation operation in path.Operations.Values)
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document, null)] = new List<string>()
                });
            }
        }

        return Task.CompletedTask;
    }
}
