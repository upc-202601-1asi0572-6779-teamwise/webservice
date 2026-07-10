using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

public interface IReportQueryService
{
    Task<IEnumerable<Report>> Handle(GetSectorReportsQuery query);
    Task<Report?> Handle(GetReportByIdQuery query);
}