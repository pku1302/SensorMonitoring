using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorRealTimeDto
{
    public string DeviceId { get; set; } = string.Empty;
    public double Temeprature { get; set; }
    public double Humidity { get; set; }
    public long TotalProduction { get; set; }
    public DateTime Timestamp { get; set; }
}
