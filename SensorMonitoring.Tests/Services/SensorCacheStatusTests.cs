using Microsoft.EntityFrameworkCore;
using Moq;
using SensorMonitoring.Api.Services;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Infrastructure.Data;
using SensorMonitoring.Tests.Helpers;

namespace SensorMonitoring.Tests.Services;

public class SensorCacheStatusTests
{
    [Fact]
    public async Task DbAlarmSetting_IsLoadedIntoCache_AndUsedForStatusEvaluation()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        
        using var testDb = new TestDbContext();
        
        await using (var dbContext = testDb.CreateContext())
        {
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

            dbContext.Sensors.Add(sensor);

            await dbContext.SaveChangesAsync(ct);
        }

        var cache =
            new SensorIdCache();

        var dbContextFactory =
            new Mock<IDbContextFactory<MonitoringDbContext>>();

        dbContextFactory
            .Setup(x => x.CreateDbContextAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => testDb.CreateContext());

        var cacheService =
            new SensorCacheService(
                cache,
                dbContextFactory.Object);

        var statusService =
            new SensorStatusService();

        // Act
        var sensorInfo =
            await cacheService.GetAsync(
                "Sensor-001",
                0,
                ct);

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

        Assert.Equal(
            0,
            sensorInfo.TotalProduction);

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

        // 초기 데이터 삽입
        await using (var dbContext = testDb.CreateContext())
        {
            dbContext.Sensors.Add(
                new Sensor
                {
                    DeviceId = "SENSOR-001",
                    Name = "Sensor 1",

                    AlarmSetting = new SensorAlarmSetting
                    {
                        IsEnabled = true,
                        TemperatureWarning = 28,
                        TemperatureDanger = 32
                    }
                });

            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var cache = new SensorIdCache();

        var factoryMock =
            new Mock<IDbContextFactory<MonitoringDbContext>>();

        factoryMock
            .Setup(x => x.CreateDbContextAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => testDb.CreateContext());

        var cacheService =
            new SensorCacheService(
                cache,
                factoryMock.Object);

        // 첫 조회 → DB → Cache
        var first =
            await cacheService.GetAsync(
                "SENSOR-001",
                0,
                CancellationToken.None);

        Assert.Equal(
            28,
            first!.SensorAlarmSetting!.TemperatureWarning);

        // DB 값 변경
        await using (var dbContext = testDb.CreateContext())
        {
            var cancellationToken =
                TestContext.Current.CancellationToken;

            var sensor =
                await dbContext.Sensors
                    .Include(x => x.AlarmSetting)
                    .FirstAsync(
                        x => x.DeviceId == "SENSOR-001",
                        cancellationToken);

            sensor.AlarmSetting!.TemperatureWarning = 20;

            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        // 두 번째 조회
        var second =
            await cacheService.GetAsync(
                "SENSOR-001",
                0,
                CancellationToken.None);

        // 캐시가 남아 있으므로 28
        Assert.Equal(
            28,
            second!.SensorAlarmSetting!.TemperatureWarning);
    }
}
