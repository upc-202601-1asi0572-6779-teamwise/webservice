namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.ACL;

public interface IIotDeviceQueryFacade
{
    Task<bool> ExistsByMacAddress(string macAddress);
}
