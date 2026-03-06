var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "WCM API - Waste Container Management");

app.Run();
