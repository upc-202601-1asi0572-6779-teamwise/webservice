using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class RegisterAgronomicInterventionCommandFromResourceAssembler
{
    public static RegisterAgronomicInterventionCommand ToCommandFromResource(
        int agronomistId,
        int plantationId,
        int recommendationId,
        RegisterAgronomicInterventionResource resource
    )
    {
        return new RegisterAgronomicInterventionCommand(
            agronomistId,
            plantationId,
            recommendationId,
            resource.description,
            resource.performedBy,
            resource.executionDate
        );
    }
}

