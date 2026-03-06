using MediatR;
using Microsoft.AspNetCore.Mvc;
using WCM.API.ApiService.Application.Zones.CreateZone;
using WCM.API.ApiService.Application.Zones.DeleteZone;
using WCM.API.ApiService.Application.Zones.GetZoneById;
using WCM.API.ApiService.Application.Zones.GetZones;
using WCM.API.ApiService.Application.Zones.UpdateZone;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Extensions;

namespace WCM.API.ApiService.Endpoints;

public static class ZonesEndpoints
{
    public static IEndpointRouteBuilder MapZonesEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("api/v1/zones")
            .WithTags("Zones")
            .RequireAuthorization();

        group.MapGet("/", GetZones)
            .WithName("GetZones")
            .WithSummary("Retrieves all zones with optional filters")
            .Produces<IReadOnlyList<ZoneDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapGet("/{id:guid}", GetZoneById)
            .WithName("GetZoneById")
            .WithSummary("Retrieves a zone by its ID")
            .Produces<ZoneDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPost("/", CreateZone)
            .WithName("CreateZone")
            .WithSummary("Creates a new zone")
            .Produces<ZoneDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPut("/{id:guid}", UpdateZone)
            .WithName("UpdateZone")
            .WithSummary("Updates an existing zone")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapDelete("/{id:guid}", DeleteZone)
            .WithName("DeleteZone")
            .WithSummary("Deletes a zone if it has no active containers")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static async Task<IResult> GetZones(
        string? district,
        string? city,
        bool? isActive,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetZonesQuery query = new(district, city, isActive);
        Result<IReadOnlyList<ZoneDto>> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> GetZoneById(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetZoneByIdQuery query = new(id);
        Result<ZoneDto> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> CreateZone(
        CreateZoneRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        CreateZoneCommand command = new(
            request.Name,
            request.District,
            request.City,
            request.IsActive);

        Result<ZoneDto> result = await sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: dto => Results.Created($"/api/v1/zones/{dto.Id}", dto),
            onFailure: error => result.ToHttpResult(httpContext));
    }

    private static async Task<IResult> UpdateZone(
        Guid id,
        UpdateZoneRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        UpdateZoneCommand command = new(
            id,
            request.Name,
            request.District,
            request.City,
            request.IsActive);

        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> DeleteZone(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        DeleteZoneCommand command = new(id);
        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }
}
