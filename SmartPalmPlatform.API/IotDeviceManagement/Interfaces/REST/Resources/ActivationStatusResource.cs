namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

public record ActivationStatusResource(string serialNumber, bool isActive);
