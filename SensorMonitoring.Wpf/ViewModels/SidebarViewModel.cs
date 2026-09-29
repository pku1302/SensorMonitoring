using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.ViewModels;
public partial class SidebarViewModel : ObservableObject
{
    public event Action<string>? NavigationRequested;

    [RelayCommand]
    private void Navigate(string destination)
    {
        NavigationRequested?.Invoke(destination);
    }

}
