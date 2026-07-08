using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;

public interface IKnownEdgeGatewayRepository : IBaseRepository<KnownEdgeGateway>
{
    Task<bool> ExistsByMacAddress(string edgeDeviceMacAddress);
}
