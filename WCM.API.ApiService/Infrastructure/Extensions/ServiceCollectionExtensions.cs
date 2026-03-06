using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WCM.API.ApiService.Application.Behaviors;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Infrastructure.Middleware;
using WCM.API.ApiService.Infrastructure.OpenApi;
using WCM.API.ApiService.Infrastructure.Persistence;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace WCM.API.ApiService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring application services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds API versioning configuration for Minimal APIs.
    /// </summary>
    public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        return services;
    }

    /// <summary>
    /// Configures ProblemDetails with custom settings including traceId.
    /// </summary>
    public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["traceId"] =
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    /// <summary>
    /// Configures ApiBehaviorOptions to customize automatic validation responses.
    /// </summary>
    public static IServiceCollection AddCustomApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Validation.Error",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.HttpContext.Request.Path
                };

                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                problemDetails.Extensions["errorCode"] = "Validation.Error";

                return new BadRequestObjectResult(problemDetails);
            };
        });

        return services;
    }

    /// <summary>
    /// Adds Entity Framework Core DbContext with PostgreSQL, retry policy, pooling, and query optimization.
    /// </summary>
    public static IServiceCollection AddApplicationDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            string? connectionString = configuration.GetConnectionString("wcmdb");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(30);

                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);

                npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            bool isLocalDevelopment = environment.IsEnvironment("LocalDevelopment");

            if (isLocalDevelopment)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    /// <summary>
    /// Registers MediatR, FluentValidation, and CQRS pipeline behaviors.
    /// </summary>
    public static IServiceCollection AddCqrsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();

            string? licenseKey = configuration["MediatR:LicenseKey"];
            if (!string.IsNullOrEmpty(licenseKey))
            {
                cfg.LicenseKey = licenseKey;
            }
        });

        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Registers all application repositories with their interfaces.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWasteTypeRepository, Repositories.WasteTypeRepository>();
        // services.AddScoped<IZoneRepository, ZoneRepository>();
        // services.AddScoped<IContainerRepository, ContainerRepository>();
        // services.AddScoped<IIncidentRepository, IncidentRepository>();

        return services;
    }

    /// <summary>
    /// Configures rate limiting with fixed window limiter and custom rejection response.
    /// </summary>
    public static IServiceCollection AddCustomRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection rateLimitConfig = configuration.GetSection("RateLimiting");
        int permitLimit = rateLimitConfig.GetValue<int>("PermitLimit", 100);
        int windowInSeconds = rateLimitConfig.GetValue<int>("WindowInSeconds", 60);
        int queueLimit = rateLimitConfig.GetValue<int>("QueueLimit", 10);

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.FindFirstValue("oid")
                        ?? httpContext.User.FindFirstValue("appid")
                        ?? httpContext.User.Identity?.Name
                        ?? httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromSeconds(windowInSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = queueLimit
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
                }

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    type = "https://httpstatuses.com/429",
                    title = "RateLimit.Exceeded",
                    status = StatusCodes.Status429TooManyRequests,
                    detail = "Too many requests. Please try again later.",
                    instance = context.HttpContext.Request.Path.ToString(),
                    traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier,
                    errorCode = "RateLimit.Exceeded"
                }, cancellationToken: cancellationToken);
            };
        });

        return services;
    }

    /// <summary>
    /// Configures output caching infrastructure for API responses.
    /// By default, NO caching is applied to ensure data freshness.
    /// </summary>
    public static IServiceCollection AddOutputCaching(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddPolicy("BurstProtection", builder => builder
                .Expire(TimeSpan.FromSeconds(5))
                .SetVaryByQuery("*")
                .Tag("burst"));

            options.AddPolicy("ReferenceData", builder => builder
                .Expire(TimeSpan.FromMinutes(30))
                .SetVaryByQuery("*")
                .Tag("reference"));

            options.AddPolicy("NoCache", builder => builder
                .NoCache());
        });

        return services;
    }

    /// <summary>
    /// Configures OpenAPI 3.1 generation with API versioning support and JWT Bearer authentication.
    /// </summary>
    public static IServiceCollection AddOpenApiWithVersioning(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        string[] knownVersions = ApiVersionConstants.KnownVersions;

        foreach (string version in knownVersions)
        {
            services.AddOpenApi(version, options =>
            {
                options.AddDocumentTransformer<OpenApiInfoTransformer>();
                options.AddDocumentTransformer<SecuritySchemeTransformer>();
                options.AddDocumentTransformer<XmlCommentsTransformer>();

                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Tags = new HashSet<OpenApiTag>
                    {
                        new() { Name = "WasteTypes", Description = "Operations for managing waste types (organic, plastic, glass, paper, etc.)" },
                        new() { Name = "Zones", Description = "Operations for managing collection zones and districts" },
                        new() { Name = "Containers", Description = "Operations for managing waste containers, their locations and status" },
                        new() { Name = "Incidents", Description = "Operations for reporting and managing container incidents" }
                    };
                    return Task.CompletedTask;
                });
            });
        }

        services.AddSingleton<OpenApiInfoTransformer>();
        services.AddSingleton<SecuritySchemeTransformer>();
        services.AddSingleton<XmlCommentsTransformer>();

        return services;
    }

    /// <summary>
    /// Configures HTTP response compression with Brotli and Gzip algorithms.
    /// </summary>
    public static IServiceCollection AddCustomResponseCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();

            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            [
                "application/json",
                "application/problem+json",
                "text/plain",
                "text/json"
            ]);
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        return services;
    }
}
