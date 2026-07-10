using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;

public class Report
{
    public int Id { get; private set; }
    public int AgronomistId { get; private set; }
    public int SectorId { get; private set; }
    public int? InterventionId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string? Findings { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    public Report(int agronomistId, int sectorId, string title, string content, int? interventionId = null, string? findings = null)
    {
        if (agronomistId <= 0)
            throw new ArgumentException("AgronomistId must be greater than zero.");

        if (sectorId <= 0)
            throw new ArgumentException("SectorId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.");

        AgronomistId = agronomistId;
        SectorId = sectorId;
        InterventionId = interventionId;
        Title = title;
        Content = content;
        Findings = findings;
        Status = ReportStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Report() 
    { 
        Title = string.Empty;
        Content = string.Empty;
    }

    public void UpdateContent(string title, string content, string? findings)
    {
        if (Status == ReportStatus.Published)
            throw new InvalidOperationException("A published report cannot be updated.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.");

        Title = title;
        Content = content;
        Findings = findings;
    }

    public void Publish()
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Only draft reports can be published.");

        Status = ReportStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }
}