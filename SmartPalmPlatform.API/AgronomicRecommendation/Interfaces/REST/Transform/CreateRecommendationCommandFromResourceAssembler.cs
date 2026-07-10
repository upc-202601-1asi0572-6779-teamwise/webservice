using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class CreateRecommendationCommandFromResourceAssembler
{
    public static CreateRecommendationCommand ToCommandFromResource(
        int? sectorId,
        CreateRecommendationResource resource
    )
    {
        var type = sectorId.HasValue ? RecommendationType.SectorSpecific : RecommendationType.General;
        return new CreateRecommendationCommand(sectorId, resource.agronomistId, resource.content, type, resource.reportId);
    }
}

