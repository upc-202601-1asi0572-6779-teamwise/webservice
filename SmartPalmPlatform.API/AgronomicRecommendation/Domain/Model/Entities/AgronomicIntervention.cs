namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;

public class AgronomicIntervention
{
    public int Id { get; private set; }

    public int RecommendationId { get; private set; }

    public int PlantationId { get; private set; }

    public int? ZoneId { get; private set; }

    public string Description { get; private set; }

    public string PerformedBy { get; private set; }

    public DateTime ExecutionDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public AgronomicIntervention(
        int recommendationId,
        string description,
        string performedBy,
        DateTime executionDate
    ) : this(recommendationId, 0, null, description, performedBy, executionDate)
    {
    }

    public AgronomicIntervention(
        int recommendationId,
        int plantationId,
        int? zoneId,
        string description,
        string performedBy,
        DateTime executionDate
    )
    {
        if (recommendationId <= 0)
            throw new ArgumentException("RecommendationId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Intervention description cannot be empty.");

        if (string.IsNullOrWhiteSpace(performedBy))
            throw new ArgumentException("PerformedBy cannot be empty.");

        RecommendationId = recommendationId;
        PlantationId = plantationId;
        ZoneId = zoneId;
        Description = description;
        PerformedBy = performedBy;
        ExecutionDate = executionDate;
        CreatedAt = DateTime.Now;
    }

    public AgronomicIntervention()
    {
        Description = string.Empty;
        PerformedBy = string.Empty;
        CreatedAt = DateTime.Now;
    }
}
