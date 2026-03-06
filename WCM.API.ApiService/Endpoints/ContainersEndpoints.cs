using MediatR;
using Microsoft.AspNetCore.Mvc;
using WCM.API.ApiService.Application.Containers.CreateContainer;
using WCM.API.ApiService.Application.Containers.DeleteContainer;
using WCM.API.ApiService.Application.Containers.GetContainerById;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Application.Containers.UpdateContainer;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Extensions;

namespace WCM.API.ApiService.Endpoints;

public static class ContainersEndpoints
{
    public static IEndpointRouteBuilder MapContainersEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("api/v1/containers")
            .WithTags("Containers")
            .RequireAuthorization();

        group.MapGet("/", GetContainers)
            .WithName("GetContainers")
            .WithSummary("Retrieves containers with pagination and optional filters")
            .Produces<ContainerListResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapGet("/{id:guid}", GetContainerById)
            .WithName("GetContainerById")
            .WithSummary("Retrieves a container by its ID")
            .Produces<ContainerDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPost("/", CreateContainer)
            .WithName("CreateContainer")
            .WithSummary("Creates a new container")
            .Produces<ContainerDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapPut("/{id:guid}", UpdateContainer)
            .WithName("UpdateContainer")
            .WithSummary("Updates an existing container")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        group.MapDelete("/{id:guid}", DeleteContainer)
            .WithName("DeleteContainer")
            .WithSummary("Deletes a container if it has no open incidents")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static async Task<IResult> GetContainers(
        Guid? zoneId,
        Guid? wasteTypeId,
        ContainerStatus? status,
        int page = 1,
        int pageSize = 20,
        ISender sender = null!,
        HttpContext httpContext = null!,
        CancellationToken cancellationToken = default)
    {
        GetContainersQuery query = new(zoneId, wasteTypeId, status, page, pageSize);
        Result<ContainerListResponse> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> GetContainerById(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        GetContainerByIdQuery query = new(id);
        Result<ContainerDto> result = await sender.Send(query, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> CreateContainer(
        CreateContainerRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        CreateContainerCommand command = new(
            request.Code,
            request.WasteTypeId,
            request.ZoneId,
            request.Latitude,
            request.Longitude,
            request.Address,
            request.CapacityLiters,
            request.Status,
            request.InstallationDate);

        Result<ContainerDto> result = await sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: dto => Results.Created($"/api/v1/containers/{dto.Id}", dto),
            onFailure: error => result.ToHttpResult(httpContext));
    }

    private static async Task<IResult> UpdateContainer(
        Guid id,
        UpdateContainerRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        UpdateContainerCommand command = new(
            id,
            request.Code,
            request.WasteTypeId,
            request.ZoneId,
            request.Latitude,
            request.Longitude,
            request.Address,
            request.CapacityLiters,
            request.Status,
            request.InstallationDate,
            request.LastCollectionDate);

        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }

    private static async Task<IResult> DeleteContainer(
        Guid id,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        DeleteContainerCommand command = new(id);
        Result result = await sender.Send(command, cancellationToken);
        return result.ToHttpResult(httpContext);
    }
}
