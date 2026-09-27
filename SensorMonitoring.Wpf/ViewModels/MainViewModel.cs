using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.AspNetCore.SignalR.Client;
using SensorMonitoring.Contracts;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const int MaxChartPoints = 30;
    private readonly HubConnection _connection;
    private readonly DispatcherTimer _statusTimer;
    private readonly ObservableCollection<double>
        _temperatureValues = new();
    private readonly ObservableCollection<double>
        _humidityValues = new();

    public ISeries[] TemperatureSeries { get; }
    public ISeries[] HumiditySeries { get; }

    public ObservableCollection<SensorItemViewModel> Sensors { get; }
        = new();

    [ObservableProperty]
    private string connectionStatus = "Disconnected";

    [ObservableProperty]
    private SensorItemViewModel? selectedSensor;

    public MainViewModel()
    {
        _connection =
            new HubConnectionBuilder()
                .WithUrl("https://localhost:7081/hubs/sensor") //https
                .WithAutomaticReconnect()
                .Build();

        _statusTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _statusTimer.Tick += (_, _) =>
        {
            CheckSensorStatus();
        };

        _statusTimer.Start();

        _connection.On<SensorPacket>(
            "SensorUpdated",
            packet =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateSensor(packet);
                });
            });

        _connection.Reconnecting += exception =>
        {
            ConnectionStatus = "Reconnecting...";
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            ConnectionStatus = "Connected";
            return Task.CompletedTask;
        };

        _connection.Closed += exception =>
        {
            ConnectionStatus = "Disconnected";
            return Task.CompletedTask;
        };

        TemperatureSeries =
        [
            new LineSeries<double>
            {
                Name = "Temperature",
                Values = _temperatureValues,
                GeometrySize = 6,
                LineSmoothness = 0
            }
        ];

        HumiditySeries =
        [
            new LineSeries<double>
            {
                Name = "Humidity",
                Values = _humidityValues,
                GeometrySize= 6,
                LineSmoothness= 0
            }
        ];
    }

    partial void OnSelectedSensorChanged(
        SensorItemViewModel? value)
    {
        _temperatureValues.Clear();
        _humidityValues.Clear();
    }

    public async Task StartAsync()
    {
        try
        {
            await _connection.StartAsync();
            ConnectionStatus = "Connected";
        }
        catch (Exception ex)
        {
            ConnectionStatus =
                $"Connection failed: {ex.Message}";
        }
    }

    public async Task StopAsync()
    {
        await _connection.DisposeAsync();
    }
    private void UpdateSensor(
        SensorPacket packet)
    {
        var sensor =
            Sensors.FirstOrDefault(
                x => x.DeviceId == packet.DeviceId);

        if (sensor is null)
        {
            sensor =
                new SensorItemViewModel(
                    packet.DeviceId);

            Sensors.Add(sensor);
        }

        sensor.Update(
            packet.Temperature,
            packet.Humidity,
            packet.Timestamp);

        if (SelectedSensor?.DeviceId == packet.DeviceId)
        {
            AddChartPoint(packet);
        }
    }

    private void AddChartPoint(
        SensorPacket packet)
    {
        _temperatureValues.Add(
            packet.Temperature);

        _humidityValues.Add(
            packet.Humidity);

        if (_temperatureValues.Count >
            MaxChartPoints)
        {
            _temperatureValues.RemoveAt(0);
        }

        if (_humidityValues.Count >
            MaxChartPoints)
        {
            _humidityValues.RemoveAt(0);
        }
    }

    private void CheckSensorStatus()
    {
        var now = DateTime.UtcNow;

        foreach (var sensor in Sensors)
        {
            var elapsed =
                now - sensor.Timestamp.ToUniversalTime();

            if (elapsed > TimeSpan.FromSeconds(5))
            {
                sensor.MarkOffline();
            }
        }
    }
}
