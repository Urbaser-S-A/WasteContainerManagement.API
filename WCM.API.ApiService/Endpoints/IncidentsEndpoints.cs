using MediatR;
using Microsoft.AspNetCore.Mvc;
using WCM.API.ApiService.Application.Incidents.CreateIncident;
using WCM.API.ApiService.Application.Incidents.DeleteIncident;
using WCM.API.ApiService.Application.Incidents.GetIncidentById;
using WCM.API.ApiService.Application.Incidents.GetIncidents;
using WCM.API.ApiService.Application.Incidents.UpdateIncident;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Extensions;

namespace WCM.API.ApiService.Endpoints;

public static class IncidentsEndpoints
{
    public static IEndpointRouteBuilder MapIncidentsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("api/v1/incidents")
            .WithTags("Incidents")
            .RequireAuthorization();

        group.MapGet("/", GetIncidents)
            .WithName("GetIncidents")
            .WithSummary("Retrieves incidents with pagination and optional filters")
            .Produces<IncidentListResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapGet("/{id:guid}", GetIncidentById)
            .WithName("GetIncidentById")
            .WithSummary("Retrieves an incident by its ID")
            .Produces<IncidentDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPost("/", CreateIncident)
            .WithName("CreateIncident")
            .WithSummary("Creates a new incident for a container")
            .Produces<IncidentDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPut("/{id:guid}", UpdateIncident)
            .WithName("UpdateIncident")
            .WithSummary("Updates an existing incident")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapDelete("/{id:guid}", DeleteIncident)
            .WithName("DeleteIncident")
            .WithSummary("Deletes an incident")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static async Task<IResult> GetIncidents(
        Guid? containerId,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        DateTime? fromDate,
        DateTime? toDate,
        int page = 1,
        int pageSize = 20,
        ISender sender = null!,
        HttpContext httpContext = null!,
        CancellationToken cancellationToken = default)
    {
        GetIncidentsQuery query = new(containerId, type, status, priority, fromDate, toDate, page, pageSize);
        Result<IncidentListResponse> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> GetIncidentById(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetIncidentByIdQuery query = new(id);
        Result<IncidentDto> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> CreateIncident(
        CreateIncidentRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        CreateIncidentCommand command = new(
            request.ContainerId,
            request.Type,
            request.Description,
            request.Priority);

        Result<IncidentDto> result = await sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: dto => Results.Created($"/api/v1/incidents/{dto.Id}", dto),
            onFailure: error => result.ToHttpResult(httpContext));
    }

    private static async Task<IResult> UpdateIncident(
        Guid id,
        UpdateIncidentRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        UpdateIncidentCommand command = new(
            id,
            request.Type,
            request.Description,
            request.Status,
            request.Priority,
            request.ResolvedAt);

        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> DeleteIncident(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        DeleteIncidentCommand command = new(id);
        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }
}
