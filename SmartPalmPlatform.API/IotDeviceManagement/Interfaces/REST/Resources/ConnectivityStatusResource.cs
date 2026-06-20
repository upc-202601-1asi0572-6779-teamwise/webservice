namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

public record ConnectivityStatusResource(string mac, bool isConnected, string status);
