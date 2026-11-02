using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorHistoryChartPointDto
{
    public DateTime Timestamp { get; set; }

    public double AverageTemperature { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }

    public double AverageHumidity { get; set; }
    public double MaxHumidity { get; set; }
    public double MinHumidity { get; set; }
}
