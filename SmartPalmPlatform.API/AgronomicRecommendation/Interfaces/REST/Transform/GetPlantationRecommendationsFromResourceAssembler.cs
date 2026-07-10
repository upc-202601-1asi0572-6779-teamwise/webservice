using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class GetSectorRecommendationsFromResourceAssembler
{
    public static GetSectorRecommendationsQuery ToQueryFromResource(
        int sectorId,
        string? status,
        int? agronomistId = null
    )
    {
        return new GetSectorRecommendationsQuery(
            sectorId,
            status is null ? null : StatusFromString(status),
            agronomistId
        );
    }

    public static GetGeneralRecommendationsQuery ToGeneralQueryFromResource(
        string? status,
        int? agronomistId = null
    )
    {
        return new GetGeneralRecommendationsQuery(
            status is null ? null : StatusFromString(status),
            agronomistId
        );
    }

    private static RecommendationStatus StatusFromString(string status)
    {
        return status switch
        {
            "pending" => RecommendationStatus.Pending,
            "approved" => RecommendationStatus.Approved,
            "published" => RecommendationStatus.Published,
            _ => throw new ArgumentException($"Invalid status '{status}'. Valid values: pending, approved, published."),
        };
    }
}
