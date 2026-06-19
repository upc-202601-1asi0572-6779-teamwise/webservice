using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistance.EFC.Repositories;

public class EdgeDeviceRepository(AppDbContext context)
    : BaseRepository<EdgeDevice>(context),
        IEdgeDeviceRepository
{
    public async Task<EdgeDevice?> FindByMacAddress(string EdgeDeviceMacAddress)
    {
        return await Context
            .Set<EdgeDevice>()
            .FirstOrDefaultAsync(device => device.MacAddress.Equals(EdgeDeviceMacAddress));
    }
}
