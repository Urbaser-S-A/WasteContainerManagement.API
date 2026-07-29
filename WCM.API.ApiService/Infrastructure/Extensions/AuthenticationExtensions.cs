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
    /// Configures authentication using a development scheme that authenticates every request.
    /// The application no longer depends on Azure Entra ID. Replace this with a real identity
    /// provider (JWT/OIDC) when one is available for the target environment.
    /// </summary>
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Bypass authentication: DevelopmentAuthenticationHandler authenticates all requests.
        // Endpoints still call RequireAuthorization(), which the fallback policy satisfies.
        services.AddAuthentication("DevelopmentScheme")
            .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                "DevelopmentScheme",
                options => { });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
