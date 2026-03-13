using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using WCM.API.ApiService.Endpoints;
using WCM.API.ApiService.Infrastructure.Extensions;
using WCM.API.ApiService.Infrastructure.Middleware;
using WCM.API.ApiService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from mounted ConfigMap volume (K8s)
string configPath = Environment.GetEnvironmentVariable("CONFIG_PATH") ?? "";
if (!string.IsNullOrEmpty(configPath) && Directory.Exists(configPath))
{
    builder.Configuration.AddJsonFile(
        Path.Combine(configPath, "appsettings.json"), optional: true, reloadOnChange: true);

    string? environmentName = builder.Environment.EnvironmentName;
    builder.Configuration.AddJsonFile(
        Path.Combine(configPath, $"appsettings.{environmentName}.json"), optional: true, reloadOnChange: true);
}

// Configure Serilog as the logging provider
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithThreadId()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
});

// Load secrets.json if it exists (local development only)
builder.Configuration.AddUserSecrets<Program>(optional: true);

// Load configuration from Azure Key Vault using managed identity
builder.Configuration.AddAzureKeyVaultWithManagedIdentity(builder.Environment);

// .NET Aspire services for observability
builder.AddServiceDefaults();

// Configure application services
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddCustomProblemDetails();
builder.Services.AddCustomApiBehavior();
builder.Services.AddApplicationDbContext(builder.Configuration, builder.Environment);

// Authentication and authorization (per-environment)
builder.Services.AddAuthenticationConfiguration(builder.Configuration, builder.Environment);

// CQRS services (MediatR, FluentValidation, pipeline behaviors)
builder.Services.AddCqrsServices(builder.Configuration);

// Register all application repositories
builder.Services.AddRepositories();

// Rate limiting
builder.Services.AddCustomRateLimiting(builder.Configuration);

// OpenAPI 3.1 with versioning support
builder.Services.AddOpenApiWithVersioning();

// Response compression (Brotli + Gzip)
builder.Services.AddCustomResponseCompression();

// Output caching
builder.Services.AddOutputCaching();

var app = builder.Build();

// Auto-apply database scripts (idempotent - each script checks __EFMigrationsHistory internally)
using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    string scriptsDir = Path.Combine(AppContext.BaseDirectory, "scripts");

    if (Directory.Exists(scriptsDir))
    {
        string[] scriptFiles = Directory.GetFiles(scriptsDir, "*.sql");
        Array.Sort(scriptFiles, StringComparer.Ordinal);

        foreach (string scriptPath in scriptFiles)
        {
            string fileName = Path.GetFileName(scriptPath);
            try
            {
                logger.LogInformation("Executing database script {Script}...", fileName);
                string sql = await File.ReadAllTextAsync(scriptPath);
                await dbContext.Database.ExecuteSqlRawAsync(sql);
                logger.LogInformation("Database script {Script} executed successfully", fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to execute database script {Script}", fileName);
                throw;
            }
        }
    }
    else
    {
        logger.LogWarning("Scripts directory not found at {ScriptsDir}. Skipping automatic database setup", scriptsDir);
    }
}

// OpenAPI and Scalar UI - enabled in LocalDevelopment and AzureDevelopment only
if (app.Environment.IsEnvironment("LocalDevelopment") || app.Environment.IsEnvironment("AzureDevelopment"))
{
    app.MapOpenApi("/openapi/WCM.API_{documentName}.json");
    app.MapOpenApi("/openapi/WCM.API_{documentName}.yaml");

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle($"WCM API Documentation - {app.Environment.EnvironmentName}")
            .WithTheme(ScalarTheme.Kepler)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithSearchHotKey("k")
            .WithOpenApiRoutePattern("/openapi/WCM.API_{documentName}.json");
    });
}

// Middleware pipeline (order per CLAUDE.md section 16)

// 1. Security headers
app.UseMiddleware<SecurityHeadersMiddleware>();

// 2. Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    options.GetLevel = (httpContext, elapsed, ex) => ex != null
        ? LogEventLevel.Error
        : httpContext.Response.StatusCode > 499
            ? LogEventLevel.Error
            : LogEventLevel.Information;

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
    };
});

// 3. Exception handler
app.UseExceptionHandler();

// 4. Status code pages
app.UseStatusCodePages();

// 5. HTTPS redirection
app.UseHttpsRedirection();

// 6. Response compression
app.UseResponseCompression();

// 7. Rate limiter
app.UseRateLimiter();

// 9. Authentication
app.UseAuthentication();

// 10. Authorization
app.UseAuthorization();

// 12. Output cache
app.UseOutputCache();

// 14. Health check endpoints (Aspire)
app.MapDefaultEndpoints();

// 15. API endpoints (Minimal APIs)
app.MapWasteTypesEndpoints();
app.MapZonesEndpoints();
app.MapContainersEndpoints();
app.MapIncidentsEndpoints();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class public so test projects can access it
public partial class Program { }
