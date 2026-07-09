using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Configuration;

public class CropMonitoringDbContext(DbContextOptions<CropMonitoringDbContext> options) : DbContext(options)
{
    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplyCropMonitoringConfiguration();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }
}
