using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Domain.Entities;

public class Sensor
{
    public int Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}
