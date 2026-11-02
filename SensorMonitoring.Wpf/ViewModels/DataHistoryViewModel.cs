using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using SensorMonitoring.Contractsl;
using SensorMonitoring.Wpf.Services;
using SensorMonitoring.Wpf.ViewModels.Chart;
using SensorMonitoring.Wpf.ViewModels.Controls;
using System.ComponentModel;
using System.Diagnostics;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class DataHistoryViewModel : ObservableObject
{
    private CancellationTokenSource? _sensorLoadCts;
    private CancellationTokenSource? _chartLoadCts;

    private readonly ISensorHistoryApiService
        _sensorHistoryApiService;
    public CalendarViewModel Calendar { get; } = new();
    public SensorSelectionViewModel SensorSelection { get; } = new();

    public HistoryChartViewModel TemperatureChart { get; } = new();
    public HistoryChartViewModel HumidityChart { get; } = new();

    private bool _initialized;

    public DataHistoryViewModel(
        ISensorHistoryApiService sensorHistoryApiService)
    {
        _sensorHistoryApiService = sensorHistoryApiService;

        Calendar.PropertyChanged += OnCalendarPropertyChanged;

        SensorSelection.PropertyChanged +=
            OnSensorSelectionPropertyChanged;
    }

    public void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;

        // 현재 달력의 선택 날짜를 이용해 최초 API 요청
        LoadSelectedDate();
    }
    private void LoadSelectedDate()
    {
        _sensorLoadCts?.Cancel();

        if (Calendar.SelectedDate is null)
        {
            SensorSelection.LoadSensors([]);
            return;
        }

        var cts = new CancellationTokenSource();
        _sensorLoadCts = cts;

        _ = LoadSensorsAsync(
            DateOnly.FromDateTime(Calendar.SelectedDate.Value),
            cts);
    }

    private void OnCalendarPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(CalendarViewModel.SelectedDate))
            return;
        
        // 1. 이전 차트 취소 및 초기화
        CancelAndClearChart();

        // 2. 이전 날짜의 선택 센서 해제
        SensorSelection.SelectedSensor = null;

        if (Calendar.SelectedDate is DateTime date)
        {
            DateTime start = date.Date;
            DateTime end = start.AddDays(1);

            foreach (var chart in new[] { TemperatureChart, HumidityChart })
            {
                chart.StartTime = start;
                chart.EndTime = end;

                chart.VisibleStartTime = start;
                chart.VisibleEndTime = end;
            }
        }

        // 날짜별 센서 목록 재조회
        LoadSelectedDate();
    }

    private void OnSensorSelectionPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName !=
            nameof(SensorSelectionViewModel.SelectedSensor))
            return;

        LoadSelectedChart();
    }
    private void CancelAndClearChart()
    {
        _chartLoadCts?.Cancel();

        TemperatureChart.Clear();
        HumidityChart.Clear();
    }
    private void LoadSelectedChart()
    {
        _chartLoadCts?.Cancel();

        var date = Calendar.SelectedDate;
        var sensor = SensorSelection.SelectedSensor;

        if (date is null || sensor is null)
        {
            TemperatureChart.Clear();
            HumidityChart.Clear();
            return;
        }

        TemperatureChart.Clear();
        HumidityChart.Clear();

        var cts = new CancellationTokenSource();
        _chartLoadCts = cts;

        _ = LoadChartAsync(
            sensor.DeviceId,
            DateOnly.FromDateTime(date.Value),
            cts);
    }
    private async Task LoadChartAsync(
        string deviceId,
        DateOnly date,
        CancellationTokenSource cts)
    {
        try
        {
            var results = await _sensorHistoryApiService
                .GetChartByDateAsync(deviceId, date, cts.Token);

            var points = results.Points;

            if (cts.IsCancellationRequested)
                return;

            // 결과가 도착했을 때도 같은 센서인지 확인
            if (SensorSelection.SelectedSensor?.DeviceId != deviceId)
                return;

            // 결과가 도착했을 때도 같은 날짜인지 확인
            if (Calendar.SelectedDate is null ||
                DateOnly.FromDateTime(Calendar.SelectedDate.Value) != date)
                return;

            // 온도 차트 데이터 변환
            var temperatureTrend = new SensorTrendDto
            {
                Name = "온도",
                Unit = "°C",
                MinValue = 0,
                MaxValue = 100,
                
                DangerValue = results.TemperatureDangerValue,

                Values = points.Select(x => (ChartData)new HistoryChartData
                {
                    Time = x.Timestamp.ToLocalTime(),
                    Value = x.AverageTemperature,
                    MinValue = x.MinTemperature,
                    MaxValue = x.MaxTemperature,
                }).ToList()
            };

            // 습도 차트 데이터 변환
            var humidityTrend = new SensorTrendDto
            {
                Name = "습도",
                Unit = "%",
                MinValue = 0,
                MaxValue = 100,

                DangerValue = results.HumidityDangerValue,
                
                Values = points.Select(x => (ChartData)new HistoryChartData
                {
                    Time = x.Timestamp.ToLocalTime(),
                    Value = x.AverageHumidity,
                    MinValue = x.MinHumidity,
                    MaxValue = x.MaxHumidity,
                }).ToList()
            };

            TemperatureChart.Load(temperatureTrend);
            HumidityChart.Load(humidityTrend);
        }
        catch (OperationCanceledException)
            when (cts.IsCancellationRequested)
        {
            // 날짜 또는 센서 변경에 따른 정상적인 취소
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            if (ReferenceEquals(_chartLoadCts, cts))
                _chartLoadCts = null;

            cts.Dispose();
        }
    }

    private async Task LoadSensorsAsync(
        DateOnly date,
        CancellationTokenSource cts)
    {
        try
        {
            var sensors = await _sensorHistoryApiService
                .GetSensorsByDateAsync(date, cts.Token);

            // 이전 요청이 취소됐다면 결과 무시
            if (cts.IsCancellationRequested)
                return;

            // 현재 선택 날짜와 요청 날짜가 다른 경우 무시
            if (Calendar.SelectedDate is null ||
                DateOnly.FromDateTime(Calendar.SelectedDate.Value) != date)
                return;

            SensorSelection.LoadSensors(sensors);
        }
        catch (OperationCanceledException)
            when (cts.IsCancellationRequested)
        {
            // 날짜 변경으로 인한 정상적인 취소
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            if (ReferenceEquals(_sensorLoadCts, cts))
                _sensorLoadCts = null;

            cts.Dispose();
        }
    }
}
