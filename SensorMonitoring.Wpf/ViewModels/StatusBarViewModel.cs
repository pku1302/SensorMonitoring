using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Wpf.Services;
using System.Windows;
using System.Windows.Threading;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class StatusBarViewModel : ObservableObject
{
    private readonly ISensorRealtimeService _realtimeService;
    private readonly DispatcherTimer _clockTimer;

    [ObservableProperty]
    private string connectionStatus;

    [ObservableProperty]
    private string currentTime = string.Empty;

    [ObservableProperty]
    private int sensorCount;

    public StatusBarViewModel(
        ISensorRealtimeService realtimeService)
    {
        _realtimeService = realtimeService;

        _realtimeService.ConnectionStatusChanged +=
            OnSignalConnectionStatusChanged;

        ConnectionStatus
            = _realtimeService.ConnectionStatus;

        _clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _clockTimer.Tick += (_, _) =>
        {
            CurrentTime =
                DateTime.Now.ToString(
                    "HH:mm");
        };

        CurrentTime =
            DateTime.Now.ToString(
                "HH:mm");

        _clockTimer.Start();
    }
    private void OnSignalConnectionStatusChanged(
        string status)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ConnectionStatus = status;
        });
    }

    public void SetSensorCount(int count)
    {
        SensorCount = count;
    }
}
