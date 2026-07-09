using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Application.Internal.EventHandlers;

public class CropMonitoringIotDeviceRegisteredEventHandler(
    IUnitOfWork uow,
    IPlantationRepository plantationRepository,
    ISectorRepository sectorRepository
) : IEventHandler<IotDeviceRegisteredEvent>
{
    public async Task Handle(
        IotDeviceRegisteredEvent notification,
        CancellationToken cancellationToken
    )
    {
        var plantation = await plantationRepository.FindByIdWithSectorsAsync(
            notification.PlantationId
        );
        if (plantation is null || plantation.Status == Domain.Model.Enums.PlantationStatus.Cancelled)
            return;

        var existing = await sectorRepository.FindByIotDeviceMacAddressAsync(
            notification.IotDeviceMacAddress
        );

        if (existing is not null)
        {
            if (existing.Status == Domain.Model.Enums.SectorStatus.Pending)
            {
                existing.Activate();
                sectorRepository.Update(existing);
                await uow.CompleteAsync();
            }
        }
        else
        {
            var sector = new Sector(
                notification.PlantationId,
                $"Sector-{notification.IotDeviceMacAddress}",
                notification.IotDeviceMacAddress
            );
            sector.Activate();
            await sectorRepository.AddAsync(sector);
            await uow.CompleteAsync();
        }

        var sectorCount = (await sectorRepository.FindByPlantationIdAsync(notification.PlantationId)).Count;
        if (sectorCount >= plantation.InstallationPlan.EstimatedSensors
            && plantation.Status == Domain.Model.Enums.PlantationStatus.Installing)
        {
            plantation.Activate();
            plantationRepository.Update(plantation);
            await uow.CompleteAsync();
        }
    }
}
