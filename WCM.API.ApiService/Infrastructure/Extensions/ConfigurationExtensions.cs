using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;

namespace WCM.API.ApiService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring application configuration sources.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds Azure Key Vault as a configuration source using managed identity (DefaultAzureCredential).
    /// LocalDevelopment: Skipped (uses User Secrets instead).
    /// AzureDevelopment/AzureProduction: Loads from Key Vault with automatic reload every 6 hours.
    /// </summary>
    public static IConfigurationBuilder AddAzureKeyVaultWithManagedIdentity(
        this IConfigurationBuilder configuration,
        IWebHostEnvironment environment)
    {
        if (environment.IsEnvironment("LocalDevelopment"))
        {
            return configuration;
        }

        IConfigurationRoot config = configuration.Build();
        string? keyVaultEndpoint = config["KeyVault:Endpoint"];

        if (string.IsNullOrWhiteSpace(keyVaultEndpoint))
        {
            return configuration;
        }

        configuration.AddAzureKeyVault(
            new Uri(keyVaultEndpoint),
            new DefaultAzureCredential(),
            new AzureKeyVaultConfigurationOptions
            {
                ReloadInterval = TimeSpan.FromHours(6)
            });

        return configuration;
    }
}
