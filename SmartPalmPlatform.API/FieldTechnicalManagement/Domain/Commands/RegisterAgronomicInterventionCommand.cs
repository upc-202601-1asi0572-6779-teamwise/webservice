namespace SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Commands;

public record RegisterAgronomicInterventionCommand(
    int SectorId,
    string Description,
    string PerformedBy,
    DateTime ExecutionDate,
    int? OriginRecommendationId
);
