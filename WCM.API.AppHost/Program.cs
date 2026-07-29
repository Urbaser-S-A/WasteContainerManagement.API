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

// The API is self-contained: it uses an embedded SQLite database, so no external
// database or cloud dependency is orchestrated here.
IResourceBuilder<ProjectResource> apiServiceBuilder = builder.AddProject<Projects.WCM_API_ApiService>("wcm-api")
    .PublishAsDockerFile()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "LocalDevelopment")
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "wcm-api";
    });

if (builder.ExecutionContext.IsRunMode)
{
    // Only expose external HTTP endpoints when running dev mode (due to WSL2 forwarding issues)
    apiServiceBuilder = apiServiceBuilder.WithExternalHttpEndpoints().WithComputeEnvironment(compose);
}
else
{
    apiServiceBuilder = apiServiceBuilder.WithComputeEnvironment(k8s);
}

builder.Build().Run();
