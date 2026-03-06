var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("wcm-postgres-data")
    .WithPgAdmin();

var wcmDatabase = postgres.AddDatabase("wcmdb");

var apiService = builder.AddProject<Projects.WCM_API_ApiService>("apiservice")
    .WithReference(wcmDatabase)
    .WaitFor(wcmDatabase);

builder.Build().Run();
