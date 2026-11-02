using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Infrastructure.Repositories;

public class SensorReadingRepository : ISensorReadingRepository
{
    private readonly MonitoringDbContext _dbContext;

    public SensorReadingRepository(
        MonitoringDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<long> AddAsync(
        SensorReading reading,
        CancellationToken cancellationToken)
    {
        _dbContext.SensorReadings.Add(reading);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return reading.Id;
    }
}
