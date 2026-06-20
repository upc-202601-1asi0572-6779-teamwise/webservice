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
        // TODO: validate if adming user is making changes from IAM BC
        if (command.Username != "admin" || command.Password != "admin")
        {
            throw new Exception("Unauthorized");
        }

        var device = await edgeDeviceRepository.FindByMacAddress(command.EdgeDeviceMac);
        if (device is not null)
        {
            throw new Exception("Edge Device already registered");
        }

        // TODO: Validate monitoringZoneId command
        //

        device = new EdgeDevice(command.EdgeDeviceMac, command.MonitoringZoneId);

        await edgeDeviceRepository.AddAsync(device);
        await uow.CompleteAsync();
    }

    public async Task Handle(RegisterIotDeviceCommand command)
    {
        // TODO: validate if adming user is making changes from IAM
        if (command.Username != "admin" || command.Password != "admin")
        {
            throw new Exception("Unauthorized");
        }

        var edgeDevice = await edgeDeviceRepository.FindByMacAddress(command.EdgeDeviceMac);

        if (edgeDevice is null)
        {
            throw new Exception("Edge Device not found");
        }

        var iotDevice = await iotDeviceRepository.FindByMacAddress(command.IotDeviceMac);

        if (iotDevice is not null)
        {
            throw new Exception("Iot Device already registered");
        }

        var registry = await edgeRegistryRepository.FindByEdgeAndIotMacAddresses(
            command.EdgeDeviceMac,
            command.IotDeviceMac
        );

        if (registry is not null)
        {
            throw new Exception("Iot Device already registered to Edge Device");
        }

        registry = new EdgeRegistry(command.EdgeDeviceMac, command.IotDeviceMac);

        await edgeRegistryRepository.AddAsync(registry);
        await uow.CompleteAsync();

        var newIotDevice = new IotDevice(command.IotDeviceMac, command.EdgeDeviceMac);

        await iotDeviceRepository.AddAsync(newIotDevice);
        await uow.CompleteAsync();

        System.Console.WriteLine("EDGE DEVICE: " + command.EdgeDeviceMac);
        System.Console.WriteLine("IOT DEVICE: " + command.IotDeviceMac);
        await mediator.Publish(
            new IotDeviceRegisteredEvent(command.EdgeDeviceMac, command.IotDeviceMac)
        );
    }

    public async Task Handle(EdgeSynchronizationCommand command)
    {
        var device = await edgeDeviceRepository.FindByMacAddress(command.EdgeDeviceMac);
        if (device is null)
        {
            throw new Exception("Device not found");
        }

        await mediator.Publish(
            new IotDeviceSynchronizationEvent(device.MacAddress, command.readings)
        );

        device.SynchronizeEdgeData();
        edgeDeviceRepository.Update(device);
        await uow.CompleteAsync();
    }
}
