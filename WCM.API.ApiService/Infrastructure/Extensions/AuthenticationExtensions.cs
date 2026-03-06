using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using WCM.API.ApiService.Infrastructure.Authentication;

namespace WCM.API.ApiService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring authentication and authorization per environment.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Configures authentication based on the current environment.
    /// LocalDevelopment: DevelopmentAuthenticationHandler (bypasses real auth).
    /// AzureDevelopment/AzureProduction: Azure Entra ID with JWT Bearer tokens.
    /// </summary>
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        if (!environment.IsEnvironment("LocalDevelopment"))
        {
            // Azure environments: Azure Entra ID authentication with JWT Bearer tokens
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(
                    jwtBearerOptions =>
                    {
                        // Explicit JWT validation parameters for enhanced security
                        jwtBearerOptions.TokenValidationParameters.ValidateIssuer = true;
                        jwtBearerOptions.TokenValidationParameters.ValidateAudience = true;
                        jwtBearerOptions.TokenValidationParameters.ValidateLifetime = true;
                        jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;

                        // Reject tokens with no expiration (security best practice)
                        jwtBearerOptions.TokenValidationParameters.RequireExpirationTime = true;

                        // Reject unsigned tokens
                        jwtBearerOptions.TokenValidationParameters.RequireSignedTokens = true;

                        // Eliminate clock skew tolerance to reject expired tokens immediately
                        // Default is 5 minutes which allows replay attacks within that window
                        jwtBearerOptions.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                    },
                    microsoftIdentityOptions =>
                    {
                        configuration.GetSection("AzureEntraId").Bind(microsoftIdentityOptions);
                    });

            services.AddAuthorization();
        }
        else
        {
            // LocalDevelopment: Bypass authentication for easier local testing
            // Endpoints still have RequireAuthorization() but DevelopmentAuthenticationHandler authenticates all requests
            services.AddAuthentication("DevelopmentScheme")
                .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                    "DevelopmentScheme",
                    options => { });

            services.AddAuthorization(options =>
            {
                // Default policy allows all authenticated users (DevelopmentAuthenticationHandler authenticates everyone)
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        return services;
    }
}
