using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorReadingDto
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public long TotalProduction { get; set; }
    public DateTime RecordedAt { get; set; }
}
