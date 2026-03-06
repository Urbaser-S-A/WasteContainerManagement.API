using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using WCM.API.ApiService.Infrastructure.Middleware;

namespace WCM.API.Tests.Middleware;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock = new();
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _handler = new GlobalExceptionHandler(_loggerMock.Object);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        DefaultHttpContext context = new();
        context.Request.Path = "/api/v1/test";
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<JsonDocument> ReadResponseBody(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        return await JsonDocument.ParseAsync(context.Response.Body);
    }

    [Fact]
    public async Task Should_handle_validation_exception_with_400()
    {
        DefaultHttpContext context = CreateHttpContext();
        List<ValidationFailure> failures = [new ValidationFailure("Name", "Name is required")];
        ValidationException exception = new(failures);

        bool result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        JsonDocument body = await ReadResponseBody(context);
        Assert.Equal("Validation.Error", body.RootElement.GetProperty("title").GetString());
        Assert.Equal("https://httpstatuses.com/400", body.RootElement.GetProperty("type").GetString());
    }

    [Fact]
    public async Task Should_handle_bad_http_request_exception_with_400()
    {
        DefaultHttpContext context = CreateHttpContext();
        BadHttpRequestException exception = new("Bad request body");

        bool result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        JsonDocument body = await ReadResponseBody(context);
        Assert.Equal("Request.BadRequest", body.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task Should_handle_generic_exception_with_500()
    {
        DefaultHttpContext context = CreateHttpContext();
        InvalidOperationException exception = new("Something went wrong");

        bool result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        JsonDocument body = await ReadResponseBody(context);
        Assert.Equal("Server.Error", body.RootElement.GetProperty("title").GetString());
        Assert.Equal("https://httpstatuses.com/500", body.RootElement.GetProperty("type").GetString());
    }

    [Fact]
    public async Task Should_include_trace_id_in_response()
    {
        DefaultHttpContext context = CreateHttpContext();
        context.TraceIdentifier = "test-trace-id";
        InvalidOperationException exception = new("Error");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        JsonDocument body = await ReadResponseBody(context);
        Assert.Equal("test-trace-id", body.RootElement.GetProperty("traceId").GetString());
    }

    [Fact]
    public async Task Should_include_error_code_in_response()
    {
        DefaultHttpContext context = CreateHttpContext();
        InvalidOperationException exception = new("Error");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        JsonDocument body = await ReadResponseBody(context);
        Assert.Equal("Server.Error", body.RootElement.GetProperty("errorCode").GetString());
    }

    [Fact]
    public async Task Should_include_instance_path_in_response()
    {
        DefaultHttpContext context = CreateHttpContext();
        InvalidOperationException exception = new("Error");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        JsonDocument body = await ReadResponseBody(context);
        Assert.Equal("/api/v1/test", body.RootElement.GetProperty("instance").GetString());
    }

    [Fact]
    public async Task Should_group_validation_errors_by_property()
    {
        DefaultHttpContext context = CreateHttpContext();
        List<ValidationFailure> failures =
        [
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Name", "Name must be less than 100 characters"),
            new ValidationFailure("Code", "Code is required")
        ];
        ValidationException exception = new(failures);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        JsonDocument body = await ReadResponseBody(context);
        JsonElement errors = body.RootElement.GetProperty("errors");
        Assert.Equal(2, errors.GetProperty("Name").GetArrayLength());
        Assert.Equal(1, errors.GetProperty("Code").GetArrayLength());
    }
}
