using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;
using SmartPalmPlatform.API.IAM.Interfaces.ACL;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.CommandServices;

public class ReportCommandService(
    IUnitOfWork unitOfWork,
    IReportRepository reportRepository,
    ICropMonitoringFacade cropMonitoringFacade,
    IIamContextFacade iamContextFacade
) : IReportCommandService
{
    public async Task<Report> Handle(CreateReportCommand command)
    {
        if (!await iamContextFacade.UserExistsByIdAsync(command.AgronomistId))
            throw new ArgumentException($"Agronomist with id {command.AgronomistId} does not exist.");

        if (!await iamContextFacade.HasActiveSubscriptionAsync(command.AgronomistId))
            throw new InvalidOperationException("User does not have an active subscription.");

        if (!await cropMonitoringFacade.SectorExistsAsync(command.SectorId))
            throw new ArgumentException($"Sector with id {command.SectorId} does not exist.");

        var report = new Report(
            command.AgronomistId,
            command.SectorId,
            command.Title,
            command.Content,
            command.InterventionId,
            command.Findings
        );

        await reportRepository.AddAsync(report);
        await unitOfWork.CompleteAsync();

        return report;
    }

    public async Task<Report> Handle(UpdateReportContentCommand command)
    {
        var report = await reportRepository.FindByIdAsync(command.ReportId);

        if (report is null)
            throw new KeyNotFoundException("Report not found.");

        if (!await iamContextFacade.UserExistsByIdAsync(report.AgronomistId))
            throw new ArgumentException($"Agronomist with id {report.AgronomistId} does not exist.");

        report.UpdateContent(command.Title, command.Content, command.Findings);

        reportRepository.Update(report);
        await unitOfWork.CompleteAsync();

        return report;
    }

    public async Task<Report> Handle(PublishReportCommand command)
    {
        var report = await reportRepository.FindByIdAsync(command.ReportId);

        if (report is null)
            throw new KeyNotFoundException("Report not found.");

        if (!await iamContextFacade.UserExistsByIdAsync(report.AgronomistId))
            throw new ArgumentException($"Agronomist with id {report.AgronomistId} does not exist.");

        report.Publish();

        reportRepository.Update(report);
        await unitOfWork.CompleteAsync();

        return report;
    }
}