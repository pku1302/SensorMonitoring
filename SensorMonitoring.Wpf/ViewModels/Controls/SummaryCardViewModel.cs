using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Wpf.ViewModels.Controls;

public partial class SummaryCardViewModel : ObservableObject
{
    public string Title { get; }
    public SensorStatusDto Status { get; }
    
    [ObservableProperty]
    private int count = 0;

    public SummaryCardViewModel(
        string title,
        SensorStatusDto status)
    {
        Title = title;
        Status = status;
    }
    public void UpdateCount(int count)
    {
        Count = count;
    }
}
