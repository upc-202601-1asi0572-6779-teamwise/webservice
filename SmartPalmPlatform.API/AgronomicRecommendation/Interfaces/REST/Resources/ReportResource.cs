namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record CreateReportResource(
    int agronomistId,
    string title,
    string content,
    int? interventionId,
    string? findings
);

public record UpdateReportContentResource(
    string title,
    string content,
    string? findings
);

public record ReportResource(
    int id,
    int agronomistId,
    int sectorId,
    int? interventionId,
    string title,
    string content,
    string? findings,
    string status,
    string createdAt,
    string? publishedAt
);