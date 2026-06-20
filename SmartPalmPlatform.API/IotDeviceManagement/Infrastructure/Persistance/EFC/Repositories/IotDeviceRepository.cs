using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistance.EFC.Repositories;

public class IotDeviceRepository(AppDbContext context)
    : BaseRepository<IotDevice>(context),
        IIotDeviceRepository
{
    public async Task<IotDevice?> FindByMacAddress(string IotDeviceMacAddress)
    {
        return await Context
            .Set<IotDevice>()
            .FirstOrDefaultAsync(device => device.MacAddress.Equals(IotDeviceMacAddress));
    }
}
