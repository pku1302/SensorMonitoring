using Microsoft.AspNetCore.SignalR.Client;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.ViewModels;
using System.Windows;

namespace SensorMonitoring.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow(
            MainViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}