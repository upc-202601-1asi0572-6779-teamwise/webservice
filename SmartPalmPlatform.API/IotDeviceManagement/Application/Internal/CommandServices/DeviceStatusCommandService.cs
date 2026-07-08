using MediatR;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.CommandServices;

public class DeviceStatusCommandService(
    IUnitOfWork uow,
    IMediator mediator,
    IIotDeviceRepository iotDeviceRepository,
    IEdgeDeviceRepository edgeDeviceRepository,
    IEdgeRegistryRepository edgeRegistryRepository
) : IDeviceStatusCommandService
{
    public async Task Handle(RegisterEdgeDeviceCommand command)
    {
        var device = await edgeDeviceRepository.FindByMacAddress(command.EdgeDeviceMac);
        if (device is not null)
            throw new InvalidOperationException("Edge Device already registered.");

        device = new EdgeDevice(command.EdgeDeviceMac, command.MonitoringZoneId);

        await edgeDeviceRepository.AddAsync(device);
        await uow.CompleteAsync();
    }

    public async Task Handle(RegisterIotDeviceCommand command)
    {
        var edgeDevice = await edgeDeviceRepository.FindByMacAddress(command.EdgeDeviceMac);
        if (edgeDevice is null)
            throw new KeyNotFoundException("Edge Device not found.");

        var iotDevice = await iotDeviceRepository.FindByMacAddress(command.IotDeviceMac);
        if (iotDevice is not null)
            throw new InvalidOperationException("IoT Device already registered.");

        var registry = await edgeRegistryRepository.FindByEdgeAndIotMacAddresses(
            command.EdgeDeviceMac,
            command.IotDeviceMac
        );
        if (registry is not null)
            throw new InvalidOperationException("IoT Device already registered to this Edge Device.");

        registry = new EdgeRegistry(command.EdgeDeviceMac, command.IotDeviceMac);
        await edgeRegistryRepository.AddAsync(registry);
        await uow.CompleteAsync();

        var newIotDevice = new IotDevice(command.EdgeDeviceMac, command.IotDeviceMac);
        await iotDeviceRepository.AddAsync(newIotDevice);
        await uow.CompleteAsync();

        await mediator.Publish(
            new IotDeviceRegisteredEvent(command.EdgeDeviceMac, command.IotDeviceMac)
        );
    }

    public async Task Handle(EdgeSynchronizationCommand command)
    {
        var device = await edgeDeviceRepository.FindByMacAddress(command.EdgeDeviceMac);
        if (device is null)
            throw new KeyNotFoundException("Edge Device not found.");

        await mediator.Publish(
            new IotDeviceSynchronizationEvent(device.MacAddress, command.readings)
        );

        device.SynchronizeEdgeData();
        edgeDeviceRepository.Update(device);
        await uow.CompleteAsync();
    }
}
