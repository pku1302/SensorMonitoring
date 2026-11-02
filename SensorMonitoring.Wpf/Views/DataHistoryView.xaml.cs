using SensorMonitoring.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SensorMonitoring.Wpf.Views;

/// <summary>
/// DataHistoryView.xaml에 대한 상호 작용 논리
/// </summary>
public partial class DataHistoryView : UserControl
{
    public DataHistoryView()
    {
        InitializeComponent();
    }

    private void DataHistoryView_Loaded(
        object sender, RoutedEventArgs e)
    {
        if (DataContext is DataHistoryViewModel vm)
            vm.Initialize();
    }
}
