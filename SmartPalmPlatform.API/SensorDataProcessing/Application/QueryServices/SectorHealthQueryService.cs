using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Enums;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.QueryServices;

public class SectorHealthQueryService(
    ICropMonitoringFacade cropMonitoringFacade,
    ISensorReadingRepository sensorReadingRepository,
    IAgronomicThresholdRepository thresholdRepository
) : ISectorHealthQueryService
{
    public async Task<SectorHealthResult?> Handle(GetSectorHealthQuery query)
    {
        var deviceMac = await cropMonitoringFacade.GetSectorIotDeviceMacAsync(query.SectorId);
        if (deviceMac is null)
            return null;

        var readings = await sensorReadingRepository.FindLatestByDeviceMacAsync(deviceMac);
        var thresholds = await thresholdRepository.FindByIotDeviceMacAddress(deviceMac);

        var sensorDetails = new List<SensorHealthDetail>();
        var hasCritical = false;
        var hasWarning = false;

        foreach (var reading in readings)
        {
            var threshold = thresholds.FirstOrDefault(t => t.Type == reading.Type);
            var isExceeded = false;
            double? min = null;
            double? max = null;

            if (threshold is not null)
            {
                min = threshold.Min;
                max = threshold.Max;
                isExceeded = reading.Value < threshold.Min || reading.Value > threshold.Max;

                if (isExceeded)
                {
                    var deviation = CalculateDeviation(reading.Value, threshold.Min, threshold.Max);
                    if (deviation > 0.2)
                        hasCritical = true;
                    else
                        hasWarning = true;
                }
            }

            sensorDetails.Add(new SensorHealthDetail(
                reading.Type.ToString(),
                reading.Value,
                min,
                max,
                isExceeded
            ));
        }

        var status = hasCritical ? SectorHealthStatus.Critical
                   : hasWarning ? SectorHealthStatus.Warning
                   : SectorHealthStatus.Healthy;

        return new SectorHealthResult(query.SectorId, status, sensorDetails);
    }

    private double CalculateDeviation(double value, double min, double max)
    {
        if (value < min)
            return (min - value) / min;
        if (value > max)
            return (value - max) / max;
        return 0;
    }
}