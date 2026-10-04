using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Domain.Entities;

public class SensorReading
{
    public long Id { get; set; }

    public int SensorId { get; set; }

    public Sensor Sensor { get; set; } = null!;

    public double Temperature { get; set; }

    public double Humidity { get; set; }
    
    public long TotalProduction { get; set; }

    public DateTime RecordedAt { get; set; }

}
