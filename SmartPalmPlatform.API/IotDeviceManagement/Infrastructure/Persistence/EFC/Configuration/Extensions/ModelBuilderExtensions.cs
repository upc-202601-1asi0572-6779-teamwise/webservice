using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;

namespace SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIotDeviceManagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<EdgeDevice>().ToTable("edge_device");
        builder.Entity<EdgeDevice>().HasKey(device => device.Id);
        builder.Entity<EdgeDevice>().Property(device => device.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<EdgeDevice>().HasIndex(device => device.MacAddress).IsUnique();
        builder.Entity<EdgeDevice>().Property(device => device.MacAddress).IsRequired();
        builder.Entity<EdgeDevice>().Property(device => device.MonitoringZoneId).IsRequired();
        builder.Entity<EdgeDevice>().Property(device => device.UserId).IsRequired();
        builder.Entity<EdgeDevice>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(device => device.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<EdgeDevice>().Property(device => device.LastConnectivityCheckAt).IsRequired();
        builder.Entity<EdgeDevice>().Property(device => device.LastSyncAt).IsRequired();
        builder.Entity<EdgeDevice>().Property(device => device.CreatedAt).IsRequired();

        builder.Entity<IotDevice>().ToTable("iot_device");
        builder.Entity<IotDevice>().HasKey(device => device.Id);
        builder.Entity<IotDevice>().Property(device => device.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<IotDevice>().HasIndex(device => device.MacAddress).IsUnique();
        builder.Entity<IotDevice>().Property(device => device.MacAddress).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.EdgeDeviceMacAddress).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.UserId).IsRequired();
        builder.Entity<IotDevice>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(device => device.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<IotDevice>().Property(device => device.CreatedAt).IsRequired();

        builder.Entity<EdgeRegistry>().ToTable("edge_registry");
        builder.Entity<EdgeRegistry>().HasKey(registry => registry.Id);
        builder.Entity<EdgeRegistry>().Property(registry => registry.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<EdgeRegistry>().Property(registry => registry.EdgeMacAddress).IsRequired();
        builder.Entity<EdgeRegistry>().Property(registry => registry.IotDeviceMacAddresses).IsRequired();
        builder.Entity<EdgeRegistry>().Property(registry => registry.CreatedAt).IsRequired();
    }
}
