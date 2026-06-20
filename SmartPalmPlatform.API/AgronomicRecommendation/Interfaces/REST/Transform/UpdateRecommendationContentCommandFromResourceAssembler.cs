using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class UpdateRecommendationContentCommandFromResourceAssembler
{
    public static UpdateRecommendationContentCommand ToCommandFromResource(
        int agronomistId,
        int plantationId,
        int recommendationId,
        UpdateRecommendationContentResource resource
    )
    {
        return new UpdateRecommendationContentCommand(
            agronomistId,
            plantationId,
            recommendationId,
            resource.content
        );
    }
}

