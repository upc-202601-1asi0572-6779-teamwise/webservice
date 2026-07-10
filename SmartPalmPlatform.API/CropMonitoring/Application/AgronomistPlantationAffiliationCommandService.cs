using SmartPalmPlatform.API.CropMonitoring.Domain.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services;

namespace SmartPalmPlatform.API.CropMonitoring.Application;

public class AgronomistPlantationAffiliationCommandService(
    IAgronomistPlantationAffiliationRepository affiliationRepository,
    IPlantationRepository plantationRepository
) : IAgronomistPlantationAffiliationCommandService
{
    public async Task<AgronomistPlantationAffiliation> Handle(CreateAgronomistPlantationAffiliationCommand command)
    {
        var plantationExists = await plantationRepository.FindByIdAsync(command.PlantationId);
        if (plantationExists is null)
            throw new KeyNotFoundException($"Plantation '{command.PlantationId}' not found.");

        var existing = await affiliationRepository.FindByAgronomistAndPlantationAsync(
            command.AgronomistId, command.PlantationId);
        if (existing is not null)
            return existing;

        var affiliation = new AgronomistPlantationAffiliation(command.AgronomistId, command.PlantationId);
        await affiliationRepository.AddAsync(affiliation);
        return affiliation;
    }

    public async Task Handle(DetachAgronomistPlantationAffiliationCommand command)
    {
        var affiliation = await affiliationRepository.FindByAgronomistAndPlantationAsync(
            command.AgronomistId, command.PlantationId);
        if (affiliation is null)
            throw new KeyNotFoundException(
                $"Active affiliation for agronomist '{command.AgronomistId}' and plantation '{command.PlantationId}' not found.");

        affiliation.Detach();
        affiliationRepository.Update(affiliation);
    }
}