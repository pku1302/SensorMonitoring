using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorChartPointDto
{
    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public DateTime Timestamp { get; set; }
}
