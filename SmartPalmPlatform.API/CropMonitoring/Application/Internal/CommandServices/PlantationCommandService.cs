using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.CommandServices;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.DomainServices;
using SmartPalmPlatform.API.IAM.Interfaces.ACL;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Application.Internal.CommandServices;

public class PlantationCommandService(
    IUnitOfWork uow,
    IPlantationRepository plantationRepository,
    ISectorRepository sectorRepository,
    IInstallationPlanService installationPlanService,
    IIamContextFacade iamContextFacade
) : IPlantationCommandService
{
    public async Task<Plantation> Handle(CreatePlantationCommand command)
    {
        if (!await iamContextFacade.HasActiveSubscriptionAsync(command.PalmGrowerId))
            throw new InvalidOperationException("User does not have an active subscription.");
        var plan = installationPlanService.CalculatePlan(
            command.Hectares,
            command.CropType.ToString()
        );

        var plantation = new Plantation(
            command.PalmGrowerId,
            command.Name,
            command.Location,
            command.Hectares,
            command.CropType,
            plan
        );

        await plantationRepository.AddAsync(plantation);
        await uow.CompleteAsync();

        return plantation;
    }

    public async Task Handle(AssignSectorCommand command)
    {
        var existing = await sectorRepository.FindByIotDeviceMacAddressAsync(
            command.IotDeviceMacAddress
        );
        if (existing is not null)
            throw new InvalidOperationException(
                $"IoT device '{command.IotDeviceMacAddress}' is already assigned to a sector."
            );

        var plantation = await plantationRepository.FindByIdWithSectorsAsync(
            command.PlantationId
        );
        if (plantation is null)
            throw new KeyNotFoundException($"Plantation '{command.PlantationId}' not found.");

        if (plantation.Status == Domain.Model.Enums.PlantationStatus.Cancelled)
            throw new InvalidOperationException("Cannot assign sectors to a cancelled plantation.");

        var sector = new Sector(
            command.PlantationId,
            command.SectorName,
            command.IotDeviceMacAddress
        );

        await sectorRepository.AddAsync(sector);
        await uow.CompleteAsync();
    }

    public async Task Handle(RemoveSectorCommand command)
    {
        var sector = await sectorRepository.FindByIdAsync(command.SectorId);
        if (sector is null)
            throw new KeyNotFoundException($"Sector '{command.SectorId}' not found.");

        if (sector.PlantationId != command.PlantationId)
            throw new InvalidOperationException("Sector does not belong to this plantation.");

        sectorRepository.Remove(sector);
        await uow.CompleteAsync();
    }

    public async Task<Plantation> Handle(UpdatePlantationDetailsCommand command)
    {
        var plantation = await plantationRepository.FindByIdAsync(command.Id);
        if (plantation is null)
            throw new KeyNotFoundException($"Plantation '{command.Id}' not found.");

        plantation.UpdateDetails(command.Name, command.Location, command.Hectares);
        plantationRepository.Update(plantation);
        await uow.CompleteAsync();

        return plantation;
    }
}
