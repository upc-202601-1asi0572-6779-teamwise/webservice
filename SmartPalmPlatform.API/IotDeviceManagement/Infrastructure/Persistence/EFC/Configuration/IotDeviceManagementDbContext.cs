using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistence.EFC.Configuration;

public class IotDeviceManagementDbContext(DbContextOptions<IotDeviceManagementDbContext> options) : DbContext(options)
{
    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplyIotDeviceManagementConfiguration();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }
}
