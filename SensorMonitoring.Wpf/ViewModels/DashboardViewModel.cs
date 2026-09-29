using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ISensorRealtimeService _realtimeService;
    private const int MaxChartPoints = 300;
    private readonly ObservableCollection<double>
        _temperatureValues = new();
    private readonly ObservableCollection<double>
        _humidityValues = new();
    public ISeries[] TemperatureSeries { get; }
    public ISeries[] HumiditySeries { get; }
    public ObservableCollection<SensorItemViewModel> Sensors { get; }
        = new();

    [ObservableProperty]
    private SensorItemViewModel? selectedSensor;
    public DashboardViewModel(
        ISensorRealtimeService realtimeService)
    {
        _realtimeService = realtimeService;

        _realtimeService.SensorUpdated +=
            OnSensorUpdated;

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
    private void OnSensorUpdated(
        SensorRealTimeDto dto)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            UpdateSensor(dto);
        });
    }

    partial void OnSelectedSensorChanged(
        SensorItemViewModel? value)
    {
        _temperatureValues.Clear();
        _humidityValues.Clear();
    }


    private void UpdateSensor(
        SensorRealTimeDto dto)
    {
        var sensor =
            Sensors.FirstOrDefault(
                x => x.DeviceId == dto.DeviceId);

        if (sensor is null)
        {
            sensor =
                new SensorItemViewModel(
                    dto.DeviceId);

            Sensors.Add(sensor);
        }

        sensor.Update(
            dto.Temeprature,
            dto.Humidity,
            dto.TotalProduction,
            dto.Timestamp);

        if (SelectedSensor?.DeviceId == dto.DeviceId)
        {
            AddChartPoint(dto);
        }
    }

    private void AddChartPoint(
        SensorRealTimeDto packet)
    {
        _temperatureValues.Add(
            packet.Temeprature);

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
