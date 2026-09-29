using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorPacket
{
    public string DeviceId { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public long ProducedQuantity { get; set; }
    public DateTime Timestamp { get; set; }
}
