using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;
using SensorMonitoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Infrastructure.Querues;

public class SensorHistoryReader : ISensorHistoryReader
{
    private readonly MonitoringDbContext _dbContext;

    public SensorHistoryReader(MonitoringDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SensorListItemDto>> GetSensorsAsync(
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct)
    {
        
        return await _dbContext.Sensors
            .AsNoTracking()
            .Where(sensor =>
                _dbContext.SensorReadings.Any(reading =>
                    reading.SensorId == sensor.Id &&
                    reading.RecordedAt >= startUtc &&
                    reading.RecordedAt < endUtc))
            .OrderBy(sensor => sensor.DeviceId)
            .Select(sensor => new SensorListItemDto
            {
                DeviceId = sensor.DeviceId,
                Name = sensor.Name
            })
            .ToListAsync(ct);
    }

    public async Task<List<SensorReadingDto>?> GetReadingsAsync(
        string deviceId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct)
    {
        var sensorId = await _dbContext.Sensors
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(ct);

        if (sensorId is null)
            return null;

        return await _dbContext.SensorReadings
            .AsNoTracking()
            .Where(x =>
                x.SensorId == sensorId.Value &&
                x.RecordedAt >= startUtc &&
                x.RecordedAt < endUtc)
            .OrderBy(x => x.RecordedAt)
            .ThenBy(x => x.Id)
            .Select(x => new SensorReadingDto
            {
                Temperature = x.Temperature,
                Humidity = x.Humidity,
                TotalProduction = x.TotalProduction,
                RecordedAt = x.RecordedAt,
            })
            .ToListAsync(ct);
    }

    public async Task<SensorHistoryChartDto?> GetChartAsync(
        string deviceId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct)
    {
        var sensor = await _dbContext.Sensors
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .Select(x => new
            {
                x.Id,
                TemperatureDanger = x.AlarmSetting != null
                    ? x.AlarmSetting.TemperatureDanger
                    : null,
                HumidityDanger = x.AlarmSetting != null
                    ? x.AlarmSetting.HumidityDanger
                    : null
            })
            .SingleOrDefaultAsync(ct);

        if (sensor is null)
            return null;

        var buckets = await _dbContext.SensorReadings
            .AsNoTracking()
            .Where(x =>
                x.SensorId == sensor.Id &&
                x.RecordedAt >= startUtc &&
                x.RecordedAt < endUtc)
            .GroupBy(x =>
                EF.Functions.DateDiffMinute(
                    startUtc, x.RecordedAt) / 5)
            .Select(g => new
            {
                Bucket = g.Key,
                AverageTemperature = g.Average(x => x.Temperature),
                MinTemperature = g.Min(x => x.Temperature),
                MaxTemperature = g.Max(x => x.Temperature),
                AverageHumidity = g.Average(x => x.Humidity),
                MinHumidity = g.Min(x => x.Humidity),
                MaxHumidity = g.Max(x => x.Humidity)
            })
            .OrderBy(x => x.Bucket)
            .ToListAsync(ct);

        return new SensorHistoryChartDto
        {
            TemperatureDangerValue = sensor.TemperatureDanger ?? 35,
            HumidityDangerValue = sensor.HumidityDanger ?? 80,

            Points = buckets.Select(x => new SensorHistoryChartPointDto
            {
                Timestamp = startUtc.AddMinutes(x.Bucket * 5),
                AverageTemperature = x.AverageTemperature,
                MinTemperature = x.MinTemperature,
                MaxTemperature = x.MaxTemperature,
                AverageHumidity = x.AverageHumidity,
                MinHumidity = x.MinHumidity,
                MaxHumidity = x.MaxHumidity
            }).ToList()
        };
    }
}
