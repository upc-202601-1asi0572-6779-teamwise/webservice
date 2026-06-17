namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources
{
    public record DeviceRegistrationResource(
        string serialNumber,
        int monitoringZoneId,
        string username,
        string password
    );
}
