using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WCM.API.ApiService.Infrastructure.Authentication;

/// <summary>
/// Development-only authentication handler that bypasses real authentication.
/// Automatically authenticates all requests with a fake user identity.
/// This handler is ONLY used in LocalDevelopment environment to facilitate local testing without Azure Entra ID.
/// </summary>
/// <remarks>
/// WARNING: This handler provides NO SECURITY and should NEVER be used in Azure environments.
/// The Program.cs file ensures this handler is only registered when IsEnvironment("LocalDevelopment") is true.
/// </remarks>
public class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.Name, "Development User"),
            new Claim(ClaimTypes.NameIdentifier, "dev-user-id"),
            new Claim(ClaimTypes.Email, "dev@localhost"),
            new Claim("preferred_username", "dev@localhost"),
        ];

        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

        Logger.LogDebug("Development authentication: Auto-authenticating request as 'Development User'");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
