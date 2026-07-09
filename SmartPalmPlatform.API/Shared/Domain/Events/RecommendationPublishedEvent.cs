using SmartPalmPlatform.API.Shared.Domain.Model.Events;

namespace SmartPalmPlatform.API.Shared.Domain.Events;

public record RecommendationPublishedEvent(int RecommendationId) : IEvent;
