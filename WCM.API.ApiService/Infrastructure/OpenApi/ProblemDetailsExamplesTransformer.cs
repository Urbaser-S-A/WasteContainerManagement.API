using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

internal sealed class ProblemDetailsExamplesTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        SetExample(operation, "400", BuildValidationErrorExample());
        SetExample(operation, "404", BuildNotFoundExample());
        SetExample(operation, "409", BuildConflictExample());
        SetExample(operation, "422", BuildUnprocessableEntityExample());

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

    private static JsonNode BuildValidationErrorExample()
    {
        return new JsonObject
        {
            ["type"] = "https://httpstatuses.com/400",
            ["title"] = "Validation.Error",
            ["status"] = 400,
            ["detail"] = "One or more validation errors occurred.",
            ["instance"] = "/api/v1/containers",
            ["errors"] = new JsonObject
            {
                ["Code"] = new JsonArray { "The 'Code' field is required." },
                ["CapacityLiters"] = new JsonArray { "'Capacity Liters' must be between 50 and 10000." }
            },
            ["traceId"] = "00-abcdef1234567890abcdef1234567890-1234567890abcdef-01",
            ["errorCode"] = "Validation.Error"
        };
    }

    private static JsonNode BuildNotFoundExample()
    {
        return new JsonObject
        {
            ["type"] = "https://httpstatuses.com/404",
            ["title"] = "Resource.NotFound",
            ["status"] = 404,
            ["detail"] = "The requested resource was not found.",
            ["instance"] = "/api/v1/containers/a1b2c3d4-e5f6-7890-abcd-ef1234567890",
            ["traceId"] = "00-abcdef1234567890abcdef1234567890-1234567890abcdef-01",
            ["errorCode"] = "Resource.NotFound"
        };
    }

    private static JsonNode BuildConflictExample()
    {
        return new JsonObject
        {
            ["type"] = "https://httpstatuses.com/409",
            ["title"] = "Resource.Duplicate",
            ["status"] = 409,
            ["detail"] = "A resource with the same unique identifier already exists.",
            ["instance"] = "/api/v1/waste-types",
            ["traceId"] = "00-abcdef1234567890abcdef1234567890-1234567890abcdef-01",
            ["errorCode"] = "Resource.Duplicate"
        };
    }

    private static JsonNode BuildUnprocessableEntityExample()
    {
        return new JsonObject
        {
            ["type"] = "https://httpstatuses.com/422",
            ["title"] = "BusinessRule.Violation",
            ["status"] = 422,
            ["detail"] = "Cannot delete zone because it has active containers assigned.",
            ["instance"] = "/api/v1/zones/d4e5f6a7-b8c9-0123-defa-234567890123",
            ["traceId"] = "00-abcdef1234567890abcdef1234567890-1234567890abcdef-01",
            ["errorCode"] = "BusinessRule.Violation"
        };
    }
}
