using MediatR;
using Microsoft.AspNetCore.Mvc;
using WCM.API.ApiService.Application.WasteTypes.CreateWasteType;
using WCM.API.ApiService.Application.WasteTypes.DeleteWasteType;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypeById;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Extensions;

namespace WCM.API.ApiService.Endpoints;

public static class WasteTypesEndpoints
{
    public static IEndpointRouteBuilder MapWasteTypesEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("api/v1/waste-types")
            .WithTags("WasteTypes")
            .RequireAuthorization();

        group.MapGet("/", GetWasteTypes)
            .WithName("GetWasteTypes")
            .WithSummary("Retrieves all waste types with optional active filter")
            .Produces<IReadOnlyList<WasteTypeDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapGet("/{id:guid}", GetWasteTypeById)
            .WithName("GetWasteTypeById")
            .WithSummary("Retrieves a waste type by its ID")
            .Produces<WasteTypeDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPost("/", CreateWasteType)
            .WithName("CreateWasteType")
            .WithSummary("Creates a new waste type")
            .Produces<WasteTypeDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPut("/{id:guid}", UpdateWasteType)
            .WithName("UpdateWasteType")
            .WithSummary("Updates an existing waste type")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapDelete("/{id:guid}", DeleteWasteType)
            .WithName("DeleteWasteType")
            .WithSummary("Deletes a waste type if it has no active containers")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        return app;
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
}
