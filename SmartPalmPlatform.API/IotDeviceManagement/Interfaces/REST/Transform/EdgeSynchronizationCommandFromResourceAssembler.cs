using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class EdgeSynchronizationCommandFromResourceAssembler
{
    public static EdgeSynchronizationCommand ToCommandFromResource(
        string edgeMac,
        EdgeSynchronizationResource resource
    )
    {
        return new EdgeSynchronizationCommand(
            edgeMac,
            resource
                .readings.Select(r => new SensorReadingPayload(
                    SensorTypeFromStringAssembler.FromStringToSensorType(r.sensorType),
                    r.measuredAt,
                    r.value
                ))
                .ToList()
        );
    }
}
