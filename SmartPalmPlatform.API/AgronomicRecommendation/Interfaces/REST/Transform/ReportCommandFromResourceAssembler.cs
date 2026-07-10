using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class CreateReportCommandFromResourceAssembler
{
    public static CreateReportCommand ToCommandFromResource(int sectorId, CreateReportResource resource)
    {
        return new CreateReportCommand(
            resource.agronomistId,
            sectorId,
            resource.title,
            resource.content,
            resource.interventionId,
            resource.findings
        );
    }
}

public static class UpdateReportContentCommandFromResourceAssembler
{
    public static UpdateReportContentCommand ToCommandFromResource(int reportId, UpdateReportContentResource resource)
    {
        return new UpdateReportContentCommand(
            reportId,
            resource.title,
            resource.content,
            resource.findings
        );
    }
}