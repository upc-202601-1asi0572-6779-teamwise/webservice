namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

public record DeviceReadingsResource(string deviceMac, List<SensorDataResource> readings);

public record ReadDeviceSensorsDataResource(
    List<DeviceReadingsResource> devices,
    DateTime syncedAt
);
