using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;

public class Recommendation
{
    public int Id { get; private set; }
    public int? SectorId { get; private set; }
    public int? ReportId { get; private set; }
    public int AgronomistId { get; private set; }
    public string Content { get; private set; }
    public RecommendationType Type { get; private set; }
    public RecommendationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    public Recommendation(int? sectorId, int agronomistId, string content, RecommendationType type = RecommendationType.SectorSpecific, int? reportId = null)
    {
        if (agronomistId <= 0)
            throw new ArgumentException("AgronomistId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Recommendation content cannot be empty.");

        SectorId = sectorId;
        ReportId = reportId;
        AgronomistId = agronomistId;
        Content = content;
        Type = type;
        Status = RecommendationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Recommendation()
    {
        Content = string.Empty;
        Type = RecommendationType.SectorSpecific;
        Status = RecommendationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Recommendation content cannot be empty.");

        if (Status == RecommendationStatus.Published)
            throw new InvalidOperationException("A published recommendation cannot be updated.");

        Content = newContent;
    }

    public void Approve()
    {
        if (Status != RecommendationStatus.Pending)
            throw new InvalidOperationException("Only pending recommendations can be approved.");

        Status = RecommendationStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status != RecommendationStatus.Approved)
            throw new InvalidOperationException("Only approved recommendations can be published.");

        Status = RecommendationStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }
}