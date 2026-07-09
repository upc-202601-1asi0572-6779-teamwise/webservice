using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Configuration;

public class IamDbContext(DbContextOptions<IamDbContext> options) : DbContext(options)
{
    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplyIamConfiguration();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }
}
