using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

public interface IRecommendationCommandService
{
    Task<Recommendation> Handle(CreateRecommendationCommand command);

    Task<Recommendation> Handle(UpdateRecommendationContentCommand command);

    Task<Recommendation> Handle(ApproveRecommendationCommand command);

    Task<Recommendation> Handle(PublishRecommendationCommand command);

    Task<AgronomicIntervention> Handle(RegisterAgronomicInterventionCommand command);
}
