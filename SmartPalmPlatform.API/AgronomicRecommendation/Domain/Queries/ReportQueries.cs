namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

public record GetSectorReportsQuery(int SectorId);

public record GetReportByIdQuery(int ReportId);

public record GetRecommendationsByReportIdQuery(int ReportId);