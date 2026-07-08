using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class ReadDeviceSensorsDataCommandFromResourceAssembly
{
    public static ReadDeviceSensorsDataCommand FromResourceToCommand(
        string edgeMac,
        ReadDeviceSensorsDataResource resource
    )
    {
        if (resource.devices is null || resource.devices.Count == 0)
            throw new ArgumentException("At least one device with readings is required.");

        var readings = resource
            .devices.SelectMany(device =>
            {
                if (string.IsNullOrWhiteSpace(device.deviceMac))
                    throw new ArgumentException("deviceMac is required for every device group.");

                return device.readings.Select(r => new SensorReadingPayload(
                    device.deviceMac,
                    SensorTypeFromStringAssembler.FromStringToSensorType(r.sensorType),
                    r.measuredAt,
                    r.value
                ));
            })
            .ToList();

        var command = new ReadDeviceSensorsDataCommand(edgeMac, readings, resource.syncedAt);

        return command;
    }
}
