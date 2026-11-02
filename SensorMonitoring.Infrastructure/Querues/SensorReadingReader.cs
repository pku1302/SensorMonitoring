using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;
using SensorMonitoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Infrastructure.Querues;

public class SensorReadingReader : ISensorReadingReader
{
    private readonly MonitoringDbContext _dbContext;
    public SensorReadingReader(MonitoringDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SensorReadingDto>?> GetRecentAsync(
        string deviceId,
        DateTime fromUtc,
        CancellationToken cancellationToken)
    {
        var sensorId = await _dbContext.Sensors
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (sensorId is null)
            return null;

        return await _dbContext.SensorReadings
            .AsNoTracking()
            .Where(x =>
                x.SensorId == sensorId.Value &&
                x.RecordedAt >= fromUtc)
            .OrderBy(x => x.RecordedAt)
            .Select(x => new SensorReadingDto
            {
                Temperature = x.Temperature,
                Humidity = x.Humidity,
                TotalProduction = x.TotalProduction,
                RecordedAt = x.RecordedAt,
            })
            .ToListAsync(cancellationToken);

    }
}
