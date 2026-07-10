using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Commands;
using SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Transform;

public static class RegisterCommandFromResourceAssembler
{
    public static RegisterAgronomicInterventionCommand ToCommandFromResource(
        int sectorId,
        RegisterInterventionResource resource
    )
    {
        return new RegisterAgronomicInterventionCommand(
            sectorId,
            resource.description,
            resource.performedBy,
            resource.executionDate,
            resource.originRecommendationId
        );
    }
}
