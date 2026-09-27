using CommunityToolkit.Mvvm.ComponentModel;
using System.Data;

namespace SensorMonitoring.Wpf.ViewModels;
public partial class SensorItemViewModel : ObservableObject
{
    public string DeviceId { get; }

    public DateTime LocalTimestamp =>
        Timestamp.ToLocalTime();

    public string ConnectionText =>
        IsOnline ? "Connected" : "Offline";

    [ObservableProperty]
    private double temperature;

    [ObservableProperty]
    private double humidity;

    [ObservableProperty]
    private DateTime timestamp;

    [ObservableProperty]
    private bool isOnline;

    [ObservableProperty]
    private string status = "Unknown";

    public SensorItemViewModel(string deviceId)
    {
        DeviceId = deviceId;
    }

    partial void OnIsOnlineChanged(bool value)
    {
        OnPropertyChanged(nameof(ConnectionText));
    }

    public void Update(
        double temperature,
        double humidity,
        DateTime timestamp)
    {
        Temperature = temperature;
        Humidity = humidity;
        Timestamp = timestamp;

        OnPropertyChanged(nameof(LocalTimestamp));

        IsOnline = true;

        UpdateStatus();
    }

    public void MarkOffline()
    {
        IsOnline = false;
        Status = "Offline";
    }

    private void UpdateStatus()
    {
        if (!IsOnline)
        {
            Status = "Offline";
            return;
        }

        if (Temperature >= 30)
        {
            Status = "High Temeprature";
        }
        else if (Humidity >= 70)
        {
            Status = "High Humidity";
        }
        else
        {
            Status = "Normal";
        }
    }
}
