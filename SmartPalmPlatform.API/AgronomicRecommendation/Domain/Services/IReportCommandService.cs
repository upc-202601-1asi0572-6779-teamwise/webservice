using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

public interface IReportCommandService
{
    Task<Report> Handle(CreateReportCommand command);
    Task<Report> Handle(UpdateReportContentCommand command);
    Task<Report> Handle(PublishReportCommand command);
}