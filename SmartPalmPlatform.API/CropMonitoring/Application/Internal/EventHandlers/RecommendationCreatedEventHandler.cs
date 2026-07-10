using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Events;
using SmartPalmPlatform.API.CropMonitoring.Domain.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;

namespace SmartPalmPlatform.API.CropMonitoring.Application.Internal.EventHandlers;

public class RecommendationCreatedEventHandler(
    ICropMonitoringFacade cropMonitoringFacade,
    IAgronomistPlantationAffiliationCommandService affiliationCommandService,
    IAgronomistPlantationAffiliationRepository affiliationRepository
) : IEventHandler<SectorSpecificRecommendationCreatedEvent>
{
    public async Task Handle(
        SectorSpecificRecommendationCreatedEvent notification,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[INFO] [CropMonitoring] [RecommendationCreatedEventHandler] " +
            $"Handling SectorSpecificRecommendationCreatedEvent: " +
            $"RecommendationId={notification.RecommendationId}, " +
            $"AgronomistId={notification.AgronomistId}, " +
            $"SectorId={notification.SectorId}");

        var plantationId = await cropMonitoringFacade.GetSectorPlantationIdAsync(notification.SectorId);
        if (plantationId is null)
        {
            Console.WriteLine(
                $"[WARN] [CropMonitoring] [RecommendationCreatedEventHandler] " +
                $"Sector '{notification.SectorId}' not found, skipping affiliation.");
            return;
        }

        var existing = await affiliationRepository.FindByAgronomistAndPlantationAsync(
            notification.AgronomistId, plantationId.Value);
        if (existing is not null)
        {
            Console.WriteLine(
                $"[INFO] [CropMonitoring] [RecommendationCreatedEventHandler] " +
                $"Affiliation already exists for agronomist '{notification.AgronomistId}' " +
                $"and plantation '{plantationId.Value}', skipping.");
            return;
        }

        var command = new CreateAgronomistPlantationAffiliationCommand(
            notification.AgronomistId, plantationId.Value);
        await affiliationCommandService.Handle(command);

        Console.WriteLine(
            $"[INFO] [CropMonitoring] [RecommendationCreatedEventHandler] " +
            $"Auto-affiliated agronomist '{notification.AgronomistId}' " +
            $"to plantation '{plantationId.Value}'.");
    }
}