using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using SensorMonitoring.Contractsl;
using SensorMonitoring.Wpf.Services;
using SensorMonitoring.Wpf.ViewModels.Chart;
using SensorMonitoring.Wpf.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ISensorRealtimeService _realtimeService;
    private readonly ISensorChartApiService _chartApiService;
    private readonly Dictionary<string, SensorChartsDto> _chartCache = new();
    private bool _chartsLoaded;
    public ObservableCollection<SensorCardViewModel> Sensors { get; }
        = new();
    public ObservableCollection<SummaryCardViewModel> StatusSummaries { get; }
    public RealtimeChartViewModel TemperatureChart { get; } = new();
    public RealtimeChartViewModel HumidityChart { get; } = new();
    public ActiveAlarmViewModel ActiveAlarms { get; } 

    private readonly SummaryCardViewModel _normalSummary;
    private readonly SummaryCardViewModel _warningSummary;
    private readonly SummaryCardViewModel _dangerSummary;
    private readonly SummaryCardViewModel _offlineSummary;

    [ObservableProperty]
    private int _sensorCount = 0;

    [ObservableProperty]
    private SensorCardViewModel? _selectedSensor = null;

    public DashboardViewModel(
        ISensorRealtimeService realtimeService,
        ISensorChartApiService sensorChartApiService)
    {
        _realtimeService = realtimeService;

        _realtimeService.SensorUpdated += OnSensorUpdated;

        _chartApiService = sensorChartApiService;

        _normalSummary =
            new SummaryCardViewModel(
                "정상",
                SensorStatusDto.Normal);

        _warningSummary =
            new SummaryCardViewModel(
                "주의",
                SensorStatusDto.Warning);

        _dangerSummary =
            new SummaryCardViewModel(
                "위험",
                SensorStatusDto.Danger);

        _offlineSummary =
            new SummaryCardViewModel(
                "수신 끊김",
                SensorStatusDto.Offline);

        ActiveAlarms =
            new ActiveAlarmViewModel(
                realtimeService);

        StatusSummaries = new ObservableCollection<SummaryCardViewModel>
        {
            _normalSummary,
            _warningSummary,
            _dangerSummary,
            _offlineSummary
        };

        TemperatureChart.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(RealtimeChartViewModel.IsPaused)
                && !TemperatureChart.IsPaused
                && SelectedSensor is not null)
            {
                RefreshChartFromCache(SelectedSensor.DeviceId, TemperatureChart);
            }
        };

        HumidityChart.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(RealtimeChartViewModel.IsPaused)
                && !HumidityChart.IsPaused
                && SelectedSensor is not null)
            {
                RefreshChartFromCache(SelectedSensor.DeviceId, HumidityChart);
            }
        };
    }
    private void RefreshChartFromCache(
        string deviceId,
        RealtimeChartViewModel chartViewModel)
    {
        if (!_chartCache.TryGetValue(deviceId, out var chart))
        {
            return;
        }

        var trend = chart.Trends.FirstOrDefault(
            x => x.Name == chartViewModel.Name);

        if (trend is null)
            return;

        chartViewModel.Load(trend);
    }

    public async Task InitializeChartsAsync()
    {
        if (_chartsLoaded)
            return;

        try
        {
            var charts = await _chartApiService.GetAllAsync();

            _chartCache.Clear();

            foreach (var chart in charts)
            {
                _chartCache[chart.DeviceId] = chart;
            }

            _chartsLoaded = true;

            if (SelectedSensor is not null)
            {
                ShowSelectedSensorChart(SelectedSensor.DeviceId);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    partial void OnSelectedSensorChanged(
        SensorCardViewModel? value)
    {
        // 센서가 변경되면 두 차트 모두 일시정지 해제
        TemperatureChart.IsPaused = false;
        HumidityChart.IsPaused = false;

        if (value is null)
        {
            TemperatureChart.Clear();
            HumidityChart.Clear();
            return;
        }

        ShowSelectedSensorChart(value.DeviceId);
    }
    private void ShowSelectedSensorChart(string deviceId)
    {
        if (!_chartCache.TryGetValue(deviceId, out var chart))
        {
            TemperatureChart.Clear();
            HumidityChart.Clear();
            return;
        }

        var temperature = chart.Trends
            .FirstOrDefault(x => x.Name == "온도");

        var humidity = chart.Trends
            .FirstOrDefault(x => x.Name == "습도");

        if (temperature is not null)
            TemperatureChart.Load(temperature);
        else
            TemperatureChart.Clear();

        if (humidity is not null)
            HumidityChart.Load(humidity);
        else
            HumidityChart.Clear();
    }

    private void OnSensorUpdated(SensorRealTimeDto dto)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // 기존 센서 카드 갱신 로직
            UpdateSensorItem(dto);

            // 모든 센서의 차트 캐시 갱신
            UpdateChartCache(dto);

            // 현재 선택한 센서의 차트만 화면에 갱신
            if (SelectedSensor?.DeviceId == dto.DeviceId)
            {
                TemperatureChart.AddPoint(
                    dto.Timestamp,
                    dto.Temperature
                );

                HumidityChart.AddPoint(
                    dto.Timestamp,
                    dto.Humidity
                );
            }
        });
    }
    private void UpdateSensorItem(SensorRealTimeDto dto)
    {
        var sensor = Sensors.FirstOrDefault(
            x => x.DeviceId == dto.DeviceId);

        if (sensor is null)
        {
            sensor = new SensorCardViewModel(dto.DeviceId);

            Sensors.Add(sensor);
        }

        if (_chartsLoaded && SelectedSensor is null)
        {
            SelectedSensor = sensor;
        }

        sensor.Update(dto);

        UpdateSensorCount();
    }

    private void UpdateSensorCount()
    {
        SensorCount = Sensors.Count;

        _normalSummary.Count =
            Sensors.Count(x => x.Status == SensorStatusDto.Normal);

        _warningSummary.Count =
            Sensors.Count(x => x.Status == SensorStatusDto.Warning);

        _dangerSummary.Count =
            Sensors.Count(x => x.Status == SensorStatusDto.Danger);

        _offlineSummary.Count =
            Sensors.Count(x => x.Status == SensorStatusDto.Offline);
    }
    private void UpdateChartCache(SensorRealTimeDto dto)
    {
        if (!_chartCache.TryGetValue(dto.DeviceId, out var chart))
            return;

        var temperature = chart.Trends
            .FirstOrDefault(x => x.Name == "온도");

        var humidity = chart.Trends
            .FirstOrDefault(x => x.Name == "습도");

        AddPoint(temperature, dto.Timestamp, dto.Temperature);
        AddPoint(humidity, dto.Timestamp, dto.Humidity);
    }

    private static void AddPoint(
        SensorTrendDto? trend,
        DateTime timestamp,
        double value)
    {
        if (trend is null)
            return;

        trend.Values.Add(new ChartData
        {
            Time = timestamp,
            Value = value
        });

        if (trend.Values.Count > 180)
            trend.Values.RemoveAt(0);
    }
}
