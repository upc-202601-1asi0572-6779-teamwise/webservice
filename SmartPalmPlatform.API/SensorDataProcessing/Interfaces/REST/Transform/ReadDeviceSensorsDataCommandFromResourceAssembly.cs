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
        var command = new ReadDeviceSensorsDataCommand(
            edgeMac,
            resource
                // TODO(Fase 2): este contrato no trae deviceMac por lectura; se rediseñará
                // como { devices: [{ deviceMac, readings: [...] }] } para atribuir cada
                // lectura a su dispositivo IoT de origen.
                .readings.Select(r => new SensorReadingPayload(
                    string.Empty,
                    SensorTypeFromStringAssembler.FromStringToSensorType(r.sensorType),
                    r.measuredAt,
                    r.value
                ))
                .ToList()
        );

        return command;
    }
}
