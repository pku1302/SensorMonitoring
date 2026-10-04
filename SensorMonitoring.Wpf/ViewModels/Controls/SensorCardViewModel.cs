using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Wpf.ViewModels.Controls;

public partial class SensorCardViewModel : ObservableObject
{
    public string DeviceId { get; }

    [ObservableProperty]
    private double temperature;

    [ObservableProperty]
    private double humidity;

    [ObservableProperty]
    private SensorStatus status;

    public SensorCardViewModel(string deviceId)
    {
        DeviceId = deviceId;
    }

    public void Update(SensorRealTimeDto dto)
    {
        Temperature = dto.Temeperature;
        Humidity = dto.Humidity;
        Status = dto.Status;
    }

}
