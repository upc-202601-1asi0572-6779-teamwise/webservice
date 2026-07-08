using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class AgronomicInterventionResourceFromEntityAssembler
{
    public static AgronomicInterventionResource ToResourceFromEntity(
        AgronomicIntervention intervention
    )
    {
        return new AgronomicInterventionResource(
            intervention.Description,
            intervention.PerformedBy,
            intervention.ExecutionDate,
            intervention.CreatedAt
        );
    }
}