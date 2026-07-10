using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class ReportResourceFromEntityAssembler
{
    public static ReportResource ToResourceFromEntity(Report entity)
    {
        return new ReportResource(
            entity.Id,
            entity.AgronomistId,
            entity.SectorId,
            entity.InterventionId,
            entity.Title,
            entity.Content,
            entity.Findings,
            entity.Status.ToString(),
            entity.CreatedAt.ToString("o"),
            entity.PublishedAt?.ToString("o")
        );
    }
}