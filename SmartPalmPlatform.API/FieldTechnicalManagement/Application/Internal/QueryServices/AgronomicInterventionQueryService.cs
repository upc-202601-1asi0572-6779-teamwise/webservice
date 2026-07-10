using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Queries;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Repositories;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Services;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Application.Internal.QueryServices;

public class AgronomicInterventionQueryService(
    IAgronomicInterventionRepository interventionRepository
) : IAgronomicInterventionQueryService
{
    public async Task<AgronomicIntervention?> Handle(GetAgronomicInterventionByIdQuery query)
    {
        return await interventionRepository.FindByIdAsync(query.InterventionId);
    }

    public async Task<IEnumerable<AgronomicIntervention>> Handle(GetAgronomicInterventionsBySectorIdQuery query)
    {
        return await interventionRepository.FindBySectorIdAsync(query.SectorId);
    }
}
