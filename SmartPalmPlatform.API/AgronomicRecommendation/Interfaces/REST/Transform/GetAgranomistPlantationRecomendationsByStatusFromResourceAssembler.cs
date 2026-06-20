using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class GetAgranomistPlantationRecomendationsByStatusFromResourceAssembler
{
    public static GetAgronomistPlantationRecommendationsByStatusQuery ToQueryFromResource(
        int agronomistId,
        int plantationId,
        string? status
    )
    {
        return new GetAgronomistPlantationRecommendationsByStatusQuery(
            agronomistId,
            plantationId,
            status is null ? null : StringFromStatus(status)
        );
    }

    private static RecommendationStatus StringFromStatus(string status)
    {
        return status switch
        {
            "pending" => RecommendationStatus.Pending,
            "published" => RecommendationStatus.Published,
            "approved" => RecommendationStatus.Approved,
            _ => throw new Exception("Status not found, only [pending, published, approved]"),
        };
    }
}
