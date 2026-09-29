using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Api.Hubs;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Infrastructure.Data;

namespace SensorMonitoring.Api.Services;

public class SensorDataProcessor : BackgroundService
{
    private readonly SensorPacketChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<SensorHub> _hubContext;
    private readonly ILogger<SensorDataProcessor> _logger;
    private readonly ProductionCounterService _productionCounter;

    public SensorDataProcessor(
        SensorPacketChannel channel,
        IServiceScopeFactory scopeFactory,
        ILogger<SensorDataProcessor> logger,
        IHubContext<SensorHub> hubContext,
        ProductionCounterService productionCounter)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
        _productionCounter = productionCounter;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await foreach (
            var packet in
            _channel.ReadAllAsync(stoppingToken))
        {
            try
            {
                await SaveReadingAsync(
                    packet,
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to save sensor data. DeviceId: {DeviceId}",
                    packet.DeviceId);
            }
        }
    }
    private async Task SaveReadingAsync(
        Contracts.SensorPacket packet,
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<
                    MonitoringDbContext>();

        var totalProduction =
           await _productionCounter.AddProductionAsync(
               packet.DeviceId,
               packet.ProducedQuantity,
               dbContext,
               cancellationToken);

        var entity =
            new SensorReading
            {
                DeviceId =
                    packet.DeviceId,

                Temperature =
                    packet.Temperature,

                Humidity =
                    packet.Humidity,

                TotalProduction = 
                    totalProduction,

                RecordedAt =
                    packet.Timestamp
            };

        dbContext.SensorReadings.Add(entity);

        var realtimeDto = new SensorRealTimeDto
        {
            DeviceId = packet.DeviceId,
            Temeprature = packet.Temperature,
            Humidity = packet.Humidity,
            TotalProduction = totalProduction,
            Timestamp = packet.Timestamp
        };

        await dbContext.SaveChangesAsync(
            cancellationToken);

        await _hubContext.Clients.All.SendAsync(
            "SensorUpdated",
            realtimeDto,
            cancellationToken);

        _logger.LogInformation(
            "Saved: {DeviceId}, Id: {Id}",
            entity.DeviceId,
            entity.Id);
    }
}
