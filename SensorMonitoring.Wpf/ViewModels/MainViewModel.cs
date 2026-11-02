using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentView;

    public DashboardViewModel Dashboard { get; }
    public DataHistoryViewModel DataHistory { get; }
    public AlarmHistoryViewModel AlarmHistory { get; }
    public SensorSettingsViewModel SensorSettings { get; }

    public SidebarViewModel Sidebar { get; }
    public StatusBarViewModel StatusBar { get; }

    public MainViewModel(
        SidebarViewModel sidebar,
        StatusBarViewModel statusBar,
        DashboardViewModel dashboard,
        DataHistoryViewModel dataHistory,
        AlarmHistoryViewModel alarmHistory,
        SensorSettingsViewModel sensorSettings)
    {
        Sidebar = sidebar;
        StatusBar = statusBar;
        Dashboard = dashboard;
        DataHistory = dataHistory;
        AlarmHistory = alarmHistory;
        SensorSettings = sensorSettings;

        AlarmHistory = alarmHistory;
        SensorSettings = sensorSettings;

        Sidebar.NavigationRequested +=
            OnNavigationRequested;

        CurrentView = Dashboard;
    }
    private void OnNavigationRequested(
        string destination)
    {
        CurrentView = destination switch
        {
            "Dashboard" => Dashboard,
            "DataHistory" => DataHistory,
            "AlarmHistory" => AlarmHistory,
            "SensorSettings" => SensorSettings,

            _ => Dashboard
        };
    }
}
