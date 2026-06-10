using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;

using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;


namespace SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Application database context
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Solo agregar el interceptor si NO es tiempo de diseño (migraciones)
        if (!builder.Options.Extensions.Any(e => e.GetType().Name.Contains("DesignTime")))
        {
            builder.AddCreatedUpdatedInterceptor();
        }
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        
        // Apply IAM context configuration
        //builder.ApplyIamConfiguration();
        
        // Apply snake_case naming convention LAST (only once!)
        builder.UseSnakeCaseNamingConvention();
    }
}