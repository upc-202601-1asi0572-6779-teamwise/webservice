using MediatR;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Factory;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.CommandServices;

public class SensorReadingCommandService(
    IUnitOfWork uow,
    IMediator mediator,
    ISensorReadingRepository sensorReadingRepository,
    IAgronomicThresholdRepository agronomicThresholdRepository,
    IThresholdEvaluationService thresholdEvaluationService
) : ISensorReadingCommandService
{
    public async Task Handle(ReadDeviceSensorsDataCommand command)
    {
        var threesholds = await agronomicThresholdRepository.FindByEdgeDeviceMacAddress(
            command.EdgeDeviceMacAddress
        );
        var readings = command.Readings;

        var thresholds = threesholds.ToDictionary(t => t.Type, t => t);

        foreach (var r in readings)
        {
            var reading = SensorReadingTypeFactory.DefaultSensorReading(
                command.EdgeDeviceMacAddress,
                r.IotDeviceMacAddress,
                r.Type,
                r.MeasuredAt,
                r.Value
            );
            await sensorReadingRepository.AddAsync(reading);

            if (!thresholds.TryGetValue(reading.Type, out var threshold))
                continue;

            if (
                threshold.IsThresholdSet()
                && thresholdEvaluationService.IsThresholdExceeded(reading, threshold)
            )
            {
                // TODO: Send alert
            }
        }

        await uow.CompleteAsync();

        await mediator.Publish(
            new SensorReadingsIngestedEvent(command.EdgeDeviceMacAddress, command.SyncedAt)
        );
    }

    public async Task Handle(UpdateAgronomicThresholdCommand command)
    {
        var threshold = await agronomicThresholdRepository.FindByIotDeviceMacAddressAndSensorType(
            command.IotDeviceMacAddress,
            command.Type
        );

        if (threshold is not null)
        {
            threshold.Update(command.Min, command.Max, command.Description);
            agronomicThresholdRepository.Update(threshold);
            await uow.CompleteAsync();
            return;
        }

        // El registro del dispositivo (IotDeviceRegisteredEventHandler) crea los 5
        // thresholds por defecto; si falta justo el de este SensorType, se resuelve el
        // edgeMac desde cualquier otro threshold ya existente del mismo dispositivo, sin
        // depender de IotDeviceManagement. Si no hay ninguno, el dispositivo no existe.
        var existingThresholds = await agronomicThresholdRepository.FindByIotDeviceMacAddress(
            command.IotDeviceMacAddress
        );
        var edgeDeviceMacAddress = existingThresholds.FirstOrDefault()?.EdgeDeviceMacAddress;

        if (edgeDeviceMacAddress is null)
            throw new KeyNotFoundException(
                $"IoT device '{command.IotDeviceMacAddress}' not found."
            );

        var newThreshold = AgronomicThresholdTypeFactory.DefaultThreshold(
            edgeDeviceMacAddress,
            command.IotDeviceMacAddress,
            command.Type
        );

        await agronomicThresholdRepository.AddAsync(newThreshold);
        await uow.CompleteAsync();
    }
}
