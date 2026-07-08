namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

public record GatewayDeviceResource(string deviceMac);

public record GatewayDevicesResource(string gatewayMac, List<GatewayDeviceResource> devices);
