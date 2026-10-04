using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.Services;
using SensorMonitoring.Wpf.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ISensorRealtimeService _realtimeService;

    public ObservableCollection<SensorCardViewModel> Sensors { get; }
        = new();

    public ObservableCollection<SummaryCardViewModel> StatusSummaries { get; }

    private readonly SummaryCardViewModel _normalSummary;
    private readonly SummaryCardViewModel _warningSummary;
    private readonly SummaryCardViewModel _dangerSummary;
    private readonly SummaryCardViewModel _offlineSummary;

    public DashboardViewModel(
        ISensorRealtimeService realtimeService)
    {
        _realtimeService = realtimeService;

        _realtimeService.SensorUpdated += OnSensorUpdated;

        _normalSummary =
            new SummaryCardViewModel(
                "정상",
                SensorStatus.Normal);

        _warningSummary =
            new SummaryCardViewModel(
                "주의",
                SensorStatus.Warning);

        _dangerSummary =
            new SummaryCardViewModel(
                "위험",
                SensorStatus.Danger);

        _offlineSummary =
            new SummaryCardViewModel(
                "수신 끊김",
                SensorStatus.Offline);

        StatusSummaries = new ObservableCollection<SummaryCardViewModel>
        {
            _normalSummary,
            _warningSummary,
            _dangerSummary,
            _offlineSummary
        };
    }
    private void OnSensorUpdated(SensorRealTimeDto dto)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var sensor = Sensors.FirstOrDefault(
                x => x.DeviceId == dto.DeviceId);

            if (sensor is null)
            {
                sensor = new SensorCardViewModel(dto.DeviceId);

                Sensors.Add(sensor);
            }

            sensor.Update(dto);

            UpdateSensorCount();
        });
    }
    private void UpdateSensorCount()
    {
        _normalSummary.Count = 
            Sensors.Count(x => x.Status == SensorStatus.Normal);

        _warningSummary.Count = 
            Sensors.Count(x => x.Status == SensorStatus.Warning);

        _dangerSummary.Count =
            Sensors.Count(x => x.Status == SensorStatus.Danger);

        _offlineSummary.Count =
            Sensors.Count(x => x.Status == SensorStatus.Offline);
    }
}
