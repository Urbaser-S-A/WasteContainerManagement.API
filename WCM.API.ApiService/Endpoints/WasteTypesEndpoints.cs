using Asp.Versioning;
using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WCM.API.ApiService.Application.WasteTypes.CreateWasteType;
using WCM.API.ApiService.Application.WasteTypes.DeleteWasteType;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypeById;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypeByIdV2;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypesV2;
using WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Extensions;

namespace WCM.API.ApiService.Endpoints;

public static class WasteTypesEndpoints
{
    public static RouteGroupBuilder MapWasteTypesEndpoints(this IEndpointRouteBuilder app, ApiVersionSet apiVersionSet)
    {
        RouteGroupBuilder group = app
            .MapGroup("api/v{version:apiVersion}/waste-types")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("WasteTypes")
            .RequireAuthorization();

        // v1 GET endpoints
        group.MapGet("/", GetWasteTypes)
            .WithName("GetWasteTypes")
            .WithSummary("Retrieves all waste types with optional active filter")
            .WithDescription("""
                Returns a list of waste types used to classify containers.

                **Optional parameters:**
                - `isActive` (query): Filter by active/inactive status
                """)
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces<IReadOnlyList<WasteTypeDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", GetWasteTypeById)
            .WithName("GetWasteTypeById")
            .WithSummary("Retrieves a waste type by its ID")
            .WithDescription("""
                Returns a single waste type identified by its GUID.

                **Required parameters:**
                - `id` (route): The waste type unique identifier
                """)
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces<WasteTypeDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // v2 GET endpoints (enriched with activeContainerCount)
        group.MapGet("/", GetWasteTypesV2)
            .WithName("GetWasteTypesV2")
            .WithSummary("Retrieves all waste types with active container count")
            .WithDescription("""
                Returns a list of waste types enriched with the number of active containers per type.

                **Optional parameters:**
                - `isActive` (query): Filter by active/inactive status

                **v2 changes:** Response includes `activeContainerCount` field.
                """)
            .MapToApiVersion(new ApiVersion(2, 0))
            .Produces<IReadOnlyList<WasteTypeV2Dto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", GetWasteTypeByIdV2)
            .WithName("GetWasteTypeByIdV2")
            .WithSummary("Retrieves a waste type by its ID with active container count")
            .WithDescription("""
                Returns a single waste type enriched with the number of active containers.

                **Required parameters:**
                - `id` (route): The waste type unique identifier

                **v2 changes:** Response includes `activeContainerCount` field.
                """)
            .MapToApiVersion(new ApiVersion(2, 0))
            .Produces<WasteTypeV2Dto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("/", CreateWasteType)
            .WithName("CreateWasteType")
            .WithSummary("Creates a new waste type")
            .WithDescription("""
                Creates a new waste type for container classification.
                The name must be unique across all waste types.

                **Required fields:** `name`
                **Optional fields:** `description`, `colorCode` (hex format), `isActive`
                """)
            .Produces<WasteTypeDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:guid}", UpdateWasteType)
            .WithName("UpdateWasteType")
            .WithSummary("Updates an existing waste type")
            .WithDescription("""
                Updates the properties of an existing waste type.
                The name must remain unique across all waste types.

                **Required parameters:**
                - `id` (route): The waste type unique identifier
                """)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:guid}", DeleteWasteType)
            .WithName("DeleteWasteType")
            .WithSummary("Deletes a waste type if it has no active containers")
            .WithDescription("""
                Deletes a waste type. The operation will be rejected if the waste type
                is currently assigned to any active containers.

                **Required parameters:**
                - `id` (route): The waste type unique identifier
                """)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetWasteTypes(
        bool? isActive,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetWasteTypesQuery query = new(isActive);
        Result<IReadOnlyList<WasteTypeDto>> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> GetWasteTypeById(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetWasteTypeByIdQuery query = new(id);
        Result<WasteTypeDto> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> CreateWasteType(
        CreateWasteTypeRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        CreateWasteTypeCommand command = new(
            request.Name,
            request.Description,
            request.ColorCode,
            request.IsActive);

        Result<WasteTypeDto> result = await sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: dto => Results.Created($"/api/v1/waste-types/{dto.Id}", dto),
            onFailure: error => result.ToHttpResult(httpContext));
    }

    private static async Task<IResult> UpdateWasteType(
        Guid id,
        UpdateWasteTypeRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        UpdateWasteTypeCommand command = new(
            id,
            request.Name,
            request.Description,
            request.ColorCode,
            request.IsActive);

        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> DeleteWasteType(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        DeleteWasteTypeCommand command = new(id);
        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    // v2 handlers

    private static async Task<IResult> GetWasteTypesV2(
        bool? isActive,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetWasteTypesV2Query query = new(isActive);
        Result<IReadOnlyList<WasteTypeV2Dto>> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> GetWasteTypeByIdV2(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetWasteTypeByIdV2Query query = new(id);
        Result<WasteTypeV2Dto> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }
}
