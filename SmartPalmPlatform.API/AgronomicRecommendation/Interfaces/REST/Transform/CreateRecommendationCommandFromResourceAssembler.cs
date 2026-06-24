using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class CreateRecommendationCommandFromResourceAssembler
{
    public static CreateRecommendationCommand ToCommandFromResource(
        int plantationId,
        CreateRecommendationResource resource
    )
    {
        return new CreateRecommendationCommand(plantationId, resource.agronomistId, resource.content);
    }
}

