using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.ACL;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Commands;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Repositories;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Services;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Application.Internal.CommandServices;

public class AgronomicInterventionCommandService(
    IUnitOfWork unitOfWork,
    IAgronomicInterventionRepository interventionRepository,
    ICropMonitoringFacade cropMonitoringFacade,
    IRecommendationFacade recommendationFacade
) : IAgronomicInterventionCommandService
{
    public async Task<AgronomicIntervention> Handle(RegisterAgronomicInterventionCommand command)
    {
        if (!await cropMonitoringFacade.SectorIsActiveAsync(command.SectorId))
            throw new ArgumentException($"Sector with id {command.SectorId} does not exist or is not active.");

        if (command.OriginRecommendationId.HasValue
            && !await recommendationFacade.RecommendationExistsAndIsPublishedAsync(
                command.OriginRecommendationId.Value
            ))
            throw new ArgumentException(
                $"Recommendation with id {command.OriginRecommendationId.Value} not found or not published."
            );

        var intervention = new AgronomicIntervention(
            command.OriginRecommendationId,
            command.SectorId,
            command.Description,
            command.PerformedBy,
            command.ExecutionDate
        );

        await interventionRepository.AddAsync(intervention);
        await unitOfWork.CompleteAsync();

        return intervention;
    }
}
