using WCM.API.ApiService.Domain.Shared;
using System.Diagnostics;

namespace WCM.API.ApiService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for handling Result types in Minimal API endpoints.
/// Provides standardized result handling and error mapping.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result{T} to an appropriate IResult for Minimal APIs.
    /// Returns 200 OK with value if successful, 204 No Content if value is null, or a problem response if failed.
    /// </summary>
    public static IResult ToHttpResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result is null)
        {
            return CreateProblemResult(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "General.Error",
                "The operation result is null.");
        }

        if (result.IsSuccess)
        {
            if (result.Value is null)
            {
                return Results.NoContent();
            }

            return Results.Ok(result.Value);
        }

        return CreateProblemResultFromError(httpContext, result.Error);
    }

    /// <summary>
    /// Converts a Result to an appropriate IResult for Minimal APIs.
    /// Returns 204 No Content if successful, or a problem response if failed.
    /// </summary>
    public static IResult ToHttpResult(this Result result, HttpContext httpContext)
    {
        if (result is null)
        {
            return CreateProblemResult(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "General.Error",
                "The operation result is null.");
        }

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return CreateProblemResultFromError(httpContext, result.Error);
    }

    private static IResult CreateProblemResultFromError(HttpContext httpContext, Error error)
    {
        return CreateProblemResult(
            httpContext,
            error.StatusCode,
            error.Code,
            error.Message);
    }

    private static IResult CreateProblemResult(
        HttpContext httpContext,
        int statusCode,
        string errorCode,
        string detail)
    {
        string traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier ?? string.Empty;
        string instance = httpContext?.Request?.Path.ToString() ?? string.Empty;

        return Results.Problem(
            detail: detail,
            statusCode: statusCode,
            title: errorCode,
            type: $"https://httpstatuses.com/{statusCode}",
            instance: instance,
            extensions: new Dictionary<string, object?>
            {
                ["traceId"] = traceId,
                ["errorCode"] = errorCode
            });
    }
}
