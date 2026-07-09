using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;

namespace SmartPalmPlatform.API.SensorDataProcessing.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySensorDataProcessingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<SensorReading>().ToTable("sensor_reading");
        builder.Entity<SensorReading>().HasKey(reading => reading.Id);
        builder.Entity<SensorReading>().Property(reading => reading.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SensorReading>().Property(reading => reading.Type).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.Value).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.MeasuredAt).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.IotDeviceMacAddress).IsRequired();
        builder.Entity<SensorReading>().HasIndex(reading => new { reading.IotDeviceMacAddress, reading.MeasuredAt });

        builder.Entity<AgronomicThreshold>().ToTable("agronomic_thresholds");
        builder.Entity<AgronomicThreshold>().HasKey(threshold => threshold.Id);
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.EdgeDeviceMacAddress).IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.IotDeviceMacAddress).IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Min).IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Max).IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Description).IsRequired();
        builder.Entity<AgronomicThreshold>().Property(threshold => threshold.Type).IsRequired();
    }
}
