namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record RegisterAgronomicInterventionCommand(
    int RecommendationId,
    string Description,
    string PerformedBy,
    DateTime ExecutionDate
);

