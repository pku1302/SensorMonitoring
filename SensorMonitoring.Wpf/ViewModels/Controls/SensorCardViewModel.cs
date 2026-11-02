using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace SensorMonitoring.Wpf.ViewModels.Controls;

public partial class SensorCardViewModel : ObservableObject
{
    public string DeviceId { get; }

    [ObservableProperty]
    private double temperature;

    [ObservableProperty]
    private double humidity;

    [ObservableProperty]
    private SensorStatusDto status;

    public SensorCardViewModel(string deviceId)
    {
        DeviceId = deviceId;
    }

    public void Update(SensorRealTimeDto dto)
    {
        Temperature = dto.Temperature;
        Humidity = dto.Humidity;
        Status = dto.Status;
    }

}
