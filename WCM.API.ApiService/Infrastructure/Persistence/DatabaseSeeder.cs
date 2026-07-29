using Microsoft.EntityFrameworkCore;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Infrastructure.Persistence;

/// <summary>
/// Seeds a small set of idempotent reference data so the API is functional out of the box.
/// Runs at startup after the schema is created; no-op if data already exists.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (await context.WasteTypes.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Database already contains reference data. Skipping seed");
            return;
        }

        logger.LogInformation("Seeding reference data...");

        WasteType organic = new() { Name = "Organic", Description = "Biodegradable food and garden waste", ColorCode = "#6B4226" };
        WasteType paper = new() { Name = "Paper", Description = "Paper and cardboard", ColorCode = "#1E90FF" };
        WasteType plastic = new() { Name = "Plastic", Description = "Plastic packaging and containers", ColorCode = "#FFD700" };
        WasteType glass = new() { Name = "Glass", Description = "Glass bottles and jars", ColorCode = "#2E8B57" };
        context.WasteTypes.AddRange(organic, paper, plastic, glass);

        Zone centro = new() { Name = "Centro", District = "Centro", City = "Madrid" };
        Zone salamanca = new() { Name = "Salamanca", District = "Salamanca", City = "Madrid" };
        context.Zones.AddRange(centro, salamanca);

        // Persist so generated Ids are available for the foreign keys below.
        await context.SaveChangesAsync(cancellationToken);

        Container[] containers =
        [
            new() { Code = "CTN-001-ORG", WasteTypeId = organic.Id, ZoneId = centro.Id, Latitude = 40.4168, Longitude = -3.7038, Address = "Plaza Mayor 1", CapacityLiters = 1100, Status = ContainerStatus.Active },
            new() { Code = "CTN-002-PAP", WasteTypeId = paper.Id, ZoneId = centro.Id, Latitude = 40.4170, Longitude = -3.7040, Address = "Calle Arenal 5", CapacityLiters = 800, Status = ContainerStatus.Active },
            new() { Code = "CTN-003-PLA", WasteTypeId = plastic.Id, ZoneId = salamanca.Id, Latitude = 40.4300, Longitude = -3.6800, Address = "Calle Serrano 20", CapacityLiters = 1100, Status = ContainerStatus.Full },
            new() { Code = "CTN-004-GLA", WasteTypeId = glass.Id, ZoneId = salamanca.Id, Latitude = 40.4310, Longitude = -3.6810, Address = "Calle Goya 10", CapacityLiters = 900, Status = ContainerStatus.Maintenance },
        ];
        context.Containers.AddRange(containers);
        await context.SaveChangesAsync(cancellationToken);

        context.Incidents.Add(new Incident
        {
            ContainerId = containers[2].Id,
            Type = IncidentType.Overflow,
            Description = "Container is overflowing and needs urgent collection",
            Status = IncidentStatus.Open,
            Priority = IncidentPriority.High,
        });
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Reference data seeded: {WasteTypes} waste types, {Zones} zones, {Containers} containers",
            4, 2, containers.Length);
    }
}
