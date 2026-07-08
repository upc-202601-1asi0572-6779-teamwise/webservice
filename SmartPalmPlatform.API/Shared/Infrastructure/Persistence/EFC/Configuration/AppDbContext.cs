using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Configuration.Extensions;
using SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Configuration.Extensions;
using SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Configuration.Extensions;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Application database context
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Solo agregar el interceptor si NO es tiempo de diseño (migraciones)
        if (!builder.Options.Extensions.Any(e => e.GetType().Name.Contains("DesignTime")))
        {
            builder.AddCreatedUpdatedInterceptor();
        }
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Iot Device Management Bounded Context Configuration

        builder.Entity<EdgeDevice>().ToTable("edge_device");
        builder.Entity<EdgeDevice>().HasKey(device => device.Id);
        builder
            .Entity<EdgeDevice>()
            .Property(device => device.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Entity<EdgeDevice>().HasIndex(device => device.MacAddress).IsUnique();
        builder.Entity<EdgeDevice>().Property(device => device.MacAddress).IsRequired();
        builder.Entity<EdgeDevice>().Property(device => device.MonitoringZoneId).IsRequired();
        builder
            .Entity<EdgeDevice>()
            .Property(device => device.LastConnectivityCheckAt)
            .IsRequired();
        builder.Entity<EdgeDevice>().Property(device => device.LastSyncAt).IsRequired();
        builder.Entity<EdgeDevice>().Property(device => device.CreatedAt).IsRequired();

        builder.Entity<IotDevice>().ToTable("iot_device");
        builder.Entity<IotDevice>().HasKey(device => device.Id);
        builder
            .Entity<IotDevice>()
            .Property(device => device.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Entity<IotDevice>().HasIndex(device => device.MacAddress).IsUnique();
        builder.Entity<IotDevice>().Property(device => device.MacAddress).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.EdgeDeviceMacAddress).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.CreatedAt).IsRequired();

        builder.Entity<SensorReading>().ToTable("sensor_reading");
        builder.Entity<SensorReading>().HasKey(reading => reading.Id);
        builder
            .Entity<SensorReading>()
            .Property(reading => reading.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<SensorReading>().Property(reading => reading.Type).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.Value).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.MeasuredAt).IsRequired();
        builder
            .Entity<SensorReading>()
            .Property(reading => reading.IotDeviceMacAddress)
            .IsRequired();
        builder
            .Entity<SensorReading>()
            .HasIndex(reading => new { reading.IotDeviceMacAddress, reading.MeasuredAt });

        builder.Entity<EdgeRegistry>().ToTable("edge_registry");
        builder.Entity<EdgeRegistry>().HasKey(registry => registry.Id);
        builder
            .Entity<EdgeRegistry>()
            .Property(registry => registry.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<EdgeRegistry>().Property(registry => registry.EdgeMacAddress).IsRequired();
        builder
            .Entity<EdgeRegistry>()
            .Property(registry => registry.IotDeviceMacAddresses)
            .IsRequired();
        builder.Entity<EdgeRegistry>().Property(registry => registry.CreatedAt).IsRequired();

        builder.Entity<AgronomicThreshold>().ToTable("agronomic_thresholds");
        builder.Entity<AgronomicThreshold>().HasKey(threshold => threshold.Id);
        builder
            .Entity<AgronomicThreshold>()
            .Property(threshold => threshold.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder
            .Entity<AgronomicThreshold>()
            .Property(threshold => threshold.EdgeDeviceMacAddress)
            .IsRequired();
        builder
            .Entity<AgronomicThreshold>()
            .Property(threshold => threshold.IotDeviceMacAddress)
            .IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Min).IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Max).IsRequired();
        builder
            .Entity<AgronomicThreshold>()
            .Property(threshold => threshold.Description)
            .IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Type).IsRequired();

        // Apply IAM context configuration
        builder.ApplyIamConfiguration();

        // Apply Crop Monitoring context configuration
        builder.ApplyCropMonitoringConfiguration();

        // Apply Notifications context configuration
        builder.ApplyNotificationsConfiguration();

        // Agronomic Recommendation Bounded Context Configuration
        builder.Entity<Recommendation>().ToTable("recommendations");
        builder.Entity<Recommendation>().HasKey(recommendation => recommendation.Id);
        builder
            .Entity<Recommendation>()
            .Property(recommendation => recommendation.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder
            .Entity<Recommendation>()
            .Property(recommendation => recommendation.PlantationId)
            .IsRequired();
        builder
            .Entity<Recommendation>()
            .Property(recommendation => recommendation.AgronomistId)
            .IsRequired();

        builder
            .Entity<Recommendation>()
            .Property(recommendation => recommendation.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .Entity<Recommendation>()
            .Property(recommendation => recommendation.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder
            .Entity<Recommendation>()
            .Property(recommendation => recommendation.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder
            .Entity<Recommendation>()
            .Property(recommendation => recommendation.CreatedAt)
            .IsRequired();
        builder.Entity<Recommendation>().Property(recommendation => recommendation.ApprovedAt);
        builder.Entity<Recommendation>().Property(recommendation => recommendation.PublishedAt);

        builder.Entity<AgronomicIntervention>().ToTable("agronomic_interventions");
        builder.Entity<AgronomicIntervention>().HasKey(intervention => intervention.Id);
        builder
            .Entity<AgronomicIntervention>()
            .Property(intervention => intervention.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder
            .Entity<AgronomicIntervention>()
            .Property(intervention => intervention.RecommendationId)
            .IsRequired();

        builder
            .Entity<AgronomicIntervention>()
            .Property(intervention => intervention.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .Entity<AgronomicIntervention>()
            .Property(intervention => intervention.PerformedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Entity<AgronomicIntervention>()
            .Property(intervention => intervention.ExecutionDate)
            .IsRequired();

        builder
            .Entity<AgronomicIntervention>()
            .Property(intervention => intervention.CreatedAt)
            .IsRequired();

        builder
            .Entity<AgronomicIntervention>()
            .HasOne<Recommendation>()
            .WithMany()
            .HasForeignKey(intervention => intervention.RecommendationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Apply snake_case naming convention LAST (only once!)
        builder.UseSnakeCaseNamingConvention();
    }
}
