using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Infraestructure.Persistance.EFC;

public class KnownEdgeGatewayRepository(AppDbContext context)
    : BaseRepository<KnownEdgeGateway>(context),
        IKnownEdgeGatewayRepository
{
    public async Task<bool> ExistsByMacAddress(string edgeDeviceMacAddress)
    {
        return await Context
            .Set<KnownEdgeGateway>()
            .AnyAsync(gateway => gateway.EdgeDeviceMacAddress.Equals(edgeDeviceMacAddress));
    }
}
