using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistance.EFC.Repositories;

public class EdgeRegistryRepository(AppDbContext context)
    : BaseRepository<EdgeRegistry>(context),
        IEdgeRegistryRepository
{
    public async Task<List<EdgeRegistry>> FindByEdgeMacAddress(string edgeMacAddress)
    {
        return await Context
            .Set<EdgeRegistry>()
            .Where(registry => registry.EdgeMacAddress.Equals(edgeMacAddress))
            .ToListAsync();
    }

    public async Task<List<EdgeRegistry>> FindByIotDeviceMacAddress(string iotDeviceMacAddress)
    {
        return await Context
            .Set<EdgeRegistry>()
            .Where(registry => registry.IotDeviceMacAddresses.Equals(iotDeviceMacAddress))
            .ToListAsync();
    }

    public async Task<EdgeRegistry?> FindByEdgeAndIotMacAddresses(
        string edgeMacAddress,
        string iotDeviceMacAddress
    )
    {
        return await Context
            .Set<EdgeRegistry>()
            .FirstOrDefaultAsync(registry =>
                registry.EdgeMacAddress.Equals(edgeMacAddress)
                && registry.IotDeviceMacAddresses.Equals(iotDeviceMacAddress)
            );
    }
}
