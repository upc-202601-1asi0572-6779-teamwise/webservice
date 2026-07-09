using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyNotificationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(a => a.SensorType).IsRequired();
            entity.Property(a => a.UserId).IsRequired();
            entity.Property(a => a.Message).IsRequired().HasMaxLength(500);
            entity.Property(a => a.Level).IsRequired().HasConversion<string>().HasMaxLength(20);
            entity.Property(a => a.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            entity.Property(a => a.Timestamp).IsRequired();
        });

        builder.Entity<UserAlertSetting>(entity =>
        {
            entity.ToTable("user_alert_settings");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(s => s.UserId).IsRequired();
            entity.Property(s => s.SensorType).IsRequired();
            entity.Property(s => s.IsMuted).IsRequired();
            entity.HasIndex(s => new { s.UserId, s.SensorType }).IsUnique();
        });
    }
}
