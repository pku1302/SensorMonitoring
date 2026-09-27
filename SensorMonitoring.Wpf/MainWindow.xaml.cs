using Microsoft.AspNetCore.SignalR.Client;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.ViewModels;
using System.Windows;

namespace SensorMonitoring.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();

            DataContext = _viewModel;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await _viewModel.StartAsync();
        }

        private async void MainWindow_Closed(
            object? sender,
            EventArgs e)
        {
            await _viewModel.StopAsync();
        }
    }
}