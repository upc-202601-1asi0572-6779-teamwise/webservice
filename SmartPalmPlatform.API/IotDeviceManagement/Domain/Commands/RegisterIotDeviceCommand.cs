namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

public record RegisterIotDeviceCommand(
    string EdgeDeviceMac,
    string IotDeviceMac,
    string Username,
    string Password
);
