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
        using var testDb = new TestDbContext();

        var service =
            new SensorReadingService(testDb.Context);

        // Act
        var result =
             await service.GetRecentAsync(
                 "Sensor-999",
                 CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRecentAsync_WhenSensorExistsButHasNoRecentData_ReturnsEmptyList()
    {
        // Arrange
        using var testDb = new TestDbContext();

        var sensor = new Sensor
        {
            DeviceId = "Sensor-001",
            Name = "Sensor 1",
            IsEnabled = true
        };

        testDb.Context.Sensors.Add(sensor);

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        var service =
            new SensorReadingService(testDb.Context);

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

        var sensor = new Sensor
        {
            DeviceId = "Sensor-001",
            Name = "Sensor 1",
            IsEnabled = true
        };

        testDb.Context.Sensors.Add(sensor);

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        testDb.Context.SensorReadings.Add(
            new SensorReading
            {
                SensorId = sensor.Id,
                Temperature = 25.5,
                Humidity = 50.0,
                TotalProduction = 100,
                RecordedAt = DateTime.UtcNow.AddSeconds(-30)
            });

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        var service =
            new SensorReadingService(testDb.Context);

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

        var sensor = new Sensor
        {
            DeviceId = "Sensor-001",
            Name = "Sensor 1",
            IsEnabled = true
        };

        testDb.Context.Sensors.Add(sensor);

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        testDb.Context.SensorReadings.AddRange(
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

        await testDb.Context.SaveChangesAsync(CancellationToken.None);

        var service =
            new SensorReadingService(testDb.Context);

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
