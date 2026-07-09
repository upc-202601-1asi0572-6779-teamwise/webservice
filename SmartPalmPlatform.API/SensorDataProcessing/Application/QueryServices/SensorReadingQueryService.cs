using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.ACL;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.QueryServices;

public class SensorReadingQueryService(
    ISensorReadingRepository sensorReadingRepository,
    IAgronomicThresholdRepository agronomicThresholdRepository,
    IIotDeviceQueryFacade iotDeviceQueryFacade
) : ISensorReadingQueryService
{
    public async Task<List<SensorReading>> Handle(SensorReadingQuery query)
    {
        return await sensorReadingRepository.FindByEdgeDeviceMacAddressAndMeasureTimeRange(
            query.EdgeDeviceMacAddress,
            query.From,
            query.To,
            query.IotDeviceMacAddress,
            query.Page,
            query.Size
        );
    }

    public async Task<List<SensorReading>> Handle(DeviceSensorReadingQuery query)
    {
        var exists = await agronomicThresholdRepository.ExistsByIotDeviceMacAddress(
            query.IotDeviceMacAddress
        );
        if (!exists)
            throw new KeyNotFoundException(
                $"IoT device '{query.IotDeviceMacAddress}' not found."
            );

        // 404 en vez de 403 si el dispositivo no pertenece al usuario: evita
        // confirmar la existencia de dispositivos ajenos por enumeración de MAC.
        if (query.OwnerUserId.HasValue)
        {
            var ownerUserId = await iotDeviceQueryFacade.GetOwnerUserIdByMacAddress(
                query.IotDeviceMacAddress
            );
            if (ownerUserId != query.OwnerUserId.Value)
                throw new KeyNotFoundException(
                    $"IoT device '{query.IotDeviceMacAddress}' not found."
                );
        }

        return await sensorReadingRepository.FindByIotDeviceMacAddressAndMeasureTimeRange(
            query.IotDeviceMacAddress,
            query.From,
            query.To,
            query.Page,
            query.Size
        );
    }
}
