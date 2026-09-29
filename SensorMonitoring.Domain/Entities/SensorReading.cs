using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Domain.Entities;

public class SensorReading
{
    public long Id { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public double Temperature { get; set; }

    public double Humidity { get; set; }
    
    public long TotalProduction { get; set; }

    public DateTime RecordedAt { get; set; }

}
