using SensorMonitoring.Api.Services;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Tests.Services;

public class SensorCacheStatusTests
{
    [Fact]
    public async Task DbAlarmSetting_IsLoadedIntoCache_AndUsedForStatusEvaluation()
    {
        // Arrange
        using var testDb = new TestDbContext();

        var sensor = new Sensor
        {
            DeviceId = "Sensor-001",
            Name = "Sensor 1",
            IsEnabled = true,

            AlarmSetting = new SensorAlarmSetting
            {
                IsEnabled = true,

                TemperatureWarning = 28,
                TemperatureDanger = 32,

                HumidityWarning = 60,
                HumidityDanger = 75
            }
        };

        testDb.Context.Sensors.Add(sensor);

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        var cache = new SensorIdCache();

        var cacheService =
            new SensorCacheService(cache);

        var statusService =
            new SensorStatusService();

        // Act
        var sensorInfo =
            await cacheService.GetAsync(
                "Sensor-001",
                testDb.Context,
                CancellationToken.None);

        Assert.NotNull(sensorInfo);

        Assert.Equal(
            28,
            sensorInfo.SensorAlarmSetting!.TemperatureWarning);

        Assert.Equal(
            32,
            sensorInfo.SensorAlarmSetting.TemperatureDanger);

        Assert.True(
            cache.TryGet(
                "Sensor-001",
                out var cacheItem));

        var status =
            statusService.Evaluate(
                temperature: 29,
                humidity: 50,
                sensorInfo.SensorAlarmSetting);

        // Assert
        Assert.Equal(
            SensorStatus.Warning,
            status);
    }

    [Fact]
    public async Task GetAsync_WhenCached_ReturnsCachedSetting()
    {
        using var testDb = new TestDbContext();

        var sensor = new Sensor
        {
            DeviceId = "SENSOR-001",
            Name = "Sensor 1",

            AlarmSetting = new SensorAlarmSetting
            {
                IsEnabled = true,
                TemperatureWarning = 28,
                TemperatureDanger = 32
            }
        };

        testDb.Context.Sensors.Add(sensor);

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        var cache = new SensorIdCache();

        var cacheService =
            new SensorCacheService(cache);

        // 첫 조회 → DB → Cache
        var first =
            await cacheService.GetAsync(
                "SENSOR-001",
                testDb.Context,
                CancellationToken.None);

        Assert.Equal(
            28,
            first!.SensorAlarmSetting!.TemperatureWarning);

        // DB 값 변경
        sensor.AlarmSetting!.TemperatureWarning = 20;

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        // 두 번째 조회
        var second =
            await cacheService.GetAsync(
                "SENSOR-001",
                testDb.Context,
                CancellationToken.None);

        // 캐시가 남아 있으므로 28
        Assert.Equal(
            28,
            second!.SensorAlarmSetting!.TemperatureWarning);
    }
}
