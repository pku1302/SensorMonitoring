using SensorMonitoring.Api.Controllers;
using SensorMonitoring.Api.Services.Get;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Tests.Services;

public class SensorReadingServiceTests
{
    [Fact]
    public async Task GetRecentAsync_WhenSensorDoseNotExist_ReturnsNull()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;

        using var testDb = new TestDbContext();

        await using var dbContext =
            testDb.CreateContext();

        var service =
            new SensorReadingService(dbContext);

        // Act
        var result =
             await service.GetRecentAsync(
                 "Sensor-999",
                 ct);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRecentAsync_WhenSensorExistsButHasNoRecentData_ReturnsEmptyList()
    {
        // Arrange
        using var testDb = new TestDbContext();

        await using var dbContext =
            testDb.CreateContext();

        var sensor = new Sensor
        {
            DeviceId = "Sensor-001",
            Name = "Sensor 1",
            IsEnabled = true
        };

        dbContext.Sensors.Add(sensor);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service =
            new SensorReadingService(dbContext);

        // Act
        var result =
            await service.GetRecentAsync(
                "Sensor-001",
                CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRecentAsync_WhenRecentDataExists_ReturnsRecentData()
    {
        // Arrange
        using var testDb = new TestDbContext();

        await using var dbContext =
            testDb.CreateContext();

        var sensor = new Sensor
        {
            DeviceId = "Sensor-001",
            Name = "Sensor 1",
            IsEnabled = true
        };

        dbContext.Sensors.Add(sensor);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        dbContext.SensorReadings.Add(
            new SensorReading
            {
                SensorId = sensor.Id,
                Temperature = 25.5,
                Humidity = 50.0,
                TotalProduction = 100,
                RecordedAt = DateTime.UtcNow.AddSeconds(-30)
            });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service =
            new SensorReadingService(dbContext);

        // Act
        var result =
            await service.GetRecentAsync(
                "Sensor-001",
                CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(25.5, result[0].Temperature);
        Assert.Equal(50.0, result[0].Humidity);
        Assert.Equal(100, result[0].TotalProduction);
    }

    [Fact]
    public async Task GetRecentAsync_WhenOldDataExists_DoesNotReturnOldData()
    {
        // Arrange
        using var testDb = new TestDbContext();

        await using var dbContext =
            testDb.CreateContext();

        var sensor = new Sensor
        {
            DeviceId = "Sensor-001",
            Name = "Sensor 1",
            IsEnabled = true
        };

        dbContext.Sensors.Add(sensor);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        dbContext.SensorReadings.AddRange(
            new SensorReading
            {
                SensorId = sensor.Id,
                Temperature = 25,
                Humidity = 50,
                TotalProduction = 100,
                RecordedAt = DateTime.UtcNow.AddSeconds(30)
            },
            new SensorReading
            {
                SensorId = sensor.Id,
                Temperature = 20,
                Humidity = 40,
                TotalProduction = 90,
                RecordedAt = DateTime.UtcNow.AddMinutes(-5)
            });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service =
            new SensorReadingService(dbContext);

        // Act
        var result =
            await service.GetRecentAsync(
                "Sensor-001",
                CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(25, result[0].Temperature);
    }
}
