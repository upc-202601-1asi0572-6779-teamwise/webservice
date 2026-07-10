using MediatR;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Events;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;
using SmartPalmPlatform.API.IAM.Interfaces.ACL;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.CommandServices;

public class RecommendationCommandService(
    IUnitOfWork unitOfWork,
    IRecommendationRepository recommendationRepository,
    ICropMonitoringFacade cropMonitoringFacade,
    IIamContextFacade iamContextFacade,
    IMediator mediator
) : IRecommendationCommandService
{
    public async Task<Recommendation> Handle(CreateRecommendationCommand command)
    {
        if (!await iamContextFacade.UserExistsByIdAsync(command.AgronomistId))
            throw new ArgumentException($"Agronomist with id {command.AgronomistId} does not exist.");

        if (!await iamContextFacade.HasActiveSubscriptionAsync(command.AgronomistId))
            throw new InvalidOperationException("User does not have an active subscription.");

        if (command.Type == RecommendationType.SectorSpecific)
        {
            if (command.SectorId is null)
                throw new ArgumentException("SectorId is required for sector-specific recommendations.");

            if (!await cropMonitoringFacade.SectorExistsAsync(command.SectorId.Value))
                throw new ArgumentException($"Sector with id {command.SectorId} does not exist.");
        }

        var recommendation = new Recommendation(
            command.SectorId,
            command.AgronomistId,
            command.Content,
            command.Type,
            command.ReportId
        );

        await recommendationRepository.AddAsync(recommendation);
        await unitOfWork.CompleteAsync();

        if (command.Type == RecommendationType.SectorSpecific && command.SectorId.HasValue)
        {
            await mediator.Publish(new SectorSpecificRecommendationCreatedEvent(
                recommendation.Id, command.AgronomistId, command.SectorId.Value));
        }

        return recommendation;
    }

    public async Task<Recommendation> Handle(UpdateRecommendationContentCommand command)
    {
        var recommendation = await recommendationRepository.FindByIdAsync(command.RecommendationId);

        if (recommendation is null)
            throw new KeyNotFoundException("Recommendation not found.");

        if (!await iamContextFacade.UserExistsByIdAsync(recommendation.AgronomistId))
            throw new ArgumentException($"Agronomist with id {recommendation.AgronomistId} does not exist.");

        if (!await iamContextFacade.HasActiveSubscriptionAsync(recommendation.AgronomistId))
            throw new InvalidOperationException("User does not have an active subscription.");

        recommendation.UpdateContent(command.Content);

        recommendationRepository.Update(recommendation);
        await unitOfWork.CompleteAsync();

        return recommendation;
    }

    public async Task<Recommendation> Handle(ApproveRecommendationCommand command)
    {
        var recommendation = await recommendationRepository.FindByIdAsync(command.RecommendationId);

        if (recommendation is null)
            throw new KeyNotFoundException("Recommendation not found.");

        if (!await iamContextFacade.UserExistsByIdAsync(recommendation.AgronomistId))
            throw new ArgumentException($"Agronomist with id {recommendation.AgronomistId} does not exist.");

        if (!await iamContextFacade.HasActiveSubscriptionAsync(recommendation.AgronomistId))
            throw new InvalidOperationException("User does not have an active subscription.");

        recommendation.Approve();

        recommendationRepository.Update(recommendation);
        await unitOfWork.CompleteAsync();

        return recommendation;
    }

    public async Task<Recommendation> Handle(PublishRecommendationCommand command)
    {
        var recommendation = await recommendationRepository.FindByIdAsync(command.RecommendationId);

        if (recommendation is null)
            throw new KeyNotFoundException("Recommendation not found.");

        if (!await iamContextFacade.UserExistsByIdAsync(recommendation.AgronomistId))
            throw new ArgumentException($"Agronomist with id {recommendation.AgronomistId} does not exist.");

        if (!await iamContextFacade.HasActiveSubscriptionAsync(recommendation.AgronomistId))
            throw new InvalidOperationException("User does not have an active subscription.");

        recommendation.Publish();

        recommendationRepository.Update(recommendation);
        await unitOfWork.CompleteAsync();

        await mediator.Publish(new RecommendationPublishedEvent(recommendation.Id));

        return recommendation;
    }
}

