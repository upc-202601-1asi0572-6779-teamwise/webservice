namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

public record ConnectivityStatusResource(string serialNumber, bool isConnected, string status);
