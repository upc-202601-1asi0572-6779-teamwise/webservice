using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

public interface IRecommendationQueryService
{
    Task<IEnumerable<Recommendation>> Handle(GetPlantationRecommendationsQuery query);

    Task<Recommendation?> Handle(GetRecommendationByIdQuery query);
}

