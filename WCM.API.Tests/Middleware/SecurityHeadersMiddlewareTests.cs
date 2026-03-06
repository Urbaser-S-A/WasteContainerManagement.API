using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using WCM.API.ApiService.Infrastructure.Middleware;

namespace WCM.API.Tests.Middleware;

public class SecurityHeadersMiddlewareTests
{
    private readonly Mock<IWebHostEnvironment> _environmentMock = new();

    private SecurityHeadersMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new SecurityHeadersMiddleware(next, _environmentMock.Object);
    }

    private static DefaultHttpContext CreateHttpContext(string path = "/api/v1/test", bool isHttps = true)
    {
        DefaultHttpContext context = new();
        context.Request.Path = path;
        context.Request.Scheme = isHttps ? "https" : "http";
        context.Request.IsHttps = isHttps;
        return context;
    }

    [Fact]
    public async Task Should_add_all_security_headers()
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("AzureProduction");
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => Task.CompletedTask);
        DefaultHttpContext context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        Assert.Equal("nosniff", context.Response.Headers["X-Content-Type-Options"]);
        Assert.Equal("DENY", context.Response.Headers["X-Frame-Options"]);
        Assert.Equal("1; mode=block", context.Response.Headers["X-XSS-Protection"]);
        Assert.Equal("no-referrer", context.Response.Headers["Referrer-Policy"]);
        Assert.Equal("default-src 'self'; frame-ancestors 'none';", context.Response.Headers["Content-Security-Policy"]);
        Assert.Equal("geolocation=(), camera=(), microphone=(), payment=()", context.Response.Headers["Permissions-Policy"]);
    }

    [Fact]
    public async Task Should_add_hsts_when_https_and_not_local()
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("AzureProduction");
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => Task.CompletedTask);
        DefaultHttpContext context = CreateHttpContext(isHttps: true);

        await middleware.InvokeAsync(context);

        Assert.Equal("max-age=31536000; includeSubDomains; preload", context.Response.Headers["Strict-Transport-Security"]);
    }

    [Fact]
    public async Task Should_not_add_hsts_in_local_development()
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("LocalDevelopment");
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => Task.CompletedTask);
        DefaultHttpContext context = CreateHttpContext(isHttps: true);

        await middleware.InvokeAsync(context);

        Assert.False(context.Response.Headers.ContainsKey("Strict-Transport-Security"));
    }

    [Fact]
    public async Task Should_not_add_hsts_when_http()
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("AzureProduction");
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => Task.CompletedTask);
        DefaultHttpContext context = CreateHttpContext(isHttps: false);

        await middleware.InvokeAsync(context);

        Assert.False(context.Response.Headers.ContainsKey("Strict-Transport-Security"));
    }

    [Theory]
    [InlineData("/scalar")]
    [InlineData("/scalar/v1")]
    [InlineData("/openapi")]
    [InlineData("/openapi/WCM.API_v1.json")]
    public async Task Should_skip_headers_for_doc_endpoints_in_local_dev(string path)
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("LocalDevelopment");
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => Task.CompletedTask);
        DefaultHttpContext context = CreateHttpContext(path: path);

        await middleware.InvokeAsync(context);

        Assert.False(context.Response.Headers.ContainsKey("X-Content-Type-Options"));
    }

    [Theory]
    [InlineData("/scalar")]
    [InlineData("/openapi")]
    public async Task Should_skip_headers_for_doc_endpoints_in_azure_dev(string path)
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("AzureDevelopment");
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => Task.CompletedTask);
        DefaultHttpContext context = CreateHttpContext(path: path);

        await middleware.InvokeAsync(context);

        Assert.False(context.Response.Headers.ContainsKey("X-Content-Type-Options"));
    }

    [Fact]
    public async Task Should_not_skip_headers_for_doc_endpoints_in_production()
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("AzureProduction");
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => Task.CompletedTask);
        DefaultHttpContext context = CreateHttpContext(path: "/scalar");

        await middleware.InvokeAsync(context);

        Assert.Equal("nosniff", context.Response.Headers["X-Content-Type-Options"]);
    }

    [Fact]
    public async Task Should_call_next_middleware()
    {
        _environmentMock.Setup(e => e.EnvironmentName).Returns("AzureProduction");
        bool nextCalled = false;
        SecurityHeadersMiddleware middleware = CreateMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        DefaultHttpContext context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }
}
