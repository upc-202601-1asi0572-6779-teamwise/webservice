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
        Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] IoT device registered: MAC={notification.IotDeviceMacAddress}, plantationId={notification.PlantationId}");

        var plantation = await plantationRepository.FindByIdWithSectorsAsync(
            notification.PlantationId
        );
        if (plantation is null || plantation.Status == Domain.Model.Enums.PlantationStatus.Cancelled)
        {
            Console.WriteLine($"[WARN] [CropMonitoring] [EventHandler] Plantation {notification.PlantationId} not found or cancelled, skipping.");
            return;
        }

        Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] Plantation '{plantation.Name}' found, checking existing sector...");
        var existing = await sectorRepository.FindByIotDeviceMacAddressAsync(
            notification.IotDeviceMacAddress
        );

        if (existing is not null)
        {
            if (existing.Status == Domain.Model.Enums.SectorStatus.Pending)
            {
                Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] Activating pre-assigned Pending sector #{existing.Id}");
                existing.Activate();
                sectorRepository.Update(existing);
                await uow.CompleteAsync();
                Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] Sector #{existing.Id} activated.");
            }
            else
            {
                Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] Sector #{existing.Id} already active, skipping.");
            }
        }
        else
        {
            Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] Creating new sector for IoT device...");
            var sector = new Sector(
                notification.PlantationId,
                $"Sector-{notification.IotDeviceMacAddress}",
                notification.IotDeviceMacAddress
            );
            sector.Activate();
            await sectorRepository.AddAsync(sector);
            await uow.CompleteAsync();
            Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] New sector #{sector.Id} created and activated.");
        }

        var sectorCount = (await sectorRepository.FindByPlantationIdAsync(notification.PlantationId)).Count;
        Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] Plantation #{notification.PlantationId} now has {sectorCount}/{plantation.InstallationPlan.EstimatedSensors} sectors.");
        if (sectorCount >= plantation.InstallationPlan.EstimatedSensors
            && plantation.Status == Domain.Model.Enums.PlantationStatus.Installing)
        {
            Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] All sensors registered — activating plantation #{plantation.Id}.");
            plantation.Activate();
            plantationRepository.Update(plantation);
            await uow.CompleteAsync();
            Console.WriteLine($"[INFO] [CropMonitoring] [EventHandler] Plantation #{plantation.Id} activated.");
        }
    }
}
