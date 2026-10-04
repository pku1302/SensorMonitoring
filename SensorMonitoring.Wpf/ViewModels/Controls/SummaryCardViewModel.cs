using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.ViewModels.Controls;

public partial class SummaryCardViewModel : ObservableObject
{
    public string Title { get; }
    public SensorStatus Status { get; }
    
    [ObservableProperty]
    private int count = 0;

    public SummaryCardViewModel(
        string title,
        SensorStatus status)
    {
        Title = title;
        Status = status;
    }
    public void UpdateCount(int count)
    {
        Count = count;
    }
}
