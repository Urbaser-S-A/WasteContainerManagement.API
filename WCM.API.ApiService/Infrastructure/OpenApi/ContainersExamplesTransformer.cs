using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

internal sealed class ContainersExamplesTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        string? operationId = operation.OperationId;

        switch (operationId)
        {
            case "GetContainers":
                SetExample(operation, "200", BuildListExample());
                break;
            case "GetContainerById":
                SetExample(operation, "200", BuildSingleExample());
                break;
            case "CreateContainer":
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
            ["id"] = "f6a7b8c9-d0e1-2345-fab0-456789012345",
            ["code"] = "CNT-2026-001",
            ["wasteTypeId"] = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
            ["wasteTypeName"] = "Organic",
            ["zoneId"] = "d4e5f6a7-b8c9-0123-defa-234567890123",
            ["zoneName"] = "Zone North-A",
            ["latitude"] = 40.4168,
            ["longitude"] = -3.7038,
            ["address"] = "Calle Gran Via 28, Madrid",
            ["capacityLiters"] = 1100,
            ["status"] = "Active",
            ["installationDate"] = "2026-01-20T00:00:00Z",
            ["lastCollectionDate"] = "2026-03-05T07:30:00Z",
            ["createdAt"] = "2026-01-20T08:00:00Z",
            ["updatedAt"] = "2026-03-05T07:30:00Z"
        };
    }

    private static JsonNode BuildListExample()
    {
        return new JsonObject
        {
            ["items"] = new JsonArray
            {
                new JsonObject
                {
                    ["id"] = "f6a7b8c9-d0e1-2345-fab0-456789012345",
                    ["code"] = "CNT-2026-001",
                    ["wasteTypeId"] = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                    ["wasteTypeName"] = "Organic",
                    ["zoneId"] = "d4e5f6a7-b8c9-0123-defa-234567890123",
                    ["zoneName"] = "Zone North-A",
                    ["latitude"] = 40.4168,
                    ["longitude"] = -3.7038,
                    ["address"] = "Calle Gran Via 28, Madrid",
                    ["capacityLiters"] = 1100,
                    ["status"] = "Active",
                    ["installationDate"] = "2026-01-20T00:00:00Z",
                    ["lastCollectionDate"] = "2026-03-05T07:30:00Z",
                    ["createdAt"] = "2026-01-20T08:00:00Z",
                    ["updatedAt"] = "2026-03-05T07:30:00Z"
                },
                new JsonObject
                {
                    ["id"] = "a7b8c9d0-e1f2-3456-ab01-567890123456",
                    ["code"] = "CNT-2026-002",
                    ["wasteTypeId"] = "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                    ["wasteTypeName"] = "Plastic",
                    ["zoneId"] = "d4e5f6a7-b8c9-0123-defa-234567890123",
                    ["zoneName"] = "Zone North-A",
                    ["latitude"] = 40.4175,
                    ["longitude"] = -3.7045,
                    ["address"] = "Calle Gran Via 32, Madrid",
                    ["capacityLiters"] = 800,
                    ["status"] = "Active",
                    ["installationDate"] = "2026-01-20T00:00:00Z",
                    ["lastCollectionDate"] = "2026-03-04T06:45:00Z",
                    ["createdAt"] = "2026-01-20T08:15:00Z",
                    ["updatedAt"] = "2026-03-04T06:45:00Z"
                }
            },
            ["totalCount"] = 47,
            ["page"] = 1,
            ["pageSize"] = 20,
            ["totalPages"] = 3
        };
    }
}
