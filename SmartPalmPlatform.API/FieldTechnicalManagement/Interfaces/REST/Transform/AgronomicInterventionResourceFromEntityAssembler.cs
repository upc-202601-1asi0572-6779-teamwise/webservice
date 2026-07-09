using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Transform;

public static class AgronomicInterventionResourceFromEntityAssembler
{
    public static AgronomicInterventionResource ToResourceFromEntity(AgronomicIntervention entity)
    {
        return new AgronomicInterventionResource(
            entity.Id,
            entity.SectorId,
            entity.PerformedBy,
            entity.Description,
            entity.ExecutionDate,
            entity.CreatedAt,
            entity.RecommendationId
        );
    }
}
