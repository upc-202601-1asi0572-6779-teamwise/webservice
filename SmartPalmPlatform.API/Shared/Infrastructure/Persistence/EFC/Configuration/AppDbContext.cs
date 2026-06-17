using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
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
        builder.Entity<IotDevice>().ToTable("iot_device");
        builder.Entity<IotDevice>().HasKey(device => device.Id);
        builder
            .Entity<IotDevice>()
            .Property(device => device.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder
            .Entity<IotDevice>()
            .OwnsOne(
                device => device.Configuration,
                config =>
                {
                    config.Property("IotDeviceId").HasColumnName("Id");
                    config.Property(x => x.MaxOfflineStorageHours);
                    config.Property(x => x.RetryPolicy);
                    config.Property(x => x.TransmissionMode);
                    config.Property(x => x.SamplingIntervalMinutes);
                }
            );
        builder.Entity<IotDevice>().Property(device => device.SerialNumber).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.MonitoringZoneId).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.ActivationStatus).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.HealthStatus).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.CreatedAt).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.LastSyncAt).IsRequired();

        // Apply IAM context configuration
        //builder.ApplyIamConfiguration();

        // Apply snake_case naming convention LAST (only once!)
        builder.UseSnakeCaseNamingConvention();
    }
}
