namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

public record RegisterEdgeDeviceCommand(
    string EdgeDeviceMac,
    int MonitoringZoneId,
    string Username,
    string Password
);
