using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Api.Hubs;
using SensorMonitoring.Api.Models;
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
    private readonly SensorStatusService _sensorStatusService;
    private readonly SensorCacheService _sensorCacheService;

    public SensorDataProcessor(
        SensorPacketChannel channel,
        IServiceScopeFactory scopeFactory,
        ILogger<SensorDataProcessor> logger,
        IHubContext<SensorHub> hubContext,
        ProductionCounterService productionCounter,
        SensorCacheService sensorCacheService,
        SensorStatusService sensorStatusService)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
        _productionCounter = productionCounter;
        _sensorCacheService = sensorCacheService;
        _sensorStatusService = sensorStatusService;
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
        SensorPacket packet,
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<
                    MonitoringDbContext>();

        var sensorInfo =
            await _sensorCacheService.GetAsync(
                packet.DeviceId,
                dbContext,
                cancellationToken);

        if (sensorInfo is null)
            return;

        var totalProduction =
           await _productionCounter.AddProductionAsync(
               sensorInfo.SensorId,
               packet.ProducedQuantity,
               dbContext,
               cancellationToken);

        var status = _sensorStatusService.Evaluate(
            packet.Temperature,
            packet.Humidity,
            sensorInfo.SensorAlarmSetting);

        var entity =
            new SensorReading
            {
                SensorId = sensorInfo.SensorId,

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
            Temeperature = packet.Temperature,
            Humidity = packet.Humidity,
            TotalProduction = totalProduction,
            Status = status,
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
            entity.SensorId,
            entity.Id);
    }
}
