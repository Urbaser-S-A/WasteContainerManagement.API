using Aspire.Hosting.Azure;
using Aspire.Hosting.Docker;
using Aspire.Hosting.Kubernetes;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<DockerComposeEnvironmentResource> compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(8080)
                .WithForwardedHeaders(enabled: true);
    });

IResourceBuilder<KubernetesEnvironmentResource> k8s = builder.AddKubernetesEnvironment("k8s")
    .WithProperties(k8s =>
    {
        k8s.HelmChartName = "wcm-api";
    });

IResourceBuilder<AzureContainerRegistryResource> acr = builder.AddAzureContainerRegistry("acrurbaserocdp897")
        .PublishAsExisting("acrurbaserocdp897", "rg-urbaser-oc-dp-northeurope");

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("wcm-postgres-data")
    .WithPgAdmin(r =>
    {
        r.WithUrlForEndpoint("http", u => u.DisplayText = "PG Admin dashboard");
    })
    .WithComputeEnvironment(compose);

IResourceBuilder<PostgresDatabaseResource> wcmDatabase = postgres.AddDatabase("wcmdb");

IResourceBuilder<ProjectResource> apiServiceBuilder = builder.AddProject<Projects.WCM_API_ApiService>("wcm-api")
    .WithReference(wcmDatabase)
    .WaitFor(wcmDatabase)
    .PublishAsDockerFile()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "LocalDevelopment")
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "wcm-api";
    });

if (builder.ExecutionContext.IsRunMode)
{
    // Otherwise break when publishing Kubernetes manifests
    postgres.WithInitFiles("../scripts");
    // Only expose external HTTP endpoints when running dev mode (due to WSL2 forwarding issues)
    apiServiceBuilder = apiServiceBuilder.WithExternalHttpEndpoints().WithComputeEnvironment(compose);
}
else
{
    apiServiceBuilder = apiServiceBuilder.WithComputeEnvironment(k8s);
}

IResourceBuilder<ContainerResource> container = builder.ExecutionContext.IsRunMode
    ? builder.AddDockerfile(
          "wcm-api-df", "WCM.API.ApiService", "Containerfile.debug")
    : builder.AddDockerfile(
          "wcm-api-df", "WCM.API.ApiService", "Containerfile.release")
    .WithComputeEnvironment(compose);

builder.Build().Run();
