namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record CreateReportCommand(
    int AgronomistId,
    int SectorId,
    string Title,
    string Content,
    int? InterventionId,
    string? Findings
);

public record UpdateReportContentCommand(
    int ReportId,
    string Title,
    string Content,
    string? Findings
);

public record PublishReportCommand(int ReportId);