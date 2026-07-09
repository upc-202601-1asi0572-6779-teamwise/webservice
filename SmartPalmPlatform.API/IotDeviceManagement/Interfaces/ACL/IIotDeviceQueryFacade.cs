namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.ACL;

public interface IIotDeviceQueryFacade
{
    Task<bool> ExistsByMacAddress(string macAddress);

    // Devuelve el UserId dueño del IoT device, o null si el MAC no está registrado.
    Task<int?> GetOwnerUserIdByMacAddress(string macAddress);
}
