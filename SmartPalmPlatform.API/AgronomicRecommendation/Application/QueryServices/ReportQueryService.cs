using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.QueryServices;

public class ReportQueryService(IReportRepository reportRepository)
    : IReportQueryService
{
    public async Task<IEnumerable<Report>> Handle(GetSectorReportsQuery query)
    {
        return await reportRepository.FindBySectorIdAsync(query.SectorId);
    }

    public async Task<Report?> Handle(GetReportByIdQuery query)
    {
        return await reportRepository.FindByIdAsync(query.ReportId);
    }
}