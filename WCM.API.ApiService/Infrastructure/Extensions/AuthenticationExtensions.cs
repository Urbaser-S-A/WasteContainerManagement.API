using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using WCM.API.ApiService.Infrastructure.Authentication;

namespace WCM.API.ApiService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring authentication and authorization.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Configures the API to require no authentication. A development scheme is still registered
    /// so the authentication/authorization middleware has a scheme to run, and the authorization
    /// policy allows every request (including anonymous). Wire in a real identity provider
    /// (JWT/OIDC) here when one is available for the target environment.
    /// </summary>
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddAuthentication("DevelopmentScheme")
            .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                "DevelopmentScheme",
                options => { });

        // Open policy: authorize every request, including anonymous ones. This makes any
        // RequireAuthorization() and the fallback policy pass without a token.
        AuthorizationPolicy allowAnonymous = new AuthorizationPolicyBuilder()
            .RequireAssertion(_ => true)
            .Build();

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = allowAnonymous;
            options.FallbackPolicy = allowAnonymous;
        });

        return services;
    }
}
