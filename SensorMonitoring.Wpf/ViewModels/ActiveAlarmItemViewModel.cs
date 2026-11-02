using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class ActiveAlarmItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string deviceId = "";

    [ObservableProperty]
    private SensorStatusDto status;

    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    private DateTime occurredAt;

    public string StatusIcon => Status switch
    {
        SensorStatusDto.Danger => "●",
        SensorStatusDto.Warning => "▲",
        SensorStatusDto.Offline => "─",
        _ => "●"
    };

    public string StatusText => Status switch
    {
        SensorStatusDto.Danger => "위험",
        SensorStatusDto.Warning => "주의",
        SensorStatusDto.Offline => "끊김",
        _ => "정상"
    };

    public string DisplayTime => OccurredAt.ToLocalTime().ToString("HH:mm:ss");
    partial void OnOccurredAtChanged(DateTime value)
    {
        OnPropertyChanged(nameof(DisplayTime));    
    }
}
