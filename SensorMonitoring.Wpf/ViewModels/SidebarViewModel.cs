using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.ViewModels;
public partial class SidebarViewModel : ObservableObject
{
    [ObservableProperty]
    private string selectedMenu = "Dashboard";

    public event Action<string>? NavigationRequested;

    [RelayCommand]
    private void Navigate(string menu)
    {
        if (SelectedMenu == menu)
            return;

        SelectedMenu = menu;
        NavigationRequested?.Invoke(menu);
    }

    [RelayCommand]
    private void NavigateDashboard()
    {
        Navigate("Dashboard");
    }

    [RelayCommand]
    private void NavigateDataHistory()
    {
        Navigate("DataHistory");
    }


    [RelayCommand]
    private void NavigateAlarmHistory()
    {
        Navigate("AlarmHistory");
    }

    [RelayCommand]
    private void NavigateSensorSettings()
    {
        Navigate("SensorSettings");
    }
}
