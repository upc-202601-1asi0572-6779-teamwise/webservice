using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.CommandServices;

public class RecommendationCommandService(
    IUnitOfWork unitOfWork,
    IRecommendationRepository recommendationRepository
) : IRecommendationCommandService
{
    public async Task<Recommendation> Handle(CreateRecommendationCommand command)
    {
        var recommendation = new Recommendation(
            command.PlantationId,
            command.AgronomistId,
            command.Content
        );

        await recommendationRepository.AddAsync(recommendation);
        await unitOfWork.CompleteAsync();

        return recommendation;
    }

    public async Task<Recommendation> Handle(UpdateRecommendationContentCommand command)
    {
        var recommendation = await recommendationRepository.FindByIdAsync(command.RecommendationId);

        if (recommendation is null)
            throw new Exception("Recommendation not found.");

        recommendation.UpdateContent(command.Content);

        recommendationRepository.Update(recommendation);
        await unitOfWork.CompleteAsync();

        return recommendation;
    }

    public async Task<Recommendation> Handle(ApproveRecommendationCommand command)
    {
        var recommendation = await recommendationRepository.FindByIdAsync(command.RecommendationId);

        if (recommendation is null)
            throw new Exception("Recommendation not found.");

        recommendation.Approve();

        recommendationRepository.Update(recommendation);
        await unitOfWork.CompleteAsync();

        return recommendation;
    }

    public async Task<Recommendation> Handle(PublishRecommendationCommand command)
    {
        var recommendation = await recommendationRepository.FindByIdAsync(command.RecommendationId);

        if (recommendation is null)
            throw new Exception("Recommendation not found.");

        recommendation.Publish();

        recommendationRepository.Update(recommendation);
        await unitOfWork.CompleteAsync();

        return recommendation;
    }

    public async Task<AgronomicIntervention> Handle(
        RegisterAgronomicInterventionCommand command
    )
    {
        var recommendation = await recommendationRepository.FindByIdAsync(command.RecommendationId);

        if (recommendation is null)
            throw new Exception("Recommendation not found.");

        if (recommendation.Status != RecommendationStatus.Published)
            throw new Exception("Only published recommendations can have interventions.");

        var intervention = new AgronomicIntervention(
            command.RecommendationId,
            command.Description,
            command.PerformedBy,
            command.ExecutionDate
        );

        await recommendationRepository.AddInterventionAsync(intervention);
        await unitOfWork.CompleteAsync();

        return intervention;
    }
}