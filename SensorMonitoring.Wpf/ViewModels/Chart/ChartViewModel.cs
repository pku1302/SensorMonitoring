using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace SensorMonitoring.Wpf.ViewModels.Chart;
public abstract partial class ChartViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private string unit = "";

    [ObservableProperty]
    private double minValue;

    [ObservableProperty]
    private double maxValue;

    [ObservableProperty]
    private double dangerValue;

    [ObservableProperty]
    private ObservableCollection<ChartData> values = new();
    
    public bool HasData => Values.Count > 0;

    public virtual Visibility ControlVisibility
        => Visibility.Collapsed;
    public void Clear()
    {
        Values.Clear();
        OnPropertyChanged(nameof(HasData));
    }
}
