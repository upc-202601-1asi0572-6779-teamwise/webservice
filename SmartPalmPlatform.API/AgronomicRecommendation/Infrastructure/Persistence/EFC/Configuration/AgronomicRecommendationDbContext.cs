using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Infrastructure.Persistence.EFC.Configuration;

public class AgronomicRecommendationDbContext(DbContextOptions<AgronomicRecommendationDbContext> options) : DbContext(options)
{
    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplyAgronomicRecommendationConfiguration();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }
}
