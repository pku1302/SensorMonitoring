using Microsoft.AspNetCore.Mvc;
using Moq;
using SensorMonitoring.Api.Controllers;
using SensorMonitoring.Api.Services;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Tests.Controllers;

public class SensorREadingsControllerTests
{
    [Fact]
    public async Task GetRecent_WhenSensorDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var serviceMock =
            new Mock<ISensorReadingService>();

        serviceMock
            .Setup(x => x.GetRecentAsync(
                "Sensor-001",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<SensorReadingDto>?)null);

        var controller =
            new SensorReadingsController(
                serviceMock.Object);

        // Act
        var result =
            await controller.GetRecent(
                "Sensor-001",
                CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(
            result.Result);
    }

    [Fact]
    public async Task GetRecent_WhenSensorExists_ReturnOk()
    {
        // Arrange
        var readings = new List<SensorReadingDto>
        {
            new SensorReadingDto
            {
                Temperature = 25.5,
                Humidity = 50,
                TotalProduction = 100,
                RecordedAt = DateTime.UtcNow
            }
        };

        var serviceMock =
            new Mock<ISensorReadingService>();

        serviceMock
            .Setup(x => x.GetRecentAsync(
                "Sensor-001",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readings);

        var controller =
            new SensorReadingsController(
                serviceMock.Object);

        // Act
        var result =
            await controller.GetRecent(
                "Sensor-001",
                CancellationToken.None);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(
                result.Result);

        var value =
            Assert.IsAssignableFrom<
                IEnumerable<SensorReadingDto>>(
                okResult.Value);

        Assert.Single(value);
    }
}
