using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Commands;
using SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Transform;

public static class RegisterCommandFromResourceAssembler
{
    public static RegisterAgronomicInterventionCommand ToCommandFromResource(
        RegisterInterventionResource resource
    )
    {
        return new RegisterAgronomicInterventionCommand(
            resource.sectorId,
            resource.description,
            resource.performedBy,
            resource.executionDate,
            resource.originRecommendationId
        );
    }
}
