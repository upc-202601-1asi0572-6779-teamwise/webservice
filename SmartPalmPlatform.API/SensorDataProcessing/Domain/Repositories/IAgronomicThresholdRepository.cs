using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;

public interface IAgronomicThresholdRepository : IBaseRepository<AgronomicThreshold>
{
    public Task<List<AgronomicThreshold>> FindByEdgeDeviceMacAddress(string edgeDeviceMacAddress);
    public Task<List<AgronomicThreshold>> FindByEdgeDeviceMacAddressAndIotDeviceMacAddress(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress
    );
    public Task<AgronomicThreshold?> FindByEdgeDeviceMacAddressAndSensorType(
        string edgeDeviceMacAddress,
        SensorType type
    );

    // Todo IoT device registrado recibe un AgronomicThreshold por cada SensorType al
    // registrarse (ver IotDeviceRegisteredEventHandler), así que esta tabla sirve como
    // comprobante local de existencia sin depender del repositorio de IotDeviceManagement.
    public Task<bool> ExistsByIotDeviceMacAddress(string iotDeviceMacAddress);
}
