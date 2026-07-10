using SmartPalmPlatform.API.Shared.Domain.Model.Events;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Events;

public record SectorSpecificRecommendationCreatedEvent(
    int RecommendationId,
    int AgronomistId,
    int SectorId
) : IEvent;