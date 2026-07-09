using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.FieldTechnicalManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Infrastructure.Persistence.EFC.Configuration;

public class FieldTechnicalManagementDbContext(DbContextOptions<FieldTechnicalManagementDbContext> options) : DbContext(options)
{
    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplyFieldTechnicalManagementConfiguration();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }
}
