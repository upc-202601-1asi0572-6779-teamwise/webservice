using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Factory;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.CommandServices;

public class SensorReadingCommandService(
    IUnitOfWork uow,
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
            await uow.CompleteAsync();

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
    }

    public async Task Handle(UpdateAgronomicThresholdCommand command)
    {
        var threshold = await agronomicThresholdRepository.FindByEdgeDeviceMacAddressAndSensorType(
            command.EdgeDeviceMacAddress,
            command.Type
        );

        if (threshold is null)
        {
            var newThreshold = AgronomicThresholdTypeFactory.DefaultThreshold(
                command.EdgeDeviceMacAddress,
                command.IotDeviceMacAddress,
                command.Type
            );

            await agronomicThresholdRepository.AddAsync(newThreshold);
            await uow.CompleteAsync();
            return;
        }

        threshold.Update(command.Min, command.Max, command.Description);

        agronomicThresholdRepository.Update(threshold);
        await uow.CompleteAsync();
    }
}
