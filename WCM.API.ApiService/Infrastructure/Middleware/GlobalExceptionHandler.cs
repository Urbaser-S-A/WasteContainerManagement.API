using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WCM.API.ApiService.Infrastructure.Middleware;

/// <summary>
/// Global exception handler that converts exceptions into standardized ProblemDetails responses.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Exception occurred: {Message}",
            exception.Message);

        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        switch (exception)
        {
            case ValidationException validationException:
                ValidationProblemDetails validationProblemDetails = new ValidationProblemDetails(
                    validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation.Error",
                    Detail = "One or more validation errors occurred.",
                    Type = "https://httpstatuses.com/400",
                    Instance = httpContext.Request.Path
                };

                validationProblemDetails.Extensions["traceId"] = traceId;
                validationProblemDetails.Extensions["errorCode"] = "Validation.Error";

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
                break;

            case BadHttpRequestException badRequestException:
                ProblemDetails badRequestProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Request.BadRequest",
                    Detail = badRequestException.Message,
                    Type = "https://httpstatuses.com/400",
                    Instance = httpContext.Request.Path
                };

                badRequestProblemDetails.Extensions["traceId"] = traceId;
                badRequestProblemDetails.Extensions["errorCode"] = "Request.BadRequest";

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(badRequestProblemDetails, cancellationToken);
                break;

            default:
                ProblemDetails serverErrorProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server.Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Type = "https://httpstatuses.com/500",
                    Instance = httpContext.Request.Path
                };

                serverErrorProblemDetails.Extensions["traceId"] = traceId;
                serverErrorProblemDetails.Extensions["errorCode"] = "Server.Error";

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(serverErrorProblemDetails, cancellationToken);
                break;
        }

        return true;
    }
}
