using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Enums;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyCropMonitoringConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Plantation>(entity =>
        {
            entity.ToTable("plantations");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(p => p.PalmGrowerId).IsRequired();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.OwnsOne(p => p.Location, loc =>
            {
                loc.Property(l => l.Address).HasColumnName("address").IsRequired().HasMaxLength(500);
                loc.Property(l => l.Coordinates).HasColumnName("coordinates").HasMaxLength(100);
            });
            entity.Property(p => p.Hectares).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(p => p.Status).IsRequired().HasConversion<int>();
            entity.OwnsOne(p => p.InstallationPlan, plan =>
            {
                plan.Property(ip => ip.EstimatedSensors).HasColumnName("estimated_sensors").IsRequired();
                plan.Property(ip => ip.Message).HasColumnName("installation_message").IsRequired().HasMaxLength(500);
            });
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.UpdatedAt);
        });

        builder.Entity<Sector>(entity =>
        {
            entity.ToTable("sectors");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(s => s.PlantationId).IsRequired();
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.IotDeviceMacAddress).IsRequired().HasMaxLength(50);
            entity.HasIndex(s => s.IotDeviceMacAddress).IsUnique();
            entity.Property(s => s.Status).IsRequired().HasConversion<int>();
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.ActivatedAt);

            entity.HasOne<Plantation>()
                .WithMany()
                .HasForeignKey(s => s.PlantationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
