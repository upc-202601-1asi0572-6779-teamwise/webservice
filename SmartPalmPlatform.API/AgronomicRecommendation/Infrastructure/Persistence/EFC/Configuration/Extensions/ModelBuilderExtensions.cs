using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAgronomicRecommendationConfiguration(this ModelBuilder builder)
    {
        ConfigureRecommendation(builder);
        ConfigureReport(builder);
    }

    private static void ConfigureRecommendation(ModelBuilder builder)
    {
        builder.Entity<Recommendation>().ToTable("recommendations");
        builder.Entity<Recommendation>().HasKey(r => r.Id);
        builder.Entity<Recommendation>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Recommendation>().Property(r => r.SectorId).IsRequired(false);
        builder.Entity<Recommendation>().Property(r => r.ReportId).IsRequired(false);
        builder.Entity<Recommendation>().Property(r => r.AgronomistId).IsRequired();
        builder.Entity<Recommendation>().Property(r => r.Content).IsRequired().HasMaxLength(500);
        builder.Entity<Recommendation>().Property(r => r.Type).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<Recommendation>().Property(r => r.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<Recommendation>().Property(r => r.CreatedAt).IsRequired();
        builder.Entity<Recommendation>().Property(r => r.ApprovedAt);
        builder.Entity<Recommendation>().Property(r => r.PublishedAt);
    }

    private static void ConfigureReport(ModelBuilder builder)
    {
        builder.Entity<Report>().ToTable("reports");
        builder.Entity<Report>().HasKey(r => r.Id);
        builder.Entity<Report>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Report>().Property(r => r.AgronomistId).IsRequired();
        builder.Entity<Report>().Property(r => r.SectorId).IsRequired();
        builder.Entity<Report>().Property(r => r.InterventionId).IsRequired(false);
        builder.Entity<Report>().Property(r => r.Title).IsRequired().HasMaxLength(200);
        builder.Entity<Report>().Property(r => r.Content).IsRequired().HasMaxLength(2000);
        builder.Entity<Report>().Property(r => r.Findings).IsRequired(false).HasMaxLength(1000);
        builder.Entity<Report>().Property(r => r.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<Report>().Property(r => r.CreatedAt).IsRequired();
        builder.Entity<Report>().Property(r => r.PublishedAt);
    }
}
