using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Configuration;

public class AlertsAndNotificationsDbContext(DbContextOptions<AlertsAndNotificationsDbContext> options) : DbContext(options)
{
    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplyNotificationsConfiguration();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }
}
