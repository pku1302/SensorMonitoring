using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Contracts;
using SensorMonitoring.Infrastructure.Data;

namespace SensorMonitoring.Api.Services.Get;

public class SensorReadingService : ISensorReadingService
{
    private readonly MonitoringDbContext _dbContext;

    public SensorReadingService(
        MonitoringDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SensorReadingDto>?> GetRecentAsync(
        string deviceId,
        CancellationToken cancellationToken)
    {
        var sensor = await _dbContext.Sensors
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.DeviceId == deviceId,
                cancellationToken);

        if (sensor is null)
            return null;

        var from = DateTime.UtcNow.AddMinutes(-1);

        return await _dbContext.SensorReadings
            .AsNoTracking()
            .Where(x =>
                x.SensorId == sensor.Id &&
                x.RecordedAt >= from)
            .OrderBy(x => x.RecordedAt)
            .Select(x => new SensorReadingDto
            {
                Temperature = x.Temperature,
                Humidity = x.Humidity,
                TotalProduction = x.TotalProduction,
                RecordedAt = x.RecordedAt
            })
            .ToListAsync(cancellationToken);
    }
}
