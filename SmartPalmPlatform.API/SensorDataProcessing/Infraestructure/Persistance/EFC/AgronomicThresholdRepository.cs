using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Infraestructure.Persistance.EFC;

public class AgronomicThresholdRepository(AppDbContext context)
    : BaseRepository<AgronomicThreshold>(context),
        IAgronomicThresholdRepository
{
    public async Task<List<AgronomicThreshold>> FindByEdgeDeviceMacAddress(
        string edgeDeviceMacAddress
    )
    {
        return await Context
            .Set<AgronomicThreshold>()
            .Where(threshold => threshold.EdgeDeviceMacAddress.Equals(edgeDeviceMacAddress))
            .ToListAsync();
    }

    public async Task<AgronomicThreshold?> FindByEdgeDeviceMacAddressAndSensorType(
        string edgeDeviceMacAddress,
        SensorType type
    )
    {
        return await Context
            .Set<AgronomicThreshold>()
            .FirstOrDefaultAsync(threshold =>
                threshold.EdgeDeviceMacAddress.Equals(edgeDeviceMacAddress)
                && threshold.Type.Equals(type)
            );
    }

    public async Task<List<AgronomicThreshold>> FindByEdgeDeviceMacAddressAndIotDeviceMacAddress(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress
    )
    {
        return await Context
            .Set<AgronomicThreshold>()
            .Where(threshold =>
                threshold.EdgeDeviceMacAddress.Equals(edgeDeviceMacAddress)
                && threshold.IotDeviceMacAddress.Equals(iotDeviceMacAddress)
            )
            .ToListAsync();
    }
}
