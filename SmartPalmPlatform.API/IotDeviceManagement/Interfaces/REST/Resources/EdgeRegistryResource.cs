namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

public record RegistryIotDeviceResource(string iotDeviceMac);

public record EdgeRegistryResource(string edgeDeviceMac, List<RegistryIotDeviceResource> registry);
