using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SensorMonitoring.Wpf.Services;
using SensorMonitoring.Wpf.Settings;
using SensorMonitoring.Wpf.ViewModels;
using SensorMonitoring.Wpf.Views;
using System.Windows;

namespace SensorMonitoring.Wpf
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);

                    config.AddJsonFile(
                        "appsettings.json",
                        optional: false,
                        reloadOnChange: true);

                    config.AddJsonFile(
                        $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                        optional: true,
                        reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // =========================
                    // Services
                    // =========================

                    services.Configure<ServerOptions>(
                        context.Configuration.GetSection("Server"));

                    services.AddSingleton<
                        ISensorRealtimeService,
                        SensorRealtimeService>();

                    services.AddHttpClient<
                        ISensorChartApiService,
                        SensorChartApiService>((provider, client) =>
                   {
                       var options = provider
                        .GetRequiredService<IOptions<ServerOptions>>()
                        .Value;

                       client.BaseAddress =
                        new Uri(options.BaseUrl.TrimEnd('/') + "/");
                   });

                    services.AddHttpClient<
                        ISensorHistoryApiService,
                        SensorHistoryApiService>((provider, client) =>
                   {
                       var options = provider
                           .GetRequiredService<
                               IOptions<ServerOptions>>().Value;

                       client.BaseAddress = new Uri(
                           options.BaseUrl.TrimEnd('/') + "/");
                   });

                    // =========================
                    // ViewModels
                    // =========================

                    services.AddSingleton<MainViewModel>();

                    services.AddSingleton<DashboardViewModel>();

                    services.AddSingleton<SidebarViewModel>();

                    services.AddSingleton<StatusBarViewModel>();

                    services.AddSingleton<DataHistoryViewModel>();

                    services.AddSingleton<AlarmHistoryViewModel>();

                    services.AddSingleton<SensorSettingsViewModel>();

                    // =========================
                    // Views
                    // =========================

                    services.AddSingleton<MainWindow>();

                    services.AddTransient<DashboardView>();

                    System.Diagnostics.Debug.WriteLine(
                        $"환경: {context.HostingEnvironment.EnvironmentName}");

                    System.Diagnostics.Debug.WriteLine(
                        $"API 주소: {context.Configuration["Server:BaseUrl"]}");
                })
                .Build();
        }

        protected override async void OnStartup(
            StartupEventArgs e)
        {
            await _host.StartAsync();

            // 먼저 MainWindow를 생성
            // → MainViewModel
            // → StatusBarViewModel 생성
            // → SignalR 이벤트 구독 완료
            var mainWindow =
                _host.Services
                    .GetRequiredService<MainWindow>();

            var realtimeService =
                _host.Services
                    .GetRequiredService<ISensorRealtimeService>();

            await realtimeService.StartAsync();

            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(
            ExitEventArgs e)
        {
            var realtimeService =
                _host.Services
                    .GetRequiredService<ISensorRealtimeService>();

            await realtimeService.StopAsync();

            await _host.StopAsync();

            _host.Dispose();

            base.OnExit(e);
        }
    }
}
