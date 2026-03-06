namespace WCM.API.ApiService.Infrastructure.Middleware;

/// <summary>
/// Middleware that adds security headers to all HTTP responses to protect against common web vulnerabilities.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public SecurityHeadersMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip security headers for API documentation endpoints in LocalDevelopment and AzureDevelopment
        // Scalar and OpenAPI require more relaxed CSP policies to function properly
        // AzureProduction: Scalar is disabled in Program.cs, so this check has no effect
        if ((_environment.IsEnvironment("LocalDevelopment") || _environment.IsEnvironment("AzureDevelopment")) &&
            (context.Request.Path.StartsWithSegments("/scalar") ||
             context.Request.Path.StartsWithSegments("/openapi")))
        {
            await _next(context);
            return;
        }

        // X-Content-Type-Options: nosniff
        // Prevents MIME type sniffing attacks by forcing browsers to respect the declared Content-Type
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // X-Frame-Options: DENY
        // Prevents clickjacking attacks by not allowing the page to be displayed in a frame/iframe
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // X-XSS-Protection: 1; mode=block
        // Enables XSS filtering in browsers (legacy but still useful for older browsers)
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // Referrer-Policy: no-referrer
        // Controls how much referrer information should be included with requests
        context.Response.Headers.Append("Referrer-Policy", "no-referrer");

        // Content-Security-Policy: default-src 'self'
        // Helps prevent XSS attacks by controlling which resources can be loaded
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");

        // Permissions-Policy (formerly Feature-Policy)
        // Controls which browser features and APIs can be used
        context.Response.Headers.Append("Permissions-Policy",
            "geolocation=(), camera=(), microphone=(), payment=()");

        // Strict-Transport-Security (HSTS)
        // Forces browsers to only use HTTPS for future requests
        // max-age=31536000: 1 year, includeSubDomains: applies to all subdomains
        // Only add HSTS when:
        // 1. Request is over HTTPS (HSTS header is ignored on HTTP responses)
        // 2. Not in LocalDevelopment (avoids issues with local HTTP development)
        if (context.Request.IsHttps && !_environment.IsEnvironment("LocalDevelopment"))
        {
            context.Response.Headers.Append("Strict-Transport-Security",
                "max-age=31536000; includeSubDomains; preload");
        }

        // Remove sensitive headers that expose server information
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-AspNetMvc-Version");

        await _next(context);
    }
}
