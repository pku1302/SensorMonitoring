using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.AspNetCore.SignalR.Client;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object currentViewModel;

    public DashboardViewModel DashboardViewModel { get; }
    public SidebarViewModel SidebarViewModel { get; }
    public StatusBarViewModel StatusBarViewModel { get; }

    public MainViewModel(
        SidebarViewModel sidebar,
        StatusBarViewModel statusBar,
        DashboardViewModel dashboard)
    {
        SidebarViewModel = sidebar;
        StatusBarViewModel = statusBar;
        DashboardViewModel = dashboard;

        CurrentViewModel = DashboardViewModel;

        SidebarViewModel.NavigationRequested +=
            OnNavigationRequested;
    }
    private void OnNavigationRequested(
        string destination)
    {
        CurrentViewModel = destination switch
        {
            "Dashboard" => DashboardViewModel,
            "Sensors" => DashboardViewModel,
            "Alarms" => DashboardViewModel,
            "Settings" => DashboardViewModel,

            _ => DashboardViewModel
        };
    }
}
