using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public interface ISensorRealtimeService
{
    public string ConnectionStatus { get; }
    event Action<SensorRealTimeDto>? SensorUpdated;

    event Action<string>? ConnectionStatusChanged;

    Task StartAsync();

    Task StopAsync();
}
