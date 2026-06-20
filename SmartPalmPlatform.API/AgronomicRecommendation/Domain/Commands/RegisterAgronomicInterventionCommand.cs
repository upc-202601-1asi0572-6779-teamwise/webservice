namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record RegisterAgronomicInterventionCommand(
    int AgronomistId,
    int PlantationId,
    int RecommendationId,
    string Description,
    string PerformedBy,
    DateTime ExecutionDate
);

