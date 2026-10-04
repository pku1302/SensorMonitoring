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
    public SensorAlarmSetting? AlarmSetting { get; set; }
    public ICollection<SensorAlarmHistory> AlarmHistories { get; set; }
        = new List<SensorAlarmHistory>();
    public ICollection<SensorReading> Readings { get; set; }
        = new List<SensorReading>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
