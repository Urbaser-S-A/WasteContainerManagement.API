using Microsoft.EntityFrameworkCore;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.ApiService.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<WasteType> WasteTypes => Set<WasteType>();

    public DbSet<Zone> Zones => Set<Zone>();

    public DbSet<Container> Containers => Set<Container>();

    public DbSet<Incident> Incidents => Set<Incident>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureWasteType(modelBuilder);
        ConfigureZone(modelBuilder);
        ConfigureContainer(modelBuilder);
        ConfigureIncident(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime utcNow = DateTime.UtcNow;

        // Ids and audit timestamps are generated in application code so the model stays
        // provider-agnostic (no database-specific SQL defaults such as gen_random_uuid() / now()).
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                switch (entry.Entity)
                {
                    case WasteType wt:
                        if (wt.Id == Guid.Empty) wt.Id = Guid.CreateVersion7();
                        wt.CreatedAt = utcNow;
                        break;
                    case Zone z:
                        if (z.Id == Guid.Empty) z.Id = Guid.CreateVersion7();
                        z.CreatedAt = utcNow;
                        break;
                    case Container c:
                        if (c.Id == Guid.Empty) c.Id = Guid.CreateVersion7();
                        c.CreatedAt = utcNow;
                        break;
                    case Incident i:
                        if (i.Id == Guid.Empty) i.Id = Guid.CreateVersion7();
                        i.CreatedAt = utcNow;
                        i.ReportedAt = utcNow;
                        break;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is WasteType wt) wt.UpdatedAt = utcNow;
                else if (entry.Entity is Zone z) z.UpdatedAt = utcNow;
                else if (entry.Entity is Container c) c.UpdatedAt = utcNow;
                else if (entry.Entity is Incident i) i.UpdatedAt = utcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static void ConfigureWasteType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WasteType>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ColorCode).HasMaxLength(7);

            entity.HasIndex(e => e.Name).IsUnique();
        });
    }

    private static void ConfigureZone(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.District).HasMaxLength(150);
            entity.Property(e => e.City).HasMaxLength(150);

            entity.HasIndex(e => e.Name).IsUnique();
        });
    }

    private static void ConfigureContainer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Container>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(300);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.WasteTypeId);
            entity.HasIndex(e => e.ZoneId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.WasteType)
                .WithMany(wt => wt.Containers)
                .HasForeignKey(e => e.WasteTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Zone)
                .WithMany(z => z.Containers)
                .HasForeignKey(e => e.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureIncident(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasIndex(e => e.ContainerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.ReportedAt);

            entity.HasOne(e => e.Container)
                .WithMany(c => c.Incidents)
                .HasForeignKey(e => e.ContainerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
