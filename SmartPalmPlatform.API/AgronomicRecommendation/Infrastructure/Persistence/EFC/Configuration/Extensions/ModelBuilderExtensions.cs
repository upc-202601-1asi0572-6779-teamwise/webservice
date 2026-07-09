using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAgronomicRecommendationConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Recommendation>().ToTable("recommendations");
        builder.Entity<Recommendation>().HasKey(recommendation => recommendation.Id);
        builder.Entity<Recommendation>().Property(recommendation => recommendation.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Recommendation>().Property(recommendation => recommendation.PlantationId).IsRequired();
        builder.Entity<Recommendation>().Property(recommendation => recommendation.AgronomistId).IsRequired();
        builder.Entity<Recommendation>().Property(recommendation => recommendation.Content).IsRequired().HasMaxLength(500);
        builder.Entity<Recommendation>().Property(recommendation => recommendation.Type).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<Recommendation>().Property(recommendation => recommendation.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<Recommendation>().Property(recommendation => recommendation.CreatedAt).IsRequired();
        builder.Entity<Recommendation>().Property(recommendation => recommendation.ApprovedAt);
        builder.Entity<Recommendation>().Property(recommendation => recommendation.PublishedAt);
    }
}
