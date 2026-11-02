using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Application.Models;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Services;

public class SensorTrendService
{
    private readonly ISensorTrendDataReader _sensorReader;
    private readonly ISensorChartCache _chartCache;

    public SensorTrendService(
        ISensorTrendDataReader sensorReader,
        ISensorChartCache chartCache)
    {
        _sensorReader = sensorReader;
        _chartCache = chartCache;
    }

    public async Task<List<SensorTrendDto>?> GetAsync(
        string deviceId,
        CancellationToken cancellationToken)
    {
        var sensor = await _sensorReader.GetAsync(
            deviceId,
            cancellationToken);

        if (sensor is null)
            return null;

        return CreateTrends(sensor);
    }

    public async Task<List<SensorChartsDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var sensors = await _sensorReader
            .GetAllEnabledAsync(cancellationToken);

        return sensors.Select(sensor => new SensorChartsDto
        {
            DeviceId = sensor.DeviceId,
            Trends = CreateTrends(sensor)
        }).ToList();
    }

    private List<SensorTrendDto> CreateTrends(
        SensorTrendSensorInfo sensor)
    {
        var points = _chartCache.Get(sensor.DeviceId);

        return
        [
            new SensorTrendDto
            {
                Name = "온도",
                Unit = "°C",
                MinValue = 0,
                MaxValue = 100,
                DangerValue = sensor.TemperatureDanger,

                Values = points.Select(x => new ChartData
                {
                    Time = x.Timestamp,
                    Value = x.Temperature
                }).ToList()
            },

            new SensorTrendDto
            {
                Name = "습도",
                Unit = "%",
                MinValue = 0,
                MaxValue = 100,
                DangerValue = sensor.HumidityDanger,

                Values = points.Select(x => new ChartData
                {
                    Time = x.Timestamp,
                    Value = x.Humidity
                }).ToList()
            }
        ];
    }

}
