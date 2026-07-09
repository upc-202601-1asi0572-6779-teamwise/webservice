using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.FieldTechnicalManagement.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.SensorDataProcessing.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (!builder.Options.Extensions.Any(e => e.GetType().Name.Contains("DesignTime")))
        {
            builder.AddCreatedUpdatedInterceptor();
        }
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply each BC's model configuration
        IamDbContext.ConfigureModel(builder);
        CropMonitoringDbContext.ConfigureModel(builder);
        IotDeviceManagementDbContext.ConfigureModel(builder);
        SensorDataProcessingDbContext.ConfigureModel(builder);
        AgronomicRecommendationDbContext.ConfigureModel(builder);
        AlertsAndNotificationsDbContext.ConfigureModel(builder);
        FieldTechnicalManagementDbContext.ConfigureModel(builder);

        builder.UseSnakeCaseNamingConvention();
    }
}
