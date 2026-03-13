using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

internal sealed class IncidentsExamplesTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        string? operationId = operation.OperationId;

        switch (operationId)
        {
            case "GetIncidents":
                SetExample(operation, "200", BuildListExample());
                break;
            case "GetIncidentById":
                SetExample(operation, "200", BuildSingleExample());
                break;
            case "CreateIncident":
                SetExample(operation, "201", BuildSingleExample());
                break;
        }

        return Task.CompletedTask;
    }

    private static void SetExample(OpenApiOperation operation, string statusCode, JsonNode example)
    {
        if (operation.Responses is null
            || !operation.Responses.TryGetValue(statusCode, out IOpenApiResponse? response)
            || response is null
            || response.Content is null
            || !response.Content.TryGetValue("application/json", out OpenApiMediaType? mediaType))
        {
            return;
        }

        mediaType.Example = example;
    }

    private static JsonNode BuildSingleExample()
    {
        return new JsonObject
        {
            ["id"] = "b8c9d0e1-f2a3-4567-b012-678901234567",
            ["containerId"] = "f6a7b8c9-d0e1-2345-fab0-456789012345",
            ["containerCode"] = "CNT-2026-001",
            ["type"] = "Overflow",
            ["description"] = "Container is overflowing with organic waste after weekend",
            ["status"] = "Open",
            ["priority"] = "High",
            ["reportedAt"] = "2026-03-03T08:15:00Z",
            ["resolvedAt"] = (JsonNode?)null,
            ["createdAt"] = "2026-03-03T08:15:00Z",
            ["updatedAt"] = (JsonNode?)null
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
                    ["id"] = "b8c9d0e1-f2a3-4567-b012-678901234567",
                    ["containerId"] = "f6a7b8c9-d0e1-2345-fab0-456789012345",
                    ["containerCode"] = "CNT-2026-001",
                    ["type"] = "Overflow",
                    ["description"] = "Container is overflowing with organic waste after weekend",
                    ["status"] = "Open",
                    ["priority"] = "High",
                    ["reportedAt"] = "2026-03-03T08:15:00Z",
                    ["resolvedAt"] = (JsonNode?)null,
                    ["createdAt"] = "2026-03-03T08:15:00Z",
                    ["updatedAt"] = (JsonNode?)null
                },
                new JsonObject
                {
                    ["id"] = "c9d0e1f2-a3b4-5678-c123-789012345678",
                    ["containerId"] = "a7b8c9d0-e1f2-3456-ab01-567890123456",
                    ["containerCode"] = "CNT-2026-002",
                    ["type"] = "Damage",
                    ["description"] = "Lid mechanism is broken, does not close properly",
                    ["status"] = "Resolved",
                    ["priority"] = "Medium",
                    ["reportedAt"] = "2026-02-28T14:30:00Z",
                    ["resolvedAt"] = "2026-03-02T10:00:00Z",
                    ["createdAt"] = "2026-02-28T14:30:00Z",
                    ["updatedAt"] = "2026-03-02T10:00:00Z"
                }
            },
            ["totalCount"] = 12,
            ["page"] = 1,
            ["pageSize"] = 20,
            ["totalPages"] = 1
        };
    }
}
