using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;

public class IotDeviceRepository(AppDbContext context)
    : BaseRepository<IotDevice>(context),
        IIotDeviceRepository
{
    public async Task<IotDevice?> FindBySerialNumberAsync(string serial)
    {
        return await Context
            .Set<IotDevice>()
            .FirstOrDefaultAsync(device => device.SerialNumber.Equals(serial));
    }
}
