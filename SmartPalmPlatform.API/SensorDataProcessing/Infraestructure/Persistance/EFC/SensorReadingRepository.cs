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
        DateTime toTime
    )
    {
        return await Context
            .Set<SensorReading>()
            .Where(reading =>
                reading.EdgeDeviceMacAddress.Equals(edgeDeviceMacAddress)
                && reading.MeasuredAt >= fromTime
                && reading.MeasuredAt <= toTime
            )
            .ToListAsync();
    }
}
