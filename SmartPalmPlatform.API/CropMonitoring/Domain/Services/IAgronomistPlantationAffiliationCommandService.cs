using SmartPalmPlatform.API.CropMonitoring.Domain.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Services;

public interface IAgronomistPlantationAffiliationCommandService
{
    Task<AgronomistPlantationAffiliation> Handle(CreateAgronomistPlantationAffiliationCommand command);
    Task Handle(DetachAgronomistPlantationAffiliationCommand command);
}