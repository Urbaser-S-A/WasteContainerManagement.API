using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Extensions;

namespace WCM.API.Tests.Extensions;

public class ResultExtensionsTests
{
    private static DefaultHttpContext CreateHttpContext()
    {
        DefaultHttpContext context = new();
        context.Request.Path = "/api/v1/test";
        context.TraceIdentifier = "test-trace-id";
        return context;
    }

    [Fact]
    public void ToHttpResult_generic_should_return_ok_when_success_with_value()
    {
        Result<string> result = Result.Success("test-value");
        DefaultHttpContext context = CreateHttpContext();

        IResult httpResult = result.ToHttpResult(context);

        Ok<string> okResult = Assert.IsType<Ok<string>>(httpResult);
        Assert.Equal("test-value", okResult.Value);
    }

    [Fact]
    public void ToHttpResult_generic_should_return_no_content_when_success_with_null()
    {
        Result<string?> result = Result.Success<string?>(null);
        DefaultHttpContext context = CreateHttpContext();

        IResult httpResult = result.ToHttpResult(context);

        Assert.IsType<NoContent>(httpResult);
    }

    [Fact]
    public void ToHttpResult_generic_should_return_problem_when_failure()
    {
        Error error = new("Test.Error", "Test error message", StatusCodes.Status404NotFound);
        Result<string> result = Result.Failure<string>(error);
        DefaultHttpContext context = CreateHttpContext();

        IResult httpResult = result.ToHttpResult(context);

        ProblemHttpResult problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
        Assert.Equal("Test.Error", problemResult.ProblemDetails.Title);
        Assert.Equal("Test error message", problemResult.ProblemDetails.Detail);
    }

    [Fact]
    public void ToHttpResult_should_return_no_content_when_success()
    {
        Result result = Result.Success();
        DefaultHttpContext context = CreateHttpContext();

        IResult httpResult = result.ToHttpResult(context);

        Assert.IsType<NoContent>(httpResult);
    }

    [Fact]
    public void ToHttpResult_should_return_problem_when_failure()
    {
        Error error = new("Test.Error", "Test error message", StatusCodes.Status409Conflict);
        Result result = Result.Failure(error);
        DefaultHttpContext context = CreateHttpContext();

        IResult httpResult = result.ToHttpResult(context);

        ProblemHttpResult problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status409Conflict, problemResult.StatusCode);
    }

    [Fact]
    public void ToHttpResult_should_include_error_code_extension()
    {
        Error error = new("Custom.Code", "Custom message", StatusCodes.Status422UnprocessableEntity);
        Result result = Result.Failure(error);
        DefaultHttpContext context = CreateHttpContext();

        IResult httpResult = result.ToHttpResult(context);

        ProblemHttpResult problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal("Custom.Code", problemResult.ProblemDetails.Extensions["errorCode"]);
    }

    [Fact]
    public void ToHttpResult_should_include_trace_id_extension()
    {
        Error error = new("Test.Error", "Message", StatusCodes.Status400BadRequest);
        Result result = Result.Failure(error);
        DefaultHttpContext context = CreateHttpContext();

        IResult httpResult = result.ToHttpResult(context);

        ProblemHttpResult problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.NotNull(problemResult.ProblemDetails.Extensions["traceId"]);
    }
}
