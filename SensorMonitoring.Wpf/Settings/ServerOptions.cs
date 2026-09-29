using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.Settings;

public class ServerOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string SignalRHubPath { get; set; } = string.Empty;
    public string SignalRUrl =>
        $"{BaseUrl.TrimEnd('/')}/{SignalRHubPath.TrimStart('/')}";

}
