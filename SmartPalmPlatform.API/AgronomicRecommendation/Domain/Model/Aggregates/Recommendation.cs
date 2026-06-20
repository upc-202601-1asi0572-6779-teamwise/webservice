using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;

public class Recommendation
{
    public int Id { get; private set; }

    public int PlantationId { get; private set; }

    public int AgronomistId { get; private set; }

    public string Content { get; private set; }

    public RecommendationType Type { get; private set; }

    public RecommendationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    public DateTime? PublishedAt { get; private set; }

    public Recommendation(int plantationId, int agronomistId, string content)
    {
        if (plantationId <= 0)
            throw new ArgumentException("PlantationId must be greater than zero.");

        if (agronomistId <= 0)
            throw new ArgumentException("AgronomistId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Recommendation content cannot be empty.");

        PlantationId = plantationId;
        AgronomistId = agronomistId;
        Content = content;
        Type = RecommendationType.Manual;
        Status = RecommendationStatus.Pending;
        CreatedAt = DateTime.Now;
    }

    public Recommendation()
    {
        Content = string.Empty;
        Type = RecommendationType.Manual;
        Status = RecommendationStatus.Pending;
        CreatedAt = DateTime.Now;
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
        ApprovedAt = DateTime.Now;
    }

    public void Publish()
    {
        if (Status != RecommendationStatus.Approved)
            throw new InvalidOperationException("Only approved recommendations can be published.");

        Status = RecommendationStatus.Published;
        PublishedAt = DateTime.Now;
    }
}