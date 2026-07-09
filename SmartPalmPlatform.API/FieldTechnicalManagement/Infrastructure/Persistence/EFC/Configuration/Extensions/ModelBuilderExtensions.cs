using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyFieldTechnicalManagementConfiguration(this ModelBuilder builder)
    {
        ConfigureAgronomicIntervention(builder);
    }

    private static void ConfigureAgronomicIntervention(ModelBuilder builder)
    {
        builder.Entity<AgronomicIntervention>().ToTable("agronomic_interventions");
        builder.Entity<AgronomicIntervention>().HasKey(i => i.Id);
        builder.Entity<AgronomicIntervention>().Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AgronomicIntervention>().Property(i => i.RecommendationId);
        builder.Entity<AgronomicIntervention>().Property(i => i.SectorId).IsRequired();
        builder.Entity<AgronomicIntervention>().Property(i => i.Description).IsRequired().HasMaxLength(500);
        builder.Entity<AgronomicIntervention>().Property(i => i.PerformedBy).IsRequired().HasMaxLength(100);
        builder.Entity<AgronomicIntervention>().Property(i => i.ExecutionDate).IsRequired();
        builder.Entity<AgronomicIntervention>().Property(i => i.CreatedAt).IsRequired();
    }
}
