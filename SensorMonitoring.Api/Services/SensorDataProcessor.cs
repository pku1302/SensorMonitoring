using SensorMonitoring.Application.Services;

namespace SensorMonitoring.Api.Services;

public class SensorDataProcessor : BackgroundService
{
    private readonly SensorPacketChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SensorDataProcessor> _logger;

    public SensorDataProcessor(
        SensorPacketChannel channel,
        IServiceScopeFactory scopeFactory,
        ILogger<SensorDataProcessor> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
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
                await using var scope =
                    _scopeFactory.CreateAsyncScope();

                var processingService = scope.ServiceProvider
                    .GetRequiredService<SensorDataProcessingService>();

                await processingService.ProcessAsync(
                    packet,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when(stoppingToken.IsCancellationRequested)
            {
                break;
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
}
