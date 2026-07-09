using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.QueryServices;

public class DeviceStatusQueryService(
    IEdgeDeviceRepository deviceRepository,
    IEdgeRegistryRepository edgeRegistryRepository
) : IDeviceStatusQueryService
{
    public async Task<EdgeDevice> Handle(ConnectiviyStatusQuery query)
    {
        var device = await deviceRepository.FindByMacAddress(query.mac);

        if (device is null)
            throw new KeyNotFoundException("Edge Device not found.");

        return device;
    }

    public async Task<Tuple<EdgeDevice, List<EdgeRegistry>>> Handle(EdgeRegistryQuery query)
    {
        var edgeDevice = await deviceRepository.FindByMacAddress(query.EdgeDeviceMac);

        if (edgeDevice is null)
            throw new KeyNotFoundException("Edge Device not found.");

        // 404 en vez de 403 si el gateway no pertenece al usuario: evita confirmar
        // la existencia de gateways ajenos por enumeración de MAC.
        if (query.OwnerUserId.HasValue && edgeDevice.UserId != query.OwnerUserId.Value)
            throw new KeyNotFoundException("Edge Device not found.");

        var registry = await edgeRegistryRepository.FindByEdgeMacAddress(query.EdgeDeviceMac);

        if (registry is null)
            throw new KeyNotFoundException("Registry not found.");

        if (registry.Count == 0)
            return Tuple.Create(edgeDevice, new List<EdgeRegistry>());

        return Tuple.Create(edgeDevice, registry);
    }

    public async Task<IEnumerable<EdgeDevice>> Handle(GetAllEdgeGatewaysQuery query)
    {
        return query.OwnerUserId.HasValue
            ? await deviceRepository.ListByUserId(query.OwnerUserId.Value)
            : await deviceRepository.ListAsync();
    }
}
