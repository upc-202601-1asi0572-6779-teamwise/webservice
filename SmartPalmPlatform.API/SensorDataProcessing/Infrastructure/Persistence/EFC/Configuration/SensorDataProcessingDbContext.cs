using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.SensorDataProcessing.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.SensorDataProcessing.Infrastructure.Persistence.EFC.Configuration;

public class SensorDataProcessingDbContext(DbContextOptions<SensorDataProcessingDbContext> options) : DbContext(options)
{
    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplySensorDataProcessingConfiguration();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }
}
