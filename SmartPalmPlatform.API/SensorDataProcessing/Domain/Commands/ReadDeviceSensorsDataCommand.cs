using SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;

public record ReadDeviceSensorsDataCommand(
    string EdgeDeviceMacAddress,
    List<SensorReadingPayload> Readings
);
