using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Infraestructure.Persistance.EFC;

public class SensorReadingRepository(AppDbContext context)
    : BaseRepository<SensorReading>(context),
        ISensorReadingRepository
{
    public async Task<List<SensorReading>> FindByEdgeDeviceMacAddressAndMeasureTimeRange(
        string edgeDeviceMacAddress,
        DateTime fromTime,
        DateTime toTime,
        string? iotDeviceMacAddress,
        int page,
        int size
    )
    {
        var query = Context
            .Set<SensorReading>()
            .Where(reading =>
                reading.EdgeDeviceMacAddress.Equals(edgeDeviceMacAddress)
                && reading.MeasuredAt >= fromTime
                && reading.MeasuredAt <= toTime
            );

        if (!string.IsNullOrWhiteSpace(iotDeviceMacAddress))
            query = query.Where(reading =>
                reading.IotDeviceMacAddress.Equals(iotDeviceMacAddress)
            );

        return await query
            .OrderByDescending(reading => reading.MeasuredAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<List<SensorReading>> FindByIotDeviceMacAddressAndMeasureTimeRange(
        string iotDeviceMacAddress,
        DateTime fromTime,
        DateTime toTime,
        int page,
        int size
    )
    {
        return await Context
            .Set<SensorReading>()
            .Where(reading =>
                reading.IotDeviceMacAddress.Equals(iotDeviceMacAddress)
                && reading.MeasuredAt >= fromTime
                && reading.MeasuredAt <= toTime
            )
            .OrderByDescending(reading => reading.MeasuredAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }
}
