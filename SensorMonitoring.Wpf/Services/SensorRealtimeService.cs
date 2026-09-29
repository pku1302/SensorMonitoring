using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.Settings;

namespace SensorMonitoring.Wpf.Services;

public class SensorRealtimeService : ISensorRealtimeService
{
    private readonly HubConnection _connection;

    public string ConnectionStatus { get; private set; }
        = "Disconnected";
    public event Action<SensorRealTimeDto>? SensorUpdated;
    public event Action<string>? ConnectionStatusChanged;
    public SensorRealtimeService(
        IOptions<ServerOptions> options)
    {
        var serverOptions = options.Value;

        _connection =
            new HubConnectionBuilder()
                .WithUrl(serverOptions.SignalRUrl)
                .WithAutomaticReconnect()
                .Build();

        _connection.On<SensorRealTimeDto>(
            "SensorUpdated",
            dto =>
            {
                SensorUpdated?.Invoke(dto);
            });

        _connection.Reconnecting += ex =>
        {
            SetConnectionStatus("Reconnecting...");

            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            SetConnectionStatus("Connected");

            return Task.CompletedTask;
        };

        _connection.Closed += ex =>
        {
            SetConnectionStatus("Disconnected");

            return Task.CompletedTask;
        };
    }

    private void SetConnectionStatus(
        string status)
    {
        ConnectionStatus = status;

        ConnectionStatusChanged?.Invoke(status);
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();

        SetConnectionStatus("Connected");
    }
    public async Task StopAsync()
    {
        await _connection.DisposeAsync();
    }
}
