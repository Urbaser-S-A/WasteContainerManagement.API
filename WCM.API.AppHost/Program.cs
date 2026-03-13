var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(8080)
                .WithForwardedHeaders(enabled: true);
    });

var k8s = builder.AddKubernetesEnvironment("k8s")
    .WithProperties(k8s =>
    {
        k8s.HelmChartName = "wcm-api";
    });

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("wcm-postgres-data")
    .WithPgAdmin()
    .WithComputeEnvironment(compose);

var wcmDatabase = postgres.AddDatabase("wcmdb");

var apiService = builder.AddProject<Projects.WCM_API_ApiService>("apiservice")
    .WithReference(wcmDatabase)
    .WaitFor(wcmDatabase)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "LocalDevelopment")
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "api";
    })
    .WithComputeEnvironment(compose);

//  temporary setup adding custom Container image
// var container = builder.ExecutionContext.IsRunMode
//     ? builder.AddDockerfile(
//           "wcm-api", "WCM.API.ApiService", "Containerfile.debug")
//     : builder.AddDockerfile(
//           "wcm-api", "WCM.API.ApiService", "Containerfile.release")
//     .WithComputeEnvironment(compose);

builder.Build().Run();
