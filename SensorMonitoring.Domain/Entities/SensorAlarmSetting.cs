using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Domain.Entities;

public class SensorAlarmSetting
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public Sensor Sensor { get; set; } = null!;
    public double? TemperatureWarning { get; set; }
    public double? TemperatureDanger { get; set; }
    public double? HumidityWarning { get; set; }
    public double? HumidityDanger { get; set; }
    public int OfflineTimeoutSeconds { get; set; } = 5;
    public bool IsEnabled { get; set; } = true;
    public DateTime UpdatedAt { get; set; }

}
