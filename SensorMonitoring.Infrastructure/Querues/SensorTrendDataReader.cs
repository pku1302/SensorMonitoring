using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Application.Models;
using SensorMonitoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Infrastructure.Querues;

public class SensorTrendDataReader : ISensorTrendDataReader
{
    private readonly MonitoringDbContext _dbContext;

    public SensorTrendDataReader(
        MonitoringDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SensorTrendSensorInfo?> GetAsync(
        string deviceId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Sensors
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .Select(x => new SensorTrendSensorInfo
            {
                DeviceId = x.DeviceId,

                TemperatureDanger =
                    x.AlarmSetting != null
                        ? x.AlarmSetting.TemperatureDanger ?? 35.0
                        : 35.0,

                HumidityDanger =
                    x.AlarmSetting != null
                        ? x.AlarmSetting.HumidityDanger ?? 80.0
                        : 80.0
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SensorTrendSensorInfo>>
        GetAllEnabledAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Sensors
            .AsNoTracking()
            .Where(x => x.IsEnabled)
            .OrderBy(x => x.DeviceId)
            .Select(x => new SensorTrendSensorInfo
            {
                DeviceId = x.DeviceId,

                TemperatureDanger =
                    x.AlarmSetting != null
                        ? x.AlarmSetting.TemperatureDanger ?? 35.0
                        : 35.0,

                HumidityDanger =
                    x.AlarmSetting != null
                        ? x.AlarmSetting.HumidityDanger ?? 80.0
                        : 80.0
            })
            .ToListAsync(cancellationToken);
    }
}
