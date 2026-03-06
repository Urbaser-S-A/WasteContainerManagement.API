using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

internal sealed class WasteTypesExamplesTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        string? operationId = operation.OperationId;

        switch (operationId)
        {
            case "GetWasteTypes":
                SetExample(operation, "200", BuildListExample());
                break;
            case "GetWasteTypeById":
                SetExample(operation, "200", BuildSingleExample());
                break;
            case "CreateWasteType":
                SetExample(operation, "201", BuildSingleExample());
                break;
        }

        return Task.CompletedTask;
    }

    private static void SetExample(OpenApiOperation operation, string statusCode, JsonNode example)
    {
        if (!operation.Responses.TryGetValue(statusCode, out IOpenApiResponse? response)
            || response?.Content is null
            || !response.Content.TryGetValue("application/json", out OpenApiMediaType? mediaType)
            || mediaType is null)
        {
            return;
        }

        mediaType.Example = example;
    }

    private static JsonNode BuildSingleExample()
    {
        return new JsonObject
        {
            ["id"] = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
            ["name"] = "Organic",
            ["description"] = "Organic and biodegradable waste",
            ["colorCode"] = "#8B4513",
            ["isActive"] = true,
            ["createdAt"] = "2026-01-15T10:30:00Z",
            ["updatedAt"] = (JsonNode?)null
        };
    }

    private static JsonNode BuildListExample()
    {
        return new JsonArray
        {
            new JsonObject
            {
                ["id"] = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                ["name"] = "Organic",
                ["description"] = "Organic and biodegradable waste",
                ["colorCode"] = "#8B4513",
                ["isActive"] = true,
                ["createdAt"] = "2026-01-15T10:30:00Z",
                ["updatedAt"] = (JsonNode?)null
            },
            new JsonObject
            {
                ["id"] = "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                ["name"] = "Plastic",
                ["description"] = "Plastic containers and packaging",
                ["colorCode"] = "#FFD700",
                ["isActive"] = true,
                ["createdAt"] = "2026-01-15T10:35:00Z",
                ["updatedAt"] = "2026-02-20T14:00:00Z"
            },
            new JsonObject
            {
                ["id"] = "c3d4e5f6-a7b8-9012-cdef-123456789012",
                ["name"] = "Glass",
                ["description"] = "Glass bottles and containers",
                ["colorCode"] = "#228B22",
                ["isActive"] = true,
                ["createdAt"] = "2026-01-15T10:40:00Z",
                ["updatedAt"] = (JsonNode?)null
            }
        };
    }
}
