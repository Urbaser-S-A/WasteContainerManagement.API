var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.WCM_API_ApiService>("apiservice");

builder.Build().Run();
