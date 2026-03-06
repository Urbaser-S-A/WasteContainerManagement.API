using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

internal sealed class ZonesExamplesTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        string? operationId = operation.OperationId;

        switch (operationId)
        {
            case "GetZones":
                SetExample(operation, "200", BuildListExample());
                break;
            case "GetZoneById":
                SetExample(operation, "200", BuildSingleExample());
                break;
            case "CreateZone":
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
            ["id"] = "d4e5f6a7-b8c9-0123-defa-234567890123",
            ["name"] = "Zone North-A",
            ["district"] = "Northern District",
            ["city"] = "Madrid",
            ["isActive"] = true,
            ["createdAt"] = "2026-01-16T09:00:00Z",
            ["updatedAt"] = (JsonNode?)null
        };
    }

    private static JsonNode BuildListExample()
    {
        return new JsonArray
        {
            new JsonObject
            {
                ["id"] = "d4e5f6a7-b8c9-0123-defa-234567890123",
                ["name"] = "Zone North-A",
                ["district"] = "Northern District",
                ["city"] = "Madrid",
                ["isActive"] = true,
                ["createdAt"] = "2026-01-16T09:00:00Z",
                ["updatedAt"] = (JsonNode?)null
            },
            new JsonObject
            {
                ["id"] = "e5f6a7b8-c9d0-1234-efab-345678901234",
                ["name"] = "Zone South-B",
                ["district"] = "Southern District",
                ["city"] = "Madrid",
                ["isActive"] = true,
                ["createdAt"] = "2026-01-16T09:15:00Z",
                ["updatedAt"] = "2026-02-10T11:30:00Z"
            }
        };
    }
}
