namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

public record RegisterDeviceCommand(
    string serial,
    int monitoringZoneId,
    string Username,
    string Password
);
