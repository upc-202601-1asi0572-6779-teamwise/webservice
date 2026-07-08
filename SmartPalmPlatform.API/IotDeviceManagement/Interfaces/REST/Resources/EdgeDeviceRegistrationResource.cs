namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

public record EdgeDeviceRegistrationResource(
    string edgeMac,
    int monitoringZoneId
);
