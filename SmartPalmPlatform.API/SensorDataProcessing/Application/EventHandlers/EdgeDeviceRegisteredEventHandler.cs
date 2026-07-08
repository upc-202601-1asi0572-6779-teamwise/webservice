using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.EventHandlers;

public class EdgeDeviceRegisteredEventHandler(
    IUnitOfWork uow,
    IKnownEdgeGatewayRepository knownEdgeGatewayRepository
) : IEventHandler<EdgeDeviceRegisteredEvent>
{
    public async Task Handle(
        EdgeDeviceRegisteredEvent notification,
        CancellationToken cancellationToken
    )
    {
        await knownEdgeGatewayRepository.AddAsync(
            new KnownEdgeGateway(notification.EdgeDeviceMacAddress)
        );
        await uow.CompleteAsync();
    }
}
